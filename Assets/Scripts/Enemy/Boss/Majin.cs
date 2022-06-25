using UnityEngine;
using System.Collections;
using TheCaveSurvivor.Characters;
using TheCaveSurvivor.Controllers;
using TheCaveSurvivor.Scene;

namespace TheCaveSurvivor.Enemy{
	
	/*
	 * Classe: Majin 
	 * 
	 * 		Herda de : <<abstract>>
	 * 					 Enemies
	 * 
	 * 	Classe que controla o chefe final do jogo
	 * 
	 * */

	public class Majin : Enemies {

		//Declaração de variáveis
		private				float				waveLength,	
												waveFrenquency, 
												xOrientation, 
												elapsedTime, 
												maxElapsedTime,
												dropRockTimeInterval;

		private				Vector3				sineVector;

		private				GameObject 			CameraYLocker, 
												RightLimiter, 
												LeftLimiter, 
												TopToPosition, 
												EnemyAdjuster;

		private				bool 				enteredOnBossArea, 
												sineMovement, 
												attackMovement,
												preparingState, 
												movingTowardState, 
												realignState,
												flipToRight,
												flipToLeft;

		private				FinalStage 			finalStage;

		//Métodos nativos da Unity

		protected override void Start(){						
			base.Start ();

			enemyCollider = new Collider2D[]{ GetComponent<BoxCollider2D> (), GetComponent<CircleCollider2D> () };
			enemyClips = new AudioClip[] { 				
				Resources.Load<AudioClip> ("Audio/SFX/Enemies/majin_rise"),
				Resources.Load<AudioClip> ("Audio/SFX/Enemies/majin_noise"),
				Resources.Load<AudioClip> ("Audio/SFX/Enemies/majin_damage"),
				Resources.Load<AudioClip> ("Audio/SFX/Enemies/majin_die")
			}; 
			enemyRigid = GetComponent<Rigidbody2D> ();
			enemySprite = GetComponent<SpriteRenderer> ();
			enemyPlayer = GetComponent<AudioSource> ();
			enemyAnims = GetComponent<Animator> ();

			//Atribui parametros do inimigo
			xOrientation = -1;
			moveForce = 2f;
			moveSpeed = 1.15f;
			waveLength = 3f;
			waveFrenquency = 1f;
			distanceToMove = 20f;
			healthPoints = 10;
			enemyHit = 1;
			elapsedTime = 0f;
			maxElapsedTime = 8f;
			dropRockTimeInterval = 2f;
			sineMovement = false;
			attackMovement = false;
			canTakeDamage = false;
			canMove = false;
			enemyDied = false;
			preparingState = false;
			movingTowardState = false;
			realignState = false;
			flipToRight = false;
			flipToLeft = true;

			//Captura os objetos essenciais do cenário
			CameraYLocker = GameObject.Find ("CameraYLocker");
			RightLimiter = GameObject.Find ("RightLimiter");
			LeftLimiter = GameObject.Find ("LeftLimiter");
			TopToPosition = GameObject.Find ("TopToPosition");
			EnemyAdjuster = GameObject.Find ("EnemyAdjuster");

			finalStage = GameObject.FindObjectOfType<FinalStage> ();

		}
			
		private void OnDrawGizmos(){
			CameraYLocker = GameObject.Find ("CameraYLocker");
			TopToPosition = GameObject.Find ("TopToPosition");
			EnemyAdjuster = GameObject.Find ("EnemyAdjuster");
			Gizmos.DrawWireCube (TopToPosition.transform.position, new Vector2 (0.5f, 0.5f));
			Gizmos.DrawWireCube (CameraYLocker.transform.position, new Vector2 (9.5f, 7.5f));
			Gizmos.DrawWireCube (EnemyAdjuster.transform.position, new Vector2 (0.1f, 0.1f));
		}

		protected override void Update(){			
			enteredOnBossArea = Physics2D.OverlapBox (CameraYLocker.transform.position, new Vector2 (9.5f, 7.5f), 0f, 1 << playerLayer);
			//SE O PLAYER ENTROU NA BOX E A CAMERA NÃO TRAVOU
			if (enteredOnBossArea && !GameCamera.GetClass.LockCameraY) {
				GameCamera.GetClass.LockYAxis ();	
				StartCoroutine (CanMoveCondition (0.5f, true, false));
				StartCoroutine (DropRock (dropRockTimeInterval));
				StartCoroutine(AudioController.GetClass.StartAudio (finalStage.ScenePlayer, finalStage.SceneClips[1], 0.75f, false));
				StartCoroutine(AudioController.GetClass.StartAudio (enemyPlayer, enemyClips[0], 0.75f, false));
			}
			//SE O PLAYER ENTROU NA BOX E A CAMERA TRAVOU
			else if (enteredOnBossArea && GameCamera.GetClass.LockCameraY) {

				//AJUSTA O MOVIMENTO DO BOSS
				if (transform.position.x < LeftLimiter.transform.position.x && !flipToRight) {
					enemyAnims.SetTrigger ("toFlipRight");
					enemySprite.flipX = true;
					xOrientation = 1;
					flipToRight = true;
					flipToLeft = false;
				}
				if (transform.position.x > RightLimiter.transform.position.x && !flipToLeft) {
					enemyAnims.SetTrigger ("toFlipLeft");
					enemySprite.flipX = false;
					xOrientation = -1;
					flipToRight = false;
					flipToLeft = true;
				}

				GameCamera.GetClass.AlignCameraToPoint (CameraYLocker.transform.position);
				Move (CalculateDistance ());

			}
			//SE O PLAYER SAIU DA BOX E A CAMERA ESTÁ TRAVADA
			else if (!enteredOnBossArea && GameCamera.GetClass.LockCameraY) {
				//StartCoroutine(AudioController.GetClass.StartAudio (finalStage.ScenePlayer, finalStage.SceneClips[0], 0.75f, false));
				GameCamera.GetClass.UnlockCamera ();
			}
		}

		protected void OnTriggerEnter2D(Collider2D col){
			if (col.gameObject.CompareTag ("Pickaxe") && canTakeDamage) {
				StartCoroutine(Damage (player.PickaxeDamage, transform));
				if (attackMovement) {
					preparingState = true;
					movingTowardState = true;
					canTakeDamage = false;
					realignState = false;
				}
			}
		}

		protected void OnCollisionEnter2D(Collision2D col){
			if (col.gameObject.CompareTag ("Player") && !canTakeDamage) {				
				Hit (enemyHit);
				if (attackMovement) {
					preparingState = true;
					movingTowardState = true;
					realignState = false;
				}
			}
			else if (col.gameObject.CompareTag ("Player") && canTakeDamage) {				
				Hit (enemyHit);
			}

			if (col.gameObject.layer == groundLayer && !movingTowardState && attackMovement) {
				preparingState = true;
				movingTowardState = true;
				realignState = false;
			}
		}			

		// Implementações e heranças

		//Responsável por permitir a movimentação
		protected override IEnumerator CanMoveCondition(float timeToWait){ 
			return base.CanMoveCondition (timeToWait);
		}			

		protected IEnumerator CanMoveCondition(float timeToWait, bool sineMove, bool attackMove){			
			canMove = false;
			yield return new WaitForSecondsRealtime (timeToWait);
			canMove = true;
			sineMovement = sineMove;
			attackMovement = attackMove;
		}

		//Responsável pela movimentação
		protected override void Move(float distance){			
			if (canMove) {
				if (sineMovement) {					
					enemyAnims.SetBool ("isAttack", false);
					if (!enemyPlayer.isPlaying) {
						StartCoroutine(AudioController.GetClass.StartAudio (enemyPlayer, enemyClips[1], 0.2f, false, 1f));
					}
					if (distance < distanceToMove) {						
						elapsedTime += Time.deltaTime;
						SineMovement ();
						if (elapsedTime >= maxElapsedTime) {
							elapsedTime = 0f;
							StartCoroutine(CanMoveCondition (1f, false, true));
						}
					}
					else {
						StartCoroutine(CanMoveCondition (1f, true, false));
					}
				}
				if (attackMovement) {
					enemyAnims.SetBool ("isAttack", true);
					if (distance < distanceToMove) {						
						StartCoroutine (AttackMovement ());
					} 
					else {
						StartCoroutine(CanMoveCondition (1f, true, false));
					}
				}
			}
		}

		//Responsável por dar dano no jogador
		protected override void Hit(int damage){
			StartCoroutine (player.Damage (damage, transform));
		}

		//Responsável por receber o dano
		public override IEnumerator Damage(int damage){		
			if (canTakeDamage) {	
				canTakeDamage = false;
				healthPoints -= damage;
				if (healthPoints <= 0) {
					StartCoroutine (Die ());
				}
				else {
					StartCoroutine(AudioController.GetClass.StartAudio (enemyPlayer, enemyClips[2], 0.65f, false));
					StartCoroutine (CoroutinesController.GetClass.Blink_Sprite (enemySprite, Color.red, 0.5f, 0.5f));
				}
			}
			yield return new WaitForSeconds (0.5f);
			moveSpeed += 0.05f;
			dropRockTimeInterval -= 0.025f;
			canTakeDamage = true;
		}	

		//Responsável por receber o dano c/ recuoe
		public IEnumerator Damage(int damage, Transform transf){
			if (!enemyDied) {
				if (canTakeDamage) {
					canTakeDamage = false;
					healthPoints -= damage;
					if (healthPoints <= 0) {
						StartCoroutine (Die ());
					} 
					else {
						StartCoroutine(AudioController.GetClass.StartAudio (enemyPlayer, enemyClips[2], 0.65f, false));
						StartCoroutine (CoroutinesController.GetClass.Blink_Sprite (enemySprite, Color.red, 0.5f, 0.5f));
						if (transf != null) {
							float multiplier = (transf.position.x < transform.position.x) ? (1) : (-1);
							enemyRigid.velocity = new Vector2 (moveForce * Time.deltaTime, moveForce * Time.deltaTime);
						}
						yield return new WaitForSeconds(0.5f);
						moveSpeed += 0.05f;
						dropRockTimeInterval -= 0.025f;
						canTakeDamage = true;
					}
				}					
			}
		}

		protected override IEnumerator Die(){
			if (!enemyDied) {
				StartCoroutine(AudioController.GetClass.StartAudio (enemyPlayer, enemyClips[3], 1f, false));
				enemyDied = true;
				StartCoroutine (Loot (transform.position));
				enemyAnims.SetTrigger ("dieAnim");
				yield return new WaitForSeconds (enemyAnims.GetCurrentAnimatorStateInfo (0).length);
				Destroy (gameObject);
				StartCoroutine(AudioController.GetClass.StartAudio (finalStage.ScenePlayer, finalStage.SceneClips[2], 0.25f, false));
			}
		}

		//Realiza ua instância na morte do inimigo
		protected override IEnumerator Loot(Vector3 posToSpawnLoot){
			float random = Random.Range (lootMinValue, lootMaxValue);
			if (enemyDied) {
				GameObject dropKey = Instantiate (keyDrop, posToSpawnLoot, Quaternion.identity, collectablesObject.transform) as GameObject;
				yield return null;
			}
		}


		// Métodos específicos da classe Majin

		private IEnumerator DropRock(float timeInterval){
			yield return new WaitForSeconds (timeInterval);
			GameObject rockInstance = ((GameObject)Instantiate(Resources.Load<GameObject>("Prefabs/ScenarioParts/rocks"), transform.position, Quaternion.identity, transform.parent));
			rockInstance.GetComponent<Animator> ().enabled = false;
			rockInstance.name = "RockInstance" + GetInstanceID ().ToString ();
			yield return new WaitUntil (() => sineMovement);
			StartCoroutine (DropRock (timeInterval));
		}

		//Método responsável por fazer uma movimentação senoidal
		private void SineMovement(){
			sineVector.x = moveForce * moveSpeed * xOrientation;
			sineVector.y = Mathf.Sin (2 * Mathf.PI * Time.time * waveFrenquency) * waveLength * moveSpeed;
			sineVector.z = transform.position.z;
			transform.position += sineVector * Time.deltaTime;
		}

		//Método responsável de gerenciar o processo de ataque do chefe
		private IEnumerator AttackMovement(){

			//PREPARA PARA A INVESTIDA
			if (!preparingState) {
				transform.position = Vector2.LerpUnclamped (transform.position, TopToPosition.transform.position, moveSpeed * Time.deltaTime);	
				Collider2D[] TopBox = Physics2D.OverlapBoxAll (TopToPosition.transform.position, new Vector2 (0.5f, 0.5f), 0f);
				foreach (Collider2D collidingCol in TopBox){					
					foreach (Collider2D enemyCol in enemyCollider) {						
						if (collidingCol == enemyCol) {
							preparingState = true;
						}
					}
				}
				yield return null;
			}

			//PERSEGUE O PLAYER
			else if (!movingTowardState) {
				canTakeDamage = true;
				enemySprite.color = Color.green;
				transform.position = Vector2.LerpUnclamped (transform.position, player.transform.position, moveForce * moveSpeed * Time.deltaTime);
				if (transform.position.x <= player.transform.position.x) {
					enemyAnims.SetTrigger ("toFlipLeft");
					enemySprite.flipX = true;
				}
				else {
					enemyAnims.SetTrigger ("toFlipLeft");
					enemySprite.flipX = false;
				}
				StartCoroutine(AudioController.GetClass.StartAudio (enemyPlayer, enemyClips[1], 0.75f, false));
				yield return null;
			}

			//REALINHA NO EIXO Y
			else if (!realignState) {
				canTakeDamage = false;
				enemySprite.color = Color.white;
				transform.position = Vector2.LerpUnclamped (transform.position, EnemyAdjuster.transform.position, moveSpeed * Time.deltaTime);
				Collider2D[] AlignBox = Physics2D.OverlapBoxAll (EnemyAdjuster.transform.position, new Vector2 (0.1f, 0.1f), 0f);
				foreach (Collider2D collidingCol in AlignBox){					
					foreach (Collider2D enemyCol in enemyCollider) {						
						if (collidingCol == enemyCol) {
							realignState = true;
						}
					}
				}
				yield return null;

			}

			//INICIA O MOVIMENTO SENOIDAL
			else{
				StartCoroutine (CanMoveCondition (0f, true, false));
				preparingState = false;
				movingTowardState = false;
				realignState = false;
				yield return null;
			}

		}

	}

}
