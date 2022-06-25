using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TheCaveSurvivor.Controllers;

namespace TheCaveSurvivor.Scene{

	/*
	 * Classe: Credits
	 * 
	 * 	Gerencia a cena de créditos
	 * 
	 * */

	public class Credits : Scenes {

		public		Transform				creditosBody;
		public		Text					timeText, backText;
		public		float					scrollSpeed;
		public		Collider2D 				creditsRollEnder;

		protected override void Awake (){
			scenePlayer = GameObject.FindObjectOfType<GameCamera>().GetComponent<AudioSource> ();	
			sceneClips = new AudioClip[] {
				Resources.Load<AudioClip>("Audio/Soundtrack/credits_enter"),
				Resources.Load<AudioClip>("Audio/SFX/UI/sfx_menu_select2"),
			};
		}

		//Chamado apenas uma vez
		protected virtual void Start(){								
			StartCoroutine(AudioController.GetClass.StartAudio(scenePlayer, sceneClips[0], 1f, false));
			StartCoroutine(UIController.GetClass.Blink_Text (backText, true));
			float finalTime = GameData.gameData.FirstStageTime + GameData.gameData.SecondStageTime + GameData.gameData.FinalStageTime;
			string minutes, seconds;
			minutes = Mathf.FloorToInt (finalTime / 60).ToString ("00");
			seconds = Mathf.FloorToInt (finalTime % 60).ToString ("00");
			timeText.text = "Tempo: " + minutes + ":" + seconds;
		}

		//Chamado em cada quadro
		protected virtual void Update(){	
			Collider2D endRoll = Physics2D.OverlapBox (GameCamera.GetClass.GetCamera.ScreenToWorldPoint(new Vector2(Screen.width / 2, Screen.height / 2)), new Vector3 (0.1f, 0.1f, 1f), 0f);
			if (endRoll == null && endRoll != creditsRollEnder) {
				creditosBody.Translate (Vector2.up * Time.deltaTime * scrollSpeed);	
			}
			GoToStartScreen (Input.GetKeyDown(KeyCode.Return));
		}
			
		private void OnDrawGizmos(){			
			Gizmos.DrawWireCube (GameCamera.GetClass.GetCamera.ScreenToWorldPoint(new Vector2(Screen.width / 2, Screen.height / 2)), new Vector3 (0.1f, 0.1f, 1f));
			Gizmos.color = Color.white;
		}

		//Vai pra tela inicial
		protected void GoToStartScreen(bool input){	
			if (input) {
				StartCoroutine(AudioController.GetClass.StartAudio(scenePlayer, sceneClips[1], 1f, false));
				Scenes.GoToScene ((int)ScenesIndexes.startScreen);	
			}
		}			

	}

}