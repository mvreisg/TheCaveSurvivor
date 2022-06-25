using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TheCaveSurvivor.Controllers;

namespace TheCaveSurvivor.Scene{

	//Enumerador que contém o index das cenas do jogo
	public enum ScenesIndexes{
		startScreen = 0, 
		gameOver, 
		firstCourse, 
		secondCourse, 
		finalCourse, 
		credits
	}

	/*
	 * Classe: <<abstract>>
	 * 			  Scenes
	 * 
	 * 	Define comportamento para as cenas
	 * 
	 * */

	public abstract class Scenes : MonoBehaviour {

		protected			AudioSource				scenePlayer;
		protected			AudioClip[]				sceneClips;

		//PROPERTIES
		public AudioSource ScenePlayer{
			get{
				return this.scenePlayer;
			}
		}

		public AudioClip[] SceneClips{
			get{
				return this.sceneClips;
			}
		}

		//ABSTRACT
		//- é necessário que o método Awake() exista nas classes da herança para inicializar os atributos vitais para as fases
		protected abstract void Awake ();			

		//STATIC
		//Recarrega a cena
		public static void ReloadScene(){
			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
		}

		//Avança ou regressa uma cena
		public static void AdvanceScene(int value){
			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex + value);
		}

		//Carrega uma cena específica com base no index
		public static void GoToScene(int value){
			SceneManager.LoadScene (value);
		}

		//Carrega uma cena específica com base no nome
		public static void GoToScene(string name){
			SceneManager.LoadScene (name);
		}

	}

}
