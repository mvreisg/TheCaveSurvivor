using UnityEngine;
using System.Collections;
using TheCaveSurvivor.Characters;
using TheCaveSurvivor.Enemy;
using TheCaveSurvivor.Controllers;

namespace TheCaveSurvivor.Scenario{

	/*
	 * Classe: DropPlatform
	 * 
	 * 	Responsável por controlar as plataformas que caem
	 * 
	 * */

	public class DropPlatform : MonoBehaviour {

		private			Player 				player;
		private			SpriteRenderer		platformRenderer;
		private			Animator 			platformAnimator;
		private			Rigidbody2D 		platformRigid;
		private			BoxCollider2D		platformBoxCol;
		private			EdgeCollider2D		platformEdgeCol;
		private			AudioSource 		platformSource;
		public			AudioClip[]			platformClips;
		public			bool 				dontDrop;

		private			int					amountDamage = 1;
		private			int					groundLayer;
		private			float 				destroyDelay;

		//Chamado uma vez apenas
		private void Start () {			
			//Chama o método Start da classe Scenario
			player = GameObject.FindObjectOfType<Player> ();
			platformRenderer = GetComponent<SpriteRenderer>();
			platformAnimator = GetComponent<Animator> ();
			platformRigid = GetComponent<Rigidbody2D>();
			platformBoxCol = GetComponent<BoxCollider2D>();
			platformEdgeCol = GetComponent<EdgeCollider2D>();
			platformSource = GetComponent<AudioSource>();
			platformRigid.isKinematic = true;
			platformAnimator.enabled = false;
			platformClips = new AudioClip[]{ 
				Resources.Load<AudioClip> ("Audio/SFX/Animations/solid_break") 
			};
			groundLayer = LayerMask.NameToLayer ("Ground");
			destroyDelay = 1f;
		}

		//Detecta algo que colidiu com o objeto
		private void OnCollisionEnter2D(Collision2D col){	
			if (!platformRigid.isKinematic){
				if (col.gameObject.CompareTag ("TrapPlatform")) {		
					if (col.collider.attachedRigidbody != null) {
						col.collider.attachedRigidbody.isKinematic = false;
					}
					StartCoroutine(DestroyProcess(destroyDelay));
				} 
			}
		}

		//Detecta algo que colidiu com o objeto e permaneceu colidindo
		private void OnCollisionStay2D(Collision2D col){
			if (platformRigid.isKinematic && !dontDrop && player.OnGround && platformBoxCol.IsTouching(col.collider)) {				
				if (col.gameObject.CompareTag("Player")) {					
					StartCoroutine (DropObjectByTime (1f));
				}
			}
		}
			
		//Ativado para destruir o gameObject
		private IEnumerator DestroyProcess(float waitTime){
			StartCoroutine(AudioController.GetClass.StartAudio (platformSource, platformClips[0], 0.2f, false));
			platformAnimator.enabled = true;
			yield return new WaitForSeconds (platformAnimator.GetCurrentAnimatorStateInfo (0).length);
			Destroy (platformBoxCol);
			Destroy (platformEdgeCol);
			Destroy (platformRigid);
			Destroy (platformRenderer);
			Destroy (platformAnimator);
			yield return new WaitForSeconds (waitTime);
			StopAllCoroutines ();		
			Destroy (platformSource);
			Destroy (gameObject);
		}

		//Detecta algo que entrou na área de colisão do colisor deste objeto
		private void OnTriggerEnter2D(Collider2D col){		
			if (!platformRigid.isKinematic) {					
				if (col.gameObject.layer == groundLayer) {
					StartCoroutine (DestroyProcess (destroyDelay));
				} 
				else {
					if (col.gameObject.CompareTag ("Player")) {					
						StartCoroutine (col.gameObject.GetComponent<Player> ().Damage (amountDamage, transform));
						StartCoroutine (DestroyProcess (destroyDelay));
					} 
					else if (col.gameObject.CompareTag ("Slime")) {
						StartCoroutine (col.gameObject.GetComponent<Slime> ().Damage (amountDamage));
						StartCoroutine (DestroyProcess (destroyDelay));
					}
					else if (col.gameObject.CompareTag ("Bat")) {
						StartCoroutine (col.gameObject.GetComponent<Bat> ().Damage (amountDamage));
						StartCoroutine (DestroyProcess (destroyDelay));
					}
					else if (col.gameObject.CompareTag ("Zombie")) {
						StartCoroutine (col.gameObject.GetComponent<Zombie> ().Damage (amountDamage, transform));
						StartCoroutine (DestroyProcess (destroyDelay));
					}
				}
			}
		}
			
		//Coloca a plataforma sobre as leis da fisica(deixa cair) com base em um tempo
		private IEnumerator DropObjectByTime(float TimeToDrop){
			yield return new WaitForSeconds (TimeToDrop);
			if (platformRigid != null) {
				platformRigid.isKinematic = false;
			}
		}

	}

}
