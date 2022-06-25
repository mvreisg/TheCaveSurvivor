using UnityEngine;
using System.Timers;
using System.Collections;
using TheCaveSurvivor.Controllers;
using TheCaveSurvivor.Characters;
using TheCaveSurvivor.Scene;

namespace TheCaveSurvivor.Enemy{

	/*
	 * Classe: Slime 
	 * 
	 * 		Herda de : 	<<abstract>>
	 * 					Enemies
	 * 
	 * 		Implementa: <<interface>>	 
	 * 					IEnemyRespawnable
	 * 
	 * 	Classe que controla o inimigo slime
	 * 
	 * */

	public class Slime : Enemies,  IEnemyRespawnable {

		public		const 		float	gravityScale = 1f;

		//Chamado uma vez apenas
		protected override void Start () {
			base.Start ();

			enemyAnims = GetComponent<Animator> ();
			enemySprite = GetComponent<SpriteRenderer> ();
			enemyPlayer = GetComponent<AudioSource> ();
			enemyRigid = GetComponent<Rigidbody2D> ();
			enemyCollider = new Collider2D[]{ 
				GetComponent<CircleCollider2D> (),
				GetComponent<BoxCollider2D>()
			};
			enemyClips = new AudioClip[] {
				/*slimejump*/
				Resources.Load<AudioClip> ("Audio/SFX/Enemies/slime_jump"),
				/*slimedie*/
				Resources.Load<AudioClip> ("Audio/SFX/Enemies/slime_die")
			};
			enemySprite.material = lightMaterial;
			startPosition = transform.position;

			distanceToMove = 2f;
			moveForce = 1000f;
			jumpForce = 3000f;
			hitWaitTime = 2f;

			enemyHit = 1;
			healthPoints = 1;

			canMove = true;
			canTakeDamage = true;

		}

		//Atualiza a cada quadro
		protected override void Update(){
			if (!Stages.PausedGame) {
				if (!enemyDied) {
					if (canMove) {
						Move (CalculateDistance ());
					}
				}
				else {
					Respawn (CalculateDistance ());
				}
			}
		}
			
		//Detecta algo que colidiu
		protected void OnCollisionEnter2D(Collision2D col){
			if (!enemyDied) {
				if (col.gameObject.CompareTag("Player")) {
					foreach (Collider2D slimeCol in enemyCollider) {
						if (slimeCol.IsTouching (player.GroundCheckCollider) && !player.OnGround) {
							StartCoroutine (Die ());
						} 
					}
				}
			}
		}

		//Detecta algo que colidiu e permaneceu colidindo
		protected void OnCollisionStay2D(Collision2D col){			
			if (col.gameObject.CompareTag("Player")) {
				Hit (enemyHit);			
			}
		}

		//Detecta algo que entrou na área do colisor
		protected void OnTriggerEnter2D(Collider2D col){
			if (!enemyDied) {
				if (col.gameObject.CompareTag("Pickaxe")) {
					StartCoroutine(Damage (player.PickaxeDamage));
				}			
			}
		}

		//Move o inimigo com base na distancia entre ele e o player
		protected override void Move(float distance){			
			if (distance < distanceToMove) {	
				while (canMove) {
					if (player.transform.position.x > transform.position.x) {
						enemyRigid.AddRelativeForce(new Vector2 (moveForce * Time.fixedDeltaTime, jumpForce * Time.fixedDeltaTime));
						enemySprite.flipX = true;
					}
					if (player.transform.position.x < transform.position.x) {						
						enemyRigid.AddRelativeForce(new Vector2 (-moveForce * Time.fixedDeltaTime, jumpForce * Time.fixedDeltaTime));
						enemySprite.flipX = false;
					}
					StartCoroutine(AudioController.GetClass.StartAudio (enemyPlayer, enemyClips[0], 1f, false));
					canMove = false;
				} 
				StartCoroutine (CanMoveCondition (hitWaitTime));
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
				healthPoints -= damage;
				if (healthPoints <= 0) {
					StartCoroutine (Die ());
				}
				yield return null;
			}
		}

		//Método que inicia a morte
		protected override IEnumerator Die(){
			if (!enemyDied) {
				StartCoroutine(AudioController.GetClass.StartAudio (enemyPlayer, enemyClips[1], 1f, false));
				enemyDied = true;
				Destroy (enemyRigid);
				enemyCollider [0].enabled = false;
				enemyCollider [1].enabled = false;
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
			if (random < 4f) {
				GameObject heart = Instantiate (heartDrop, posToSpawnLoot, Quaternion.identity, collectablesObject.transform) as GameObject;
				yield return new WaitForSecondsRealtime (lootMaxItemWait);
				Destroy (heart);
			}
			if (random < 1f) {
				GameObject lifes = Instantiate (lifeDrop, posToSpawnLoot, Quaternion.identity, collectablesObject.transform) as GameObject;
				yield return new WaitForSecondsRealtime (lootMaxItemWait);
				Destroy (lifes);
			}

		}
			
		//Método que inicia o renascimento
		public void Respawn(float distance){	
			if (!GeometryUtility.TestPlanesAABB(GameCamera.GetClass.GetPlanes, enemyCollider[0].bounds)) {
				if (distance > distanceToMove) {
					float respawnDistance = Vector2.Distance (player.transform.position, startPosition);
					if (respawnDistance > distanceToSpawn) {
						enemyPrefab = Resources.Load ("Prefabs/Enemies/Slime") as GameObject;
						Instantiate (enemyPrefab, startPosition, Quaternion.identity, GameObject.Find ("Enemies").transform);
						Destroy (gameObject);
					}
				}
			}
		}
	}

}