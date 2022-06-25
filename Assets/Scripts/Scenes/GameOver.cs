using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TheCaveSurvivor.Controllers;

namespace TheCaveSurvivor.Scene{

	/*
	 * Classe: GameOver
	 * 
	 * 	Gerencia a tela de GameOver
	 * 
	 * */

	public class GameOver : Scenes {

		public 		Text	 				pressAnyKeyText;	

		protected override void Awake (){
			scenePlayer = GameObject.FindObjectOfType<GameCamera>().GetComponent<AudioSource> ();	
			sceneClips = new AudioClip[] {
				Resources.Load<AudioClip>("Audio/Soundtrack/gameover_area"),
				Resources.Load<AudioClip>("Audio/SFX/UI/sfx_menu_select")
			};
		}

		//Chamado apenas uma vez
		private void Start(){		
			StartCoroutine(AudioController.GetClass.StartAudio (scenePlayer, sceneClips[0], 1f, false));
			StartCoroutine (UIController.GetClass.Blink_Text (pressAnyKeyText, true));
		}

		//Chamado em cada quadro
		private void Update(){	
			GoToStartScreen (Input.anyKeyDown);
		}

		//vai pra tela inicial
		protected void GoToStartScreen(bool input){
			if (input) {
				StopAllCoroutines ();
				StartCoroutine(AudioController.GetClass.StartAudio (scenePlayer, sceneClips[1], 1f, false));
				Scenes.GoToScene ((int)ScenesIndexes.startScreen);
			}
		}

	}

}