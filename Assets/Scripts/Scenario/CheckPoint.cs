using UnityEngine;
using System.Collections;
using TheCaveSurvivor.Characters;
using TheCaveSurvivor.Controllers;
using TheCaveSurvivor.Scene;

namespace TheCaveSurvivor.Scenario{

	/*
	 * Classe: CheckPoint
	 * 
	 * 	Responsável por gerenciar os checkpoints do jogo
	 * 
	 * */

	public class CheckPoint : MonoBehaviour {

		private			Player					player;
		private			Animator 				torchAnimator;
		private			SpriteRenderer			torchRenderer;
		private			AudioSource 			torchSource;
		private			AudioClip[]				torchClips;
		public			Light 					torchLight;
		private			bool					passedThroughCheckpoint;

		//Chamado apenas uma vez
		private void Start(){
			//Chama o metodo Start da classe Scenario
			player = GameObject.FindObjectOfType<Player> ();
			torchAnimator = GetComponent<Animator> ();
			torchRenderer = GetComponent<SpriteRenderer> ();
			torchSource = GetComponent<AudioSource> ();
			torchLight = GetComponentInChildren<Light> ();
			torchAnimator.enabled = false;
			torchClips = new AudioClip[] { Resources.Load<AudioClip>("Audio/SFX/Animations/torch_lightup"), Resources.Load<AudioClip> ("Audio/SFX/Animations/fire_burning") };
		}
			
		//Detecta entrada de algo na área do colisor
		private void OnTriggerEnter2D(Collider2D col){												
			if (col.gameObject.CompareTag("Player")) {
				StartCoroutine(SetCheckpoint (true));
			}
		}

		//Método para atribuir localização do checkpoint a posição de renascimento do jogador
		private IEnumerator SetCheckpoint(bool check){
			if (check && !passedThroughCheckpoint) {
				passedThroughCheckpoint = true;
				StartCoroutine(AudioController.GetClass.StartAudio (torchSource, torchClips [0], 1f, false));
				torchAnimator.enabled = true;
				torchLight.enabled = check;
				player.RespawnPosition = transform.position;
				yield return new WaitUntil (() => !torchSource.isPlaying);
				StartCoroutine(AudioController.GetClass.StartAudio (torchSource, torchClips [1], 1f, true));
			}
		}


	}

}
