using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TheCaveSurvivor.Controllers;

namespace TheCaveSurvivor.Scene{

	/*
	 * Classe: GameData
	 * 
	 * 	Salva informações do jogador entre as cenas evitando a perda e destruição pelo recarregamento destas
	 * 
	 * */

	public class GameData : MonoBehaviour {

		public		static 	GameData 			gameData;

		private				int 				playerLifes;
		private				bool 				playerPickedPickaxe;
		private				bool 				storagedData;
		private				Vector2 			respawnPosition;
		private				float 				firstStageTime, secondStageTime, finalStageTime;

		//Propriedades para acesso externo
		public bool StoragedData{
			get{ 
				return this.storagedData;
			}
			set{ 
				this.storagedData = value; 
			}
		}
		public int PlayerLifes{
			get{ 
				return this.playerLifes; 
			}
			set{
				this.playerLifes = value; 
			}
		}

		public bool PlayerPickedPickaxe{
			get{ 
				return this.playerPickedPickaxe; 
			}
			set{ 
				this.playerPickedPickaxe = value; 
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

		public float FirstStageTime{
			get{ 
				return this.firstStageTime; 
			}
			set{
				this.firstStageTime = value; 
			}
		}
		public float SecondStageTime{
			get{ 
				return this.secondStageTime; 
			}
			set{
				this.secondStageTime = value; 
			}
		}
		public float FinalStageTime{
			get{ 
				return this.finalStageTime; 
			}
			set{
				this.finalStageTime = value; 
			}
		}

		//Chamado na compilação
		private void Awake(){
			if (gameData == null) {
				DontDestroyOnLoad (transform.root.gameObject);
				gameData = this;
			}
			else if (gameData != this){
				Destroy (gameObject);
			}
		}

		private void Update(){
			switch (SceneManager.GetActiveScene().buildIndex) {
			case (int)ScenesIndexes.startScreen:	
				if (GameObject.FindObjectOfType<GameData> () != null) {
					Instantiate (Resources.Load<GameObject> ("Prefabs/EventMaker/GameData"), transform.root);
					Destroy (gameObject);
				}
				break;
			case (int)ScenesIndexes.firstCourse:
				foreach (GameData dataObject in GameObject.FindObjectsOfType<GameData>()) {
					if (dataObject == null) {
						Instantiate (Resources.Load<GameObject> ("Prefabs/EventMaker/GameData"), transform.root);
					} 
					else {						
						if (gameData == null) {
							DontDestroyOnLoad (transform.root.gameObject);
							gameData = this;
						} 
						else if (gameData != this) {
							Destroy (gameObject);
						}
					}
				}
				break;
			case (int)ScenesIndexes.secondCourse:
				foreach (GameData dataObject in GameObject.FindObjectsOfType<GameData>()) {
					if (dataObject == null) {
						Instantiate (Resources.Load<GameObject> ("Prefabs/EventMaker/GameData"), transform.root);
					} 
					else {						
						if (gameData == null) {
							DontDestroyOnLoad (transform.root.gameObject);
							gameData = this;
						} else if (gameData != this) {
							Destroy (gameObject);
						}
					}
				}
				break;
			case (int)ScenesIndexes.finalCourse:
				foreach (GameData dataObject in GameObject.FindObjectsOfType<GameData>()) {
					if (dataObject == null) {
						Instantiate (Resources.Load<GameObject> ("Prefabs/EventMaker/GameData"), transform.root);
					} 
					else {						
						if (gameData == null) {
							DontDestroyOnLoad (transform.root.gameObject);
							gameData = this;
						} else if (gameData != this) {
							Destroy (gameObject);
						}
					}
				}
				break;
			case (int)ScenesIndexes.gameOver:
				if (GameObject.FindObjectOfType<GameData> () != null) {
					Instantiate (Resources.Load<GameObject> ("Prefabs/EventMaker/GameData"), transform.root);
					Destroy (gameObject);
				}
				break;
			case (int)ScenesIndexes.credits:
				if (GameObject.FindObjectOfType<GameData> () != null) {
					Instantiate (Resources.Load<GameObject> ("Prefabs/EventMaker/GameData"), transform.root);
					Destroy (gameObject);
				}
				break;
			}
		}

	}

}
