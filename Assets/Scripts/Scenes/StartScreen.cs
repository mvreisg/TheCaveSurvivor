using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TheCaveSurvivor.Controllers;
using TheCaveSurvivor.Characters;

namespace TheCaveSurvivor.Scene{

	/*
	 * Classe: StartScreen
	 * 
	 * 	Gerencia a tela inicial do jogo
	 * 
	 * */

	public class StartScreen : Scenes {
		
		public		Text					pressSpaceText;

		protected override void Awake (){
			scenePlayer = GameObject.FindObjectOfType<GameCamera>().GetComponent<AudioSource> ();	
			sceneClips = new AudioClip[] {
				Resources.Load<AudioClip>("Audio/Soundtrack/start_sting"),
				Resources.Load<AudioClip>("Audio/Soundtrack/start_cont"),
				Resources.Load<AudioClip>("Audio/SFX/UI/sfx_menu_select")
			};
		}

		//Chamado apenas uma vez
		protected virtual void Start(){		
			StartCoroutine (AudioController.GetClass.StartAudio (scenePlayer, sceneClips [0], 0.5f, false));
			//StartCoroutine (AudioController.GetClass.PlaySequenced(scenePlayer, sceneClips, 0.5f, false));
			StartCoroutine (UIController.GetClass.Blink_Text (pressSpaceText, true));
		}

		//Chamado a cada quadro
		protected virtual void Update(){	
			StartGame (Input.GetKeyDown(KeyCode.Return));
			ExitGame (Input.GetKeyDown (KeyCode.Escape));
		}

		//Inicia o jogo
		private void StartGame(bool input){
			if (input) {				
				StartCoroutine (AudioController.GetClass.StartAudio (scenePlayer, sceneClips [2], 0.5f, false));
				Scenes.GoToScene((int)ScenesIndexes.firstCourse); 
			}
		}

		//Sai do jogo
		private void ExitGame(bool input){
			if (input) {
				Application.Quit ();
			}
		}

	}

}