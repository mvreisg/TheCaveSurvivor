using UnityEngine;
using System.Collections;
using TheCaveSurvivor.Controllers;
using TheCaveSurvivor.Characters;
using TheCaveSurvivor.Scene;

namespace TheCaveSurvivor.Enemy{

	/*
	 * Classe: Ghost 
	 * 
	 * 		Herda de : 	<<abstract>>
	 * 					Enemies
	 * 
	 * 	Classe que controla o inimigo fantasma
	 * 
	 * */

	public class Ghost : Enemies {

		public const 		float				gravityScale = 0f;

		//Chamado uma vez apenas
		protected override void Start () {	
			base.Start ();

			//Ignorar colisão com outros inimigos
			enemyAnims = GetComponent<Animator> ();
			enemySprite = GetComponent<SpriteRenderer> ();
			enemyPlayer = GetComponent<AudioSource> ();
			enemyRigid = GetComponent<Rigidbody2D> ();
			enemyCollider = new Collider2D[] {GetComponent<CircleCollider2D> ()};
			enemyClips = new AudioClip[] { Resources.Load<AudioClip> ("Audio/SFX/Enemies/ghost_sound") };
			startPosition = transform.position;

			//Atribuição de parametors
			healthPoints = 4;
			hitWaitTime = 0.5f;
			enemyHit = 1;

			distanceToMove = 2f;
			moveForce = 60f;
			moveSpeed = 1f;
			jumpForce = 0f;

			canMove = true;
			enemyDied = false;
			canTakeDamage = true;

			//Inicia o som do fantasma
			StartCoroutine(AudioController.GetClass.StartAudio(enemyPlayer, enemyClips[0], 1f, true));

		}

		//Atualiza a cada quadro
		protected override void Update(){
			if (!Stages.PausedGame) {
				Move (CalculateDistance ());
			}
		}

		//Detecta algo que entrou na área do colisor
		protected void OnTriggerEnter2D(Collider2D col){
			if (!enemyDied) {
				if (col.gameObject.CompareTag("Pickaxe")) {
					StartCoroutine(Damage (player.PickaxeDamage));
					StartCoroutine (CanMoveCondition (hitWaitTime));
				}			
			}
		}

		//Detecta algo que entrou e permaneceu na área do colisor
		protected void OnTriggerStay2D(Collider2D col){
			if (col.gameObject.CompareTag("Player")) {
				Hit (enemyHit);
			}

		}

		//Move o inimigo com base na distancia entre ele e o player
		protected override void Move(float distance){
			if (!enemyDied) {
				if (distance < distanceToMove) {
					if (canMove) {
						enemyAnims.SetBool ("willAttack", true);
						if (player.transform.position.x > transform.position.x) {
							transform.Translate (Vector2.right * Time.deltaTime);
						}
						if (player.transform.position.x < transform.position.x) {
							transform.Translate (Vector2.left * Time.deltaTime);
						}

						if (player.transform.position.y > transform.position.y) {
							transform.Translate (Vector2.up * Time.deltaTime);
						}
						if (player.transform.position.y < transform.position.y) {
							transform.Translate (Vector2.down * Time.deltaTime);
						}
					
					}
				} 
				else {
					enemyAnims.SetBool ("willAttack", false);
					enemyRigid.velocity = Vector2.zero;
				}
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
						StartCoroutine (CoroutinesController.GetClass.Blink_Sprite (enemySprite, Color.white, 0.5f, 0.8f));
					}
					yield return new WaitForSeconds(0.5f);
					canTakeDamage = true;
				}
			}
		}

		//Método que inicia a morte
		protected override IEnumerator Die(){
			if (!enemyDied) {
				enemyDied = true;
				enemyRigid.isKinematic = true;
				enemyCollider [0].enabled = false;
				StartCoroutine(Loot (transform.position));
				enemyAnims.SetTrigger("dieAnim");	
				yield return new WaitForSeconds(deathWaitTime);
				Destroy (gameObject);
			}
		}

		//Método que controla o loot dos inimigos
		protected override IEnumerator Loot(Vector3 posToSpawnLoot){
			float random = Random.Range (lootMinValue, lootMaxValue);
			if (random < 7f) {
				GameObject potion = Instantiate (potionDrop, posToSpawnLoot, Quaternion.identity, collectablesObject.transform) as GameObject;
				yield return new WaitForSecondsRealtime (lootMaxItemWait);
				Destroy (potion);
			}
			if (random < 2f) {
				GameObject lifes = Instantiate (lifeDrop, posToSpawnLoot, Quaternion.identity, collectablesObject.transform) as GameObject;
				yield return new WaitForSecondsRealtime (lootMaxItemWait);
				Destroy (lifes);
			}

		}

	}

}