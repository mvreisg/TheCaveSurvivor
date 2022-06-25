using UnityEngine;
using System.Collections;
using TheCaveSurvivor.Characters;
using TheCaveSurvivor.Enemy;
using TheCaveSurvivor.Controllers;

namespace TheCaveSurvivor.Scenario{

	/*
	 * Classe: DropRock
	 * 	
	 * 	Herda de: <<abstract>>
	 * 				Scenario
	 * 
	 * 	Responsável por controlar as pedras que caem
	 * 
	 * */

	public class DropRock : MonoBehaviour {
		
		private			Player 				player;
		private			SpriteRenderer		dropRenderer;
		private			Rigidbody2D 		dropRigid;
		private			Collider2D			dropCol;
		private			AudioSource 		dropSource;
		public			AudioClip[]			dropClips;
		private			Animator 			dropAnimator;
		private			Vector3				startPosition;
		public			bool 				dontDropOnStart, autoRespawn;

		private			int					amountDamage = 1;
		private			int 				roofLayer, groundLayer;
		private			float 				destroyDelay;

		//Chamado uma vez apenas
		private void Start () {
			//Chama o método Start da class Scenario
			player = GameObject.FindObjectOfType<Player> ();	
			dropRenderer = GetComponent<SpriteRenderer>();
			dropRigid = GetComponent<Rigidbody2D>();
			dropCol = GetComponent<Collider2D>();
			dropSource = GetComponent<AudioSource>();
			dropAnimator = GetComponent<Animator> ();
			dropAnimator.enabled = false;
			dropRigid.isKinematic = (dontDropOnStart) ? true : false;
			dropClips = new AudioClip[]{ 
				Resources.Load<AudioClip> ("Audio/SFX/Animations/solid_break") 
			};
			roofLayer = LayerMask.NameToLayer ("Rooftop");
			groundLayer = LayerMask.NameToLayer ("Ground");
			startPosition = transform.position;
			destroyDelay = 1f;
		}

		//Chamado a cada quadro
		private void Update(){
			if (playerXYdistance(transform).x < 0.3f) {
				DropObject ();
			}
		}

		//Método que calcula a distância entre o objeto e o player no eixo X e Y
		private Vector2 playerXYdistance(Transform transf){
			Vector2 playerDistance = new Vector2 (
				Mathf.Abs(player.transform.position.x - transf.position.x),
				Mathf.Abs(player.transform.position.y - transf.position.y)
			);
			return playerDistance;
		}

		//Detecta algo que colidiu com o objeto
		private void OnCollisionEnter2D(Collision2D col){
			if (col.gameObject.layer != roofLayer) {
				if (col.gameObject.CompareTag ("Player")) {
					StartCoroutine (col.gameObject.GetComponent<Player> ().Damage (amountDamage, transform));
					StartCoroutine (DestroyProcess (destroyDelay));
				}
				else if (col.gameObject.CompareTag ("Slime")) {
					StartCoroutine (col.gameObject.GetComponent<Slime> ().Damage (amountDamage));
					StartCoroutine (DestroyProcess (destroyDelay));
				}
				else if (col.gameObject.CompareTag ("Bat")) {
					Physics2D.IgnoreCollision (dropCol, col.collider);
					StartCoroutine (col.gameObject.GetComponent<Bat> ().Damage (amountDamage));
				}
				else if (col.gameObject.CompareTag ("Zombie")) {
					StartCoroutine (col.gameObject.GetComponent<Zombie> ().Damage (amountDamage, transform));				
					StartCoroutine (DestroyProcess (destroyDelay));
				}
				else if (col.gameObject.CompareTag ("Majin")) {
					Physics2D.IgnoreCollision (dropCol, col.collider);
				}
				else {
					StartCoroutine (DestroyProcess (destroyDelay));
				}
			}
		}

		//Ativado para destruir o gameObject
		private IEnumerator DestroyProcess(float waitTime){
			StartCoroutine(AudioController.GetClass.StartAudio (dropSource, dropClips [0], 0.2f, false));
			dropAnimator.enabled = true;
			yield return new WaitForSeconds (dropAnimator.GetCurrentAnimatorStateInfo (0).length);
			Destroy (dropCol);
			Destroy (dropRigid);
			Destroy (dropRenderer);
			Destroy (dropAnimator);
			yield return new WaitForSeconds (waitTime);
			StopAllCoroutines ();
			Destroy (dropSource);
			Destroy (gameObject);
		}

		//Coloca a plataforma sobre as leis da fisica(deixa cair)
		private void DropObject(){
			if (dropRigid != null) {
				dropRigid.isKinematic = false;
			}
		}

	}

}