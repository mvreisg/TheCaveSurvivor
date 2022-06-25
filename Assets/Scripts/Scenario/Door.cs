using UnityEngine;
using System.Collections;
using TheCaveSurvivor.Characters;
using TheCaveSurvivor.Controllers;

namespace TheCaveSurvivor.Scenario{

	/*
	 * Classe: Door
	 * 
	 * 	Classe que gerencia as portas do jogo
	 * 
	 * */

	public class Door : MonoBehaviour {

		public				int						keysToOpen;
		private				bool					alreadyOpen;
		private				bool					showKeyText = false;
		private				Player					player;
		private				AudioSource				doorSource;
		private				BoxCollider2D			doorCollider;
		private				SpriteRenderer			doorRenderer;
		public				Sprite[] 				doorSprites;
		public				AudioClip[] 			doorClips;

		//Chamado apenas uma vez
		private void Start(){
			//Chama o método Start da classe Scenario
			player = GameObject.FindObjectOfType<Player> ();
			doorSource = GetComponent<AudioSource> ();
			doorCollider = GetComponent<BoxCollider2D> ();
			doorRenderer = GetComponent<SpriteRenderer> ();
			doorClips = new AudioClip[] {
				Resources.Load<AudioClip> ("Audio/SFX/Animations/door_locked"),
				Resources.Load<AudioClip> ("Audio/SFX/Animations/door_opening")
			};
			OpenDoor (alreadyOpen);
		}

		//Detecta algo que colidiu com o objeto
		private void OnCollisionEnter2D(Collision2D col){			
			
			if (col.gameObject.CompareTag ("Player")) {
				if (player.KeysQuantity >= keysToOpen) {					
					OpenDoor (true);
				}
				else if (!showKeyText){
					StartCoroutine(AudioController.GetClass.StartAudio (doorSource, doorClips [0], 0.5f, false));
					showKeyText = true;	
				}
			}
		}

		//Método para checar se a porta está aberta ou não
		private void OpenDoor(bool open){
			if (open) {
				player.KeysQuantity -= keysToOpen;
				alreadyOpen = true;
				if (doorCollider != null) {
					doorCollider.enabled = false;
				}
				if (GetComponent<SpriteRenderer>() != null) {
					StartCoroutine(AudioController.GetClass.StartAudio (doorSource, doorClips [1], 0.5f, false));
					doorRenderer.sprite = doorSprites[1] as Sprite;
				}
			}
		}
	

	}

}