using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TheCaveSurvivor.Controllers;
using TheCaveSurvivor.Scenario;
using TheCaveSurvivor.Scene;

namespace TheCaveSurvivor.Characters{

	/*
	 * Classe: Player
	 * 
	 * 	Controladora do personagem principal
	 * 
	 * */

	public class Player : MonoBehaviour {

		//Declaração de variáveis de escopo global na classe Player
		private			GameObject 				playerGround, playerLantern, playerPickaxe;
		private			Rigidbody2D 			playerRigid;
		private			Collider2D[] 			playerColliders;
		private			BoxCollider2D 			groundcheckCollider;
		private			Animator 				playerAnims;
		private			SpriteRenderer			playerSprite;
		private			AudioSource[]			playerSounds;

		private			AudioClip 				Jump_Clip, 
												Hurt_Clip, 
												Death_Clip,											
												PickItem_Clip,
												PickPickaxe_Clip,
												PickKey_Clip,												
												Recover_Clip,
												FullRecover_Clip,
												PickaxeUse_Clip;		
																								

		public 	 const	float 					playerGravity = 2.5f;

		private 		float 					moveForce = 2f,
												jumpForce = 800f,											
												deathWaitTime = 2f;

		private			int 					playerHearts = 3, 
												playerLifes = 4,
												keysQuantity = 0,
												pickaxeDamage = 2,
												playerLayer, 
												groundLayer, 
												waterLayer, 
												scalableLayer, 
												itemsLayer;

		public			bool					onGround, 
												onScalable, 
												onWater,
												playerPickedPickaxe = false,
												playerDied = false, 
												canBeHitted = true;

		private			Vector2 				respawnPosition;
		private			bool					playerInvincible;

		//Propriedades de acesso externo
		public bool PlayerInvincible{
			get{
				return this.playerInvincible;
			}
		}

		public int PlayerHearts {
			get{ 
				return this.playerHearts; 
			} 
		}	
		public int PlayerLifes {
			get{ 
				return this.playerLifes; 
			} 
		}		
		public int KeysQuantity{	
			get{ 
				return this.keysQuantity; 
			}
			set{ 
				this.keysQuantity = value; 
				if (this.keysQuantity < 0) {
					keysQuantity = 0;
				}
				HUDManager.GetClass.KeysManager (); 
			} 
		}
		public bool PickedPickaxe{	
			get{ 
				return this.playerPickedPickaxe; 
			}
		}
		public int PickaxeDamage{	
			get{ 
				return this.pickaxeDamage; 
			}
		}
		public Vector2 RespawnPosition{	
			get{ 
				return this.respawnPosition; 
			}
			set{ 
				this.respawnPosition = value; 
			}
		}
		public Rigidbody2D PlayerRigid {
			get{ 
				return this.playerRigid; 
			} 
		}
		public Collider2D[] PlayerColliders {
			get{ 
				return this.playerColliders; 
			} 
		}
		public BoxCollider2D GroundCheckCollider {
			get{ 
				return this.groundcheckCollider;
			} 
		}
		public bool OnGround {
			get{
				return this.onGround;
			} 
		}
		public bool OnScalable {
			get{
				return this.onScalable;
			} 
			set{
				this.onScalable = value;
			}
		}
		public bool OnWater {
			get{
				return this.onWater;
			} 
		}
			
		public AudioSource GetPlayerAudioSource{
			get{
				return this.playerSounds [0];
			}
		}

		public AudioSource GetPlayerAmbienceSource{
			get{
				return this.playerSounds [1];
			}
		}

		//Chamado apenas uma vez
		private void Start () {					
			//Recupera os dados do jogador ao reiniciar a cena
			if (GameData.gameData.StoragedData) {
				playerLifes = GameData.gameData.PlayerLifes;
				respawnPosition = GameData.gameData.RespawnPosition;
				playerPickedPickaxe = GameData.gameData.PlayerPickedPickaxe;
				transform.position = respawnPosition;
			}
			else {
				switch (SceneManager.GetActiveScene ().buildIndex) {
				case (int)ScenesIndexes.firstCourse:					
					respawnPosition = GameObject.FindObjectOfType<FirstStage> ().GetDefaultSpawnPosition;
					transform.position = respawnPosition;
					break;
				case (int)ScenesIndexes.secondCourse:
					respawnPosition = GameObject.FindObjectOfType<SecondStage> ().GetDefaultSpawnPosition;
					transform.position = respawnPosition;
					break;
				case (int)ScenesIndexes.finalCourse:
					respawnPosition = GameObject.FindObjectOfType<FinalStage> ().GetDefaultSpawnPosition;
					transform.position = respawnPosition;
					break;
				}

			}

			//Coloca a picareta de volta na sua mão
			if (playerPickedPickaxe) {
				playerPickedPickaxe = true;
				GameObject pickaxe = ((GameObject)Instantiate (Resources.Load<GameObject> ("Prefabs/Items/Pickaxe"), gameObject.transform));
				playerPickaxe = pickaxe;
				if (playerPickaxe != null) {
					print (true);
					playerPickaxe.layer = playerLayer;
					playerPickaxe.transform.SetParent (gameObject.transform);
					Destroy (playerPickaxe.GetComponent<Rigidbody2D> ());
					Destroy (playerPickaxe.GetComponent<CircleCollider2D> ());
					foreach (GameObject obj in GameObject.FindGameObjectsWithTag ("Pickaxe")) {
						if (transform.root.gameObject.name != obj.transform.root.gameObject.name) {
							Destroy (obj);
						}
					}
					playerPickaxe.transform.localPosition = new Vector3 (-0.075f, 0f, -0.1f);
					playerPickaxe.transform.localRotation = Quaternion.identity;
					playerPickaxe.transform.localScale = new Vector3 (0.4f, 0.4f, 1f);					
				}
			}

			//Declaração de layers
			groundLayer = LayerMask.NameToLayer("Ground");
			waterLayer = LayerMask.NameToLayer("Water");
			scalableLayer = LayerMask.NameToLayer("Scalable");
			playerLayer = LayerMask.NameToLayer("Player");
			itemsLayer = LayerMask.NameToLayer("Items");

			//Declaração de lanterna e detector de chão
			playerLantern = transform.FindChild ("PlayerLantern").gameObject;
			playerGround = transform.FindChild ("PlayerGround").gameObject;

			//Captura de componentes
			playerAnims = GetComponent<Animator> ();
			playerRigid = GetComponent<Rigidbody2D> ();
			playerSprite = GetComponent<SpriteRenderer> ();
			playerSounds = new AudioSource[2];
			playerSounds = GetComponents<AudioSource>();
			playerColliders = new Collider2D[] { GetComponent<BoxCollider2D> (), GetComponent<CircleCollider2D> () };
			groundcheckCollider = playerGround.GetComponent<BoxCollider2D> ();

			//Atualização do HUD
			HUDManager.GetClass.HeartsManager ();
			HUDManager.GetClass.LifesManager ();
			HUDManager.GetClass.KeysManager ();

			//Atribuição dos arquivos de audio
			Jump_Clip = Resources.Load<AudioClip> ("Audio/SFX/Player/player_jump");
			Hurt_Clip = Resources.Load<AudioClip> ("Audio/SFX/Player/player_hurt");
			Death_Clip = Resources.Load<AudioClip> ("Audio/SFX/Player/player_death");
			Recover_Clip = Resources.Load<AudioClip> ("Audio/SFX/Player/player_recover");
			FullRecover_Clip = Resources.Load<AudioClip> ("Audio/SFX/Player/player_fullrecover");
			PickItem_Clip = Resources.Load<AudioClip> ("Audio/SFX/Items/item_pick");
			PickPickaxe_Clip = Resources.Load<AudioClip> ("Audio/SFX/Items/item_pickaxe_active");
			PickaxeUse_Clip = Resources.Load<AudioClip> ("Audio/SFX/Items/item_pickaxe_use");
			PickKey_Clip = Resources.Load<AudioClip> ("Audio/SFX/Items/item_key");

		}

		//Desenha a parte visual dos OverlapBoxes
		private void OnDrawGizmos(){			
			Gizmos.DrawWireCube (transform.position, new Vector2(0.1f, 0.1f));
			Gizmos.DrawWireCube (GameObject.Find("PlayerGround").transform.position, new Vector2(0.135f, 0.015f));
		}

		//Atualiza a cada quadro
		private void Update () {			
			if (!playerDied) {

				//Detectores de chão e água	
				onGround = Physics2D.OverlapBox (playerGround.transform.position, new Vector2(0.135f, 0.015f), 0f, 1 << groundLayer);			
				onWater = Physics2D.OverlapBox (transform.position, new Vector2(0.1f, 0.1f), 0f, 1 << waterLayer);

				if (!Stages.LockCommands) {
					//Chamada dos métodos de movimentos
					Walk (Input.GetButton ("Horizontal"));
					Jump (Input.GetButtonDown ("Jump"));
					Climb (Input.GetButton ("Vertical"));
					StartCoroutine(Hit (Input.GetKeyDown (KeyCode.C)));
				}

				if (!onWater && playerRigid.gravityScale < playerGravity) {
					playerRigid.gravityScale = playerGravity;
				}
					
				if (!onWater && playerSounds [1].isPlaying) {
					StartCoroutine(AudioController.GetClass.StopAudio(playerSounds [1]));
				}

			}

		}

		//Método andar
		private void Walk(bool input){
			if (input) {
				if (Input.GetAxis ("Horizontal") > 0) {							
					if (onGround && onWater) {
						transform.Translate (Vector2.right * moveForce * Time.deltaTime * Water.fluidDensity);
						playerAnims.SetBool("isRunning", true);
					} 
					else if (onWater) {						
						transform.Translate (Vector2.right * moveForce * Time.deltaTime * Water.fluidDensity);
						playerAnims.SetBool("inWater", true);
					}
					else if (onGround) {
						transform.Translate (Vector2.right * moveForce * Time.deltaTime);
						playerAnims.SetBool("isRunning", true);
					} 
					else {
						transform.Translate (Vector2.right * moveForce * Time.deltaTime);
						playerAnims.SetBool("inWater", false);
					}
					transform.rotation = Quaternion.Euler(new Vector3 (0f, 0f, 0f));
					transform.localScale = new Vector3 (1.4f, 1.4f, 1f);
					playerLantern.transform.eulerAngles = new Vector2 (0, 45);

					if (playerPickedPickaxe) {
						if (playerPickaxe != null) {
							playerPickaxe.transform.localPosition = new Vector3 (-0.075f, 0f, -0.1f);
							playerPickaxe.transform.localRotation = Quaternion.identity;
							playerPickaxe.transform.localScale = new Vector3 (0.4f, 0.4f, 1f);
						}
					}
				} 
				else if (Input.GetAxis ("Horizontal") < 0) {		
					if (onGround && onWater) {
						transform.Translate (Vector2.right * moveForce * Time.deltaTime * Water.fluidDensity);
						playerAnims.SetBool("isRunning", true);
					} 
					else if (onWater) {						
						transform.Translate (Vector2.right * moveForce * Time.deltaTime * Water.fluidDensity);
						playerAnims.SetBool("inWater", true);
					}
					else if (onGround) {
						transform.Translate (Vector2.right * moveForce * Time.deltaTime);
						playerAnims.SetBool("isRunning", true);
					} 
					else {
						transform.Translate (Vector2.right * moveForce * Time.deltaTime);
						playerAnims.SetBool("inWater", false);
					}
					transform.rotation = Quaternion.Euler(new Vector3 (0f, -180f, 0f));
					transform.localScale = new Vector3 (1.4f, 1.4f, -1f);
					playerLantern.transform.eulerAngles = new Vector2 (0, -45);

					if (playerPickedPickaxe) {
						if (playerPickaxe != null) {
							playerPickaxe.transform.localPosition = new Vector3 (-0.075f, 0f, -0.1f);
							playerPickaxe.transform.localRotation = Quaternion.identity;
							playerPickaxe.transform.localScale = new Vector3 (0.4f, 0.4f, 1f);
						}
					}
				}
				else {
					transform.Translate (Vector2.zero);
					playerAnims.SetBool("isRunning", false);
					playerAnims.SetBool("inWater", false);
				}
			} 
			else {
				transform.Translate (Vector2.zero);
				playerAnims.SetBool("isRunning", false);
				playerAnims.SetBool("inWater", false);
			}
		}

		//Método pular
		private void Jump(bool input){
			if (input) {
				if (onGround && onWater) {
					playerRigid.velocity = new Vector2 (0, jumpForce * Time.fixedDeltaTime * Water.fluidDensity);
				}
				else if (onWater) {
					playerRigid.velocity = new Vector2 (0, jumpForce * Time.fixedDeltaTime * Water.fluidDensity);
				} 
				else if (onGround) {
					playerRigid.velocity = new Vector2 (0, jumpForce * Time.fixedDeltaTime);
					StartCoroutine(AudioController.GetClass.StartAudio (playerSounds[0], Jump_Clip, 0.5f, false));
				}				
			}

		}

		//Método escalar
		private void Climb(bool input){
			if (input) {				
				float directionValue = Input.GetAxisRaw ("Vertical");
				if (onScalable) {					
					playerRigid.isKinematic = true;
					playerLantern.transform.eulerAngles = new Vector2 (-35, 0);
					transform.Translate (new Vector2 (0, directionValue * Time.deltaTime));				
					playerAnims.enabled = true;
					playerAnims.SetBool("inStair", true);
					Physics2D.IgnoreLayerCollision (playerLayer, groundLayer, !onGround && onScalable);
				} 
				else if (onGround) {
					playerAnims.enabled = true;
					playerAnims.SetBool("inStair", false);
					playerRigid.isKinematic = false;
					Physics2D.IgnoreLayerCollision (playerLayer, groundLayer, false);
				} 
				else {
					playerAnims.enabled = true;
					playerAnims.SetBool("inStair", false);
					playerRigid.isKinematic = false;
					Physics2D.IgnoreLayerCollision (playerLayer, groundLayer, false);
				}	
			} 
			else {
				if (onScalable && !onGround && playerRigid.isKinematic) {
					playerAnims.enabled = false;
					playerRigid.isKinematic = true;
				} 
				else {
					playerRigid.isKinematic = false;
					playerAnims.enabled = true;
					playerAnims.SetBool("inStair", false);
					Physics2D.IgnoreLayerCollision (playerLayer, groundLayer, false);
				}
			}
		}

		//Método de ataque c/ picareta
		private IEnumerator Hit(bool input){
			if (!playerDied) {
				if (input) {			
					if (playerPickedPickaxe) {						
						if (playerPickaxe != null) {									
							Animator pickaxeAnimator = playerPickaxe.GetComponent<Animator> ();
							AudioSource pickaxeSource = playerPickaxe.GetComponent<AudioSource> ();
							EdgeCollider2D pickaxeEdgeCol = playerPickaxe.GetComponent<EdgeCollider2D> ();
							if (!pickaxeAnimator.enabled) {
								pickaxeAnimator.enabled = true;
								pickaxeAnimator.ResetTrigger ("startAttack");
								pickaxeAnimator.SetTrigger("startAttack");
								StartCoroutine(AudioController.GetClass.StartAudio (pickaxeSource, PickaxeUse_Clip, 1f, false));		
								yield return new WaitForSeconds (pickaxeAnimator.GetCurrentAnimatorStateInfo (0).length);
								pickaxeAnimator.enabled = false;
								pickaxeEdgeCol.enabled = false;
							}
						}
					}
				}
			}
		}

		//Método dano
		public IEnumerator Damage(int damage){
			if (!playerInvincible) {
				if (!playerDied) {
					if (canBeHitted) {
						canBeHitted = false;
						playerHearts -= damage;
						HUDManager.GetClass.HeartsManager ();
						if (playerHearts <= 0) {
							StartCoroutine (Die ());
						}
						else {
							StartCoroutine(AudioController.GetClass.StartAudio (playerSounds[0], Hurt_Clip, 0.5f, false));
							StartCoroutine (CoroutinesController.GetClass.Blink_Sprite (playerSprite, Color.red, 0.6f, 0.5f));
							yield return new WaitForSeconds (0.6f);
							canBeHitted = true;
						}
					}
				}
			}
		}

		//Método dano c/ recuo
		public IEnumerator Damage(int damage, Transform transf){
			if (!playerInvincible) {
				if (!playerDied) {
					if (canBeHitted) {
						canBeHitted = false;
						playerHearts -= damage;
						HUDManager.GetClass.HeartsManager ();
						if (playerHearts <= 0) {
							StartCoroutine (Die ());
						}
						else {
							StartCoroutine(AudioController.GetClass.StartAudio (playerSounds[0], Hurt_Clip, 0.5f, false));
							StartCoroutine (CoroutinesController.GetClass.Blink_Sprite (playerSprite, Color.red, 0.6f, 0.5f));
							if (transf != null) {
								float multiplier = (transf.position.x < transform.position.x) ? (1) : (-1);
								playerRigid.velocity = new Vector2 (moveForce * multiplier * Time.deltaTime * 100, moveForce * Time.deltaTime * 100);
							}
							yield return new WaitForSeconds (0.6f);
							canBeHitted = true;
						}
					}
				}
			}
		}

		//Método recuperar
		public void Recover(int recover){
			if (!playerDied) {
				playerHearts += recover;
				if (playerHearts > 4) {
					playerHearts = 4;
				}
				HUDManager.GetClass.HeartsManager ();
			}
		}

		//Método morrer
		public IEnumerator Die(){
			if (!playerDied) {
				playerDied = true;
				playerAnims.SetTrigger("deathTrigger");
				StartCoroutine(AudioController.GetClass.StartAudio (playerSounds[0], Death_Clip, 0.7f, false));
				playerColliders [0].enabled = false;
				playerColliders [1].enabled = false;
				yield return new WaitForSeconds (deathWaitTime);

				HUDManager.GetClass.HeartsManager ();
				HUDManager.GetClass.LifesManager ();
				Respawn ();
			}

		}

		//Método renascer
		private void Respawn(){
			if (playerLifes > 0) {
				playerLifes--; 
				HUDManager.GetClass.HeartsManager ();
				HUDManager.GetClass.LifesManager ();

				GameData.gameData.PlayerLifes = playerLifes;
				GameData.gameData.RespawnPosition = respawnPosition;
				GameData.gameData.PlayerPickedPickaxe = playerPickedPickaxe;
				GameData.gameData.StoragedData = true;

				Scenes.ReloadScene ();
				playerColliders [0].enabled = true;
				playerColliders [1].enabled = true;
				transform.position = GameData.gameData.RespawnPosition;
			} 
			else {								
				Scenes.GoToScene ((int)ScenesIndexes.gameOver);
			}
		}			

		//Detector de entrada em colisão
		private void OnCollisionEnter2D(Collision2D col){
			if (col.gameObject.layer == itemsLayer) {
				if (col.gameObject.CompareTag ("Potion")) {
					StartCoroutine(AudioController.GetClass.StartAudio (playerSounds[0], FullRecover_Clip, 0.5f, false));
					Recover (4);
					Destroy (col.gameObject);
				}
				if (col.gameObject.CompareTag ("Heart")) {
					StartCoroutine(AudioController.GetClass.StartAudio (playerSounds[0], Recover_Clip, 0.5f, false));
					Recover (1);
					Destroy (col.gameObject);
				}
				if (col.gameObject.CompareTag ("Life")) {
					playerLifes++;
					StartCoroutine(AudioController.GetClass.StartAudio (playerSounds[0], PickItem_Clip, 0.5f, false));
					HUDManager.GetClass.LifesManager ();
					Destroy (col.gameObject);
				}
				if (col.gameObject.CompareTag ("Key")) {
					StartCoroutine(AudioController.GetClass.StartAudio (playerSounds[0], PickKey_Clip, 0.5f, false));
					keysQuantity++;
					HUDManager.GetClass.KeysManager ();
					Destroy (col.gameObject);
				}

				if (col.gameObject.CompareTag ("Pickaxe") && !playerPickedPickaxe) {					
					if (!playerPickedPickaxe) {
						StartCoroutine(AudioController.GetClass.StartAudio (playerSounds[0], PickPickaxe_Clip, 0.5f, false));
						playerPickedPickaxe = true;
						playerPickaxe = col.gameObject;
						if (playerPickaxe != null) {
							playerPickaxe.layer = playerLayer;
							playerPickaxe.transform.SetParent (transform);
							Destroy (playerPickaxe.GetComponent<Rigidbody2D> ());
							Destroy (playerPickaxe.GetComponent<CircleCollider2D> ());
						}
					}
				}
				else {
					Destroy (col.gameObject);
				}

			}

		}
			
	}

}