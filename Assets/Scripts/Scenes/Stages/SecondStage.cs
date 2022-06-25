using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using TheCaveSurvivor.Characters;
using TheCaveSurvivor.Scene;
using TheCaveSurvivor.Controllers;

namespace TheCaveSurvivor.Scene{

	/*
	 * Classe: SecondStage
	 * 
	 * 	Gerencia a segunda fase
	 * 
	 * */

	public class SecondStage : Stages {

		protected override void Awake(){			
			player = GameObject.FindObjectOfType<Player> ();
			scenePlayer = GameObject.FindObjectOfType<GameCamera>().GetComponent<AudioSource> ();			
			defaultSpawnPosition = new Vector2 (-3.6f, -1.22f);
			sceneClips = new AudioClip[] {
				Resources.Load<AudioClip>("Audio/Soundtrack/stage2_area"),
				Resources.Load<AudioClip>("Audio/Soundtrack/end_stage"),
				Resources.Load<AudioClip>("Audio/SFX/UI/sfx_menu_select"),
				Resources.Load<AudioClip>("Audio/SFX/UI/sfx_sounds_pause_in"),
				Resources.Load<AudioClip>("Audio/SFX/UI/sfx_sounds_pause_out")
			};
		}

		//Chamado apenas uma vez
		protected void Start(){	
			StartCoroutine(AudioController.GetClass.StartAudio (scenePlayer, sceneClips[0], 0.3f, true));
			StartCoroutine (UIController.GetClass.Fade_InScene(0.5f));		
			lockCommands = false;
		}

		//Chamado em cada quadro
		protected void Update(){			
			EndDetector ();
			if (Input.GetKeyDown (KeyCode.Keypad8)) { AdvanceScene (1); }
			if (Input.GetKeyDown (KeyCode.Keypad2)) { AdvanceScene (-1); }
			if (Input.GetKeyDown (KeyCode.Keypad5)) { ReloadScene (); }
			if (!stopTime) { stageTime += Time.deltaTime; }
			if (liberateKeys) {	if (Input.GetKeyDown (KeyCode.Return)) { GoToNextStage (); } }
			if (Input.GetKeyDown (KeyCode.Escape) && !enterEnd) { StartCoroutine(PauseGame ()); }
		}
			
		private void OnDrawGizmos(){
			endStageObject = GameObject.FindGameObjectWithTag ("EndStageObject");
			Gizmos.DrawWireCube (endStageObject.transform.position, endStageObject.GetComponent<BoxCollider2D> ().size);
			Gizmos.color = Color.yellow;
		}

		protected override IEnumerator PauseGame(){			
			if (!pausedGame) {
				pausedGame = true;
				Text pausedText, buttonsText;
				Image blackImg;
				Vector2 pTxtlocPos = new Vector2 (0, 45f);
				Vector2 pTxtlocScale = new Vector2 (1, 1);
				Vector2 pTxtlocSize = new Vector2 (150, 40);
				pausedText = UIController.GetClass.Text_Generate (pTxtlocPos, pTxtlocScale, pTxtlocSize, "Pausado", Color.yellow, 40, TextAnchor.MiddleCenter);
				Vector2 bTxtlocPos = new Vector2 (0, -20f);
				Vector2 bTxtlocScale = new Vector2 (1, 1);
				Vector2 bTxtlocSize = new Vector2 (250, 150);
				buttonsText = UIController.GetClass.Text_Generate (bTxtlocPos, bTxtlocScale, bTxtlocSize, "ESC - Tela inicial\nEnter - Voltar ao jogo", Color.white, 22, TextAnchor.MiddleCenter);
				Vector2 bImglocPos = new Vector3 (0f, 0f, -1f);
				Vector2 bImglocScale = new Vector2 (1f, 1f);
				Vector2 bImglocSize = new Vector2 (1000, 1000);
				blackImg = UIController.GetClass.Image_Generate (bImglocPos, bImglocScale, bImglocSize);
				Color blackImgColor = new Color (0f, 0f, 0f, 0.25f);
				blackImg.color = blackImgColor;
				float delaySecTime = 0.01f;
				yield return new WaitForSeconds (delaySecTime);
				StartCoroutine (AudioController.GetClass.StartAudio (scenePlayer, sceneClips [3], 1f, false));
				Time.timeScale = 0;
				yield return new WaitUntil (() => Input.GetKeyDown (KeyCode.Return) || Input.GetKeyDown (KeyCode.Escape));
				if (Input.GetKeyDown (KeyCode.Escape)) {
					Time.timeScale = 1;
					pausedGame = false;
					Destroy (pausedText.gameObject);
					Destroy (buttonsText.gameObject);
					Destroy (blackImg.gameObject);
					Scenes.GoToScene ((int)ScenesIndexes.startScreen);
				}
				else if (Input.GetKeyDown (KeyCode.Return)) {
					Time.timeScale = 1;
					pausedGame = false;
					Destroy (pausedText.gameObject);
					Destroy (buttonsText.gameObject);
					Destroy (blackImg.gameObject);
					StartCoroutine (AudioController.GetClass.StartAudio (scenePlayer, sceneClips [4], 1f, false));
					yield return new WaitUntil (() => !scenePlayer.isPlaying);
					StartCoroutine (AudioController.GetClass.StartAudio (scenePlayer, sceneClips [0], 0.3f, true));
				}

			}

		}

		protected override void EndDetector (){
			endStageObject = GameObject.FindGameObjectWithTag ("EndStageObject");
			bool collide = Physics2D.OverlapBox (endStageObject.transform.position, endStageObject.GetComponent<BoxCollider2D> ().size, 0f, 1 << LayerMask.NameToLayer ("Player"));
			if (collide && !enterEnd) {
				enterEnd = true;
				StartCoroutine (CourseEnd());
			}
		}			

		//Gerenciador do processo de finalização da fase
		protected override IEnumerator CourseEnd(){

			lockCommands = true;
			stopTime = true;

			StartCoroutine (AudioController.GetClass.StartAudio (scenePlayer, sceneClips [1], 0.5f, false));

			GameData.gameData.RespawnPosition = new Vector2 (-8.25f, -5.63f);
			GameData.gameData.PlayerLifes = player.PlayerLifes;
			GameData.gameData.PlayerPickedPickaxe = player.PickedPickaxe;
			GameData.gameData.SecondStageTime += stageTime;
			GameData.gameData.StoragedData = true;

			Vector3 imglocalPos = new Vector3 (0, 0, 1);
			Vector3 imglocalScale = new Vector2 (Screen.width, Screen.height);
			Vector3 imgsizeDelta = new Vector2 (200, 120);
			Image blackImage = UIController.GetClass.Image_Generate (imglocalPos, imglocalScale, imgsizeDelta);
			StartCoroutine(UIController.GetClass.Fade_OutScene (0.5f, blackImage));

			yield return new WaitUntil (() => blackImage.color.a >= 0.8);

			Vector3 txt1localPos = new Vector3 (0, 100, 1);
			Vector3 txt1localScale = new Vector3 (1, 1, 1);
			Vector3 txt1sizeDelta = new Vector2 (600, 120);
			Text parabensText = UIController.GetClass.Text_Generate (txt1localPos, txt1localScale, txt1sizeDelta);
			parabensText.text = "Mais um pouco... para o final!";
			parabensText.color = Color.white;
			parabensText.font = UIController.GetUIFont;
			parabensText.fontSize = 50;
			parabensText.alignment = TextAnchor.MiddleCenter;

			Vector3 txt2localPos = new Vector3 (0, 50, 1);
			Vector3 txt2localScale = new Vector3 (1, 1, 1);
			Vector3 txt2sizeDelta = new Vector2 (200, 120);
			string minutes, seconds;
			minutes = Mathf.FloorToInt (GameData.gameData.SecondStageTime / 60).ToString ("00");
			seconds = Mathf.FloorToInt (GameData.gameData.SecondStageTime % 60).ToString("00");
			Text timeText = UIController.GetClass.Text_Generate (txt2localPos, txt2localScale, txt2sizeDelta);
			timeText.text = "Tempo: " + minutes + ":" + seconds;
			timeText.color = Color.white;
			timeText.font = UIController.GetUIFont;
			timeText.fontSize = 30;
			timeText.alignment = TextAnchor.MiddleCenter;

			yield return new WaitForSeconds (5f);
			liberateKeys = true;

			Vector3 txt3localPos = new Vector3 (0, -140, 1);
			Vector3 txt3localScale = new Vector3 (1, 1, 1);
			Vector3 txt3sizeDelta = new Vector2 (400, 100);
			Text continueText = UIController.GetClass.Text_Generate (txt3localPos, txt3localScale, txt3sizeDelta);
			continueText.text = "Continue...";
			continueText.color = Color.white;
			continueText.font = UIController.GetUIFont;
			continueText.fontSize = 40;
			continueText.alignment = TextAnchor.MiddleCenter;

			StartCoroutine (UIController.GetClass.Blink_Text (continueText, true));

		}
			
		protected override void GoToNextStage(){
			liberateKeys = false;
			lockCommands = false;
			stopTime = false;
			enterEnd = false;
			StartCoroutine (AudioController.GetClass.StartAudio (scenePlayer, sceneClips [2], 0.5f, false));
			Scenes.AdvanceScene (1);
		}			

		protected void OnDestroy(){
			GameData.gameData.SecondStageTime += stageTime;
		}

	}

}