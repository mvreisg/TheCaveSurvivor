using UnityEngine;
using System.Collections;
using TheCaveSurvivor.Characters;
using TheCaveSurvivor.Controllers;
using TheCaveSurvivor.Scene;

namespace TheCaveSurvivor.Enemy{

	/*
	 * Classe: Bat 
	 * 
	 * 		Herda de : 	<<abstract>>
	 * 					Enemies

	 * 		Implementa: <<interface>>	 
	 * 					IEnemyRespawnable
	 * 
	 * 	Classe que controla o inimigo morcego
	 * 
	 * */

	public class Bat : Enemies, IEnemyRespawnable {

		public  	const 	float		gravityScale = 0f;

		//Chamado uma vez apenas
		protected override void Start(){
			base.Start ();

			enemyAnims = GetComponent<Animator> ();
			enemySprite = GetComponent<SpriteRenderer> ();
			enemyPlayer = GetComponent<AudioSource> ();
			enemyRigid = GetComponent<Rigidbody2D> ();
			enemyCollider = new Collider2D[]{ GetComponent<CircleCollider2D> () };
			enemyClips = new AudioClip[] {
				Resources.Load<AudioClip> ("Audio/SFX/Enemies/bat_fly"),
				Resources.Load<AudioClip> ("Audio/SFX/Enemies/bat_die")
			};
			enemySprite.material = lightMaterial;
			startPosition = transform.position;

			distanceToMove = 3f;
			moveForce = 240f;
			jumpForce = 0f;
			hitWaitTime = 3f;

			enemyHit = 1;
			healthPoints = 1;

			canMove = true;
			canTakeDamage = true;

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
			
		//Detecta colisão com algo
		protected void OnCollisionEnter2D(Collision2D col){
			if (col.gameObject.layer == roofLayer) {
				canMove = false;
				StartCoroutine(AudioController.GetClass.StopAudio (enemyPlayer));
				Physics2D.IgnoreLayerCollision (groundLayer, enemiesLayer, false);
				enemyAnims.enabled = false;
				enemyRigid.velocity = Vector2.zero;
				StartCoroutine (CanMoveCondition (hitWaitTime));
			} 

			if (col.gameObject.layer == groundLayer) {
				foreach (Collider2D batCol in enemyCollider) {
					Physics2D.IgnoreCollision (col.collider, batCol);
				}
			}
				
			if (col.gameObject.layer == playerLayer) {
				if (col.gameObject.CompareTag("Player")) {
					Hit (enemyHit);
					canMove = false;
				}
				if (col.collider == player.GroundCheckCollider) {					
					StartCoroutine (Die ());
				}

			}

		}

		//Detecta algo que entrou na área do colisor
		protected void OnTriggerEnter2D(Collider2D col){
			if (col.gameObject.CompareTag("Pickaxe")) {
				StartCoroutine(Damage (player.PickaxeDamage));			
			}
			if (col.gameObject.layer == LayerMask.NameToLayer ("Water")) {
				canMove = false;
				enemyRigid.AddRelativeForce (Vector2.up * moveForce * Time.deltaTime);
			}
		}

		//Move o inimigo com base na distancia entre ele e o player
		protected override void Move(float distance){			
			if (distance < distanceToMove) {
				if (canMove) {
					enemyAnims.enabled = true;
					StartCoroutine(AudioController.GetClass.StartAudio (enemyPlayer, enemyClips[0], 1f, true));
					if (transform.position.x < player.transform.position.x) {
						enemyRigid.AddRelativeForce (new Vector2 (moveForce * Time.deltaTime, -moveForce * Time.deltaTime));
					}
					if (transform.position.x > player.transform.position.x) {
						enemyRigid.AddRelativeForce (new Vector2 (-moveForce * Time.deltaTime, -moveForce * Time.deltaTime));
					}	
					if (transform.position.y < player.transform.position.y) {
						canMove = false;
					}
				} 
				else {		
					enemyRigid.AddRelativeForce (Vector2.up * moveForce * Time.deltaTime);
				}
			} 
			else {	
				enemyRigid.AddRelativeForce (Vector2.up * moveForce * Time.deltaTime);
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

		//Implemento: <<interface>>IEnemyRespawnable - Método que inicia o renascimento
		public void Respawn(float distance){
			if (!GeometryUtility.TestPlanesAABB(GameCamera.GetClass.GetPlanes, enemyCollider[0].bounds)) {
				if (distance > distanceToMove) {
					float respawnDistance = Vector2.Distance (player.transform.position, startPosition);
					if (respawnDistance > distanceToSpawn) {
						enemyPrefab = Resources.Load ("Prefabs/Enemies/Bat") as GameObject;
						Instantiate (enemyPrefab, startPosition, Quaternion.identity, GameObject.Find ("Enemies").transform);
						Destroy (gameObject);
					}
				}
			}

		}						

	}

}