using UnityEngine;
using System.Collections;
using TheCaveSurvivor.Controllers;
using TheCaveSurvivor.Characters;
using TheCaveSurvivor.Scene;

namespace TheCaveSurvivor.Enemy{

	/*
	 * Classe: Zombie 
	 * 
	 * 		Herda de : 	<<abstract>>
	 * 					Enemies
	 * 
	 * 		Implementa: <<interface>>	 
	 * 					IEnemyRespawnable
	 * 
	 * 	Classe que controla o inimigo zumbi
	 * 
	 * */

	public class Zombie : Enemies,  IEnemyRespawnable {

		public  	const 	float		gravityScale = 1f;

		//Chamado uma vez apenas
		protected override void Start(){
			base.Start ();

			enemyAnims = GetComponent<Animator> ();
			enemySprite = GetComponent<SpriteRenderer> ();
			enemyPlayer = GetComponent<AudioSource> ();
			enemyRigid = GetComponent<Rigidbody2D> ();
			enemyCollider = new Collider2D[] { GetComponent<BoxCollider2D> () };
			enemyClips = new AudioClip[] { 
				/*zombienoise*/
				Resources.Load<AudioClip> ("Audio/SFX/Enemies/zombie_noise"), 
				/*zombiedie*/
				Resources.Load<AudioClip> ("Audio/SFX/Enemies/zombie_die") 
			};
			enemySprite.material = lightMaterial;
			startPosition = transform.position;

			distanceToMove = 3f;
			moveForce = 1f;
			jumpForce = 0f;
			hitWaitTime = 3f;
			enemyHit = 1;
			healthPoints = 6;

			canMove = true;
			canTakeDamage = true;
			enemyDied = false;

			StartCoroutine(AudioController.GetClass.StartAudio (enemyPlayer, enemyClips[0], 1f, true));
		}

		//Atualiza a cada quadro
		protected override void Update(){
			if (!Stages.PausedGame) {
				if (!enemyDied) {
					Move (CalculateDistance ());			
				} else {
					Respawn (CalculateDistance ());
				}
			}
		}
			
		//Detecta algo que colidiu e permaneceu colidindo
		protected void OnCollisionStay2D(Collision2D col){			
			if (col.gameObject.CompareTag ("Player")) {
				Hit (enemyHit);
				CanMoveCondition (0.5f);
			}

		}

		//Detecta algo que entrou na área do colisor
		protected void OnTriggerEnter2D(Collider2D col){
			if (col.gameObject.CompareTag("Pickaxe")) {
				StartCoroutine(Damage (player.PickaxeDamage, col.gameObject.transform));			
			}
		}
			
		//Move o inimigo com base na distancia entre ele e o player
		protected override void Move(float distance){			
			if (distance < distanceToMove) {
				if (distance > 0.3f) {
					enemyAnims.enabled = true;
					if (canMove) {						
						enemyAnims.SetBool ("sawPlayer", true);
						if (transform.position.x < player.transform.position.x) {
							transform.Translate (Vector2.right * moveForce * Time.deltaTime);
							enemySprite.flipX = false;
						} else {
							transform.Translate (Vector2.left * moveForce * Time.deltaTime);
							enemySprite.flipX = true;
						}
					}
				}
				else {
					enemyAnims.enabled = false;
				}
			}
			else {
				enemyAnims.SetBool ("sawPlayer", false);
			}
		}

		//Método para dar dano no jogador
		protected override void Hit(int damage){
			if (!enemyDied) {				
				StartCoroutine(player.Damage (damage, transform));			
			}
		}

		//Método para receber dano
		public override IEnumerator Damage(int damage){
			if (!enemyDied) {
				if (canTakeDamage) {
					canTakeDamage = false;
					healthPoints -= damage;
					if (healthPoints <= 0) {
						StartCoroutine (Die ());
					} 
					else {
						StartCoroutine (AudioController.GetClass.StartAudio (enemyPlayer, enemyClips[1], 1f, false));
						StartCoroutine (CoroutinesController.GetClass.Blink_Sprite (enemySprite, Color.red, 0.5f, 0.5f));
					}
					yield return new WaitForSeconds(0.5f);
					StartCoroutine(AudioController.GetClass.StartAudio (enemyPlayer, enemyClips[0], 1f, true));
					canTakeDamage = true;
				}
			}
		}

		//Método para receber dano c/ recuo
		public IEnumerator Damage(int damage, Transform transf){
			if (!enemyDied) {
				if (canTakeDamage) {
					canTakeDamage = false;
					healthPoints -= damage;
					if (healthPoints <= 0) {
						StartCoroutine (Die ());
					} 
					else {
						StartCoroutine(AudioController.GetClass.StartAudio (enemyPlayer, enemyClips[1], 1f, false));
						StartCoroutine (CoroutinesController.GetClass.Blink_Sprite (enemySprite, Color.red, 0.5f, 0.5f));
						if (transf != null) {
							float multiplier = (transf.position.x < transform.position.x) ? (1) : (-1);
							enemyRigid.velocity = new Vector2 (moveForce * multiplier * Time.deltaTime * 300, moveForce * Time.deltaTime * 300);
						}
						yield return new WaitForSeconds(0.5f);
						StartCoroutine(AudioController.GetClass.StartAudio (enemyPlayer, enemyClips[0], 1f, true));
						canTakeDamage = true;
					}
				}					
			}
		}

		//Método que inicia a morte
		protected override IEnumerator Die(){
			if (!enemyDied) {				
				enemyDied = true;
				StartCoroutine(AudioController.GetClass.StartAudio (enemyPlayer, enemyClips[1], 1f, false));
				enemyRigid.isKinematic = true;
				enemyCollider [0].enabled = false;
				StartCoroutine(Loot (transform.position));
				enemyAnims.SetTrigger("dieAnim");	
				yield return new WaitForSeconds(deathWaitTime);
				Destroy (enemyAnims);
				Destroy	(enemySprite);
				if (destroyEnemy) {
					Destroy (gameObject);
				}
			}
		}

		//Método que controla o loot dos inimigos
		protected override IEnumerator Loot(Vector3 posToSpawnLoot){
			float random = Random.Range (lootMinValue, lootMaxValue);
			if (random < 1f) {
				GameObject lifes = Instantiate (lifeDrop, posToSpawnLoot, Quaternion.identity, collectablesObject.transform) as GameObject;
				yield return new WaitForSecondsRealtime (lootMaxItemWait);
				Destroy (lifes);
			}
			else if (random < 4f) {
				GameObject heart = Instantiate (heartDrop, posToSpawnLoot, Quaternion.identity, collectablesObject.transform) as GameObject;
				yield return new WaitForSecondsRealtime (lootMaxItemWait);
				Destroy (heart);
			}
				
		}


		//Método que inicia o renascimento
		public void Respawn(float distance){
			if (!GeometryUtility.TestPlanesAABB(GameCamera.GetClass.GetPlanes, enemyCollider[0].bounds)) {
				if (distance > distanceToMove) {
					float respawnDistance = Vector2.Distance (player.transform.position, startPosition);
					if (respawnDistance > distanceToSpawn) {
						enemyPrefab = Resources.Load ("Prefabs/Enemies/Zombie") as GameObject;
						Instantiate (enemyPrefab, startPosition, Quaternion.identity, GameObject.Find ("Enemies").transform);
						Destroy (gameObject);
					}
				}
			}
		}		

	}

}