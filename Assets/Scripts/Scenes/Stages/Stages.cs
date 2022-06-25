using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TheCaveSurvivor.Characters;
using TheCaveSurvivor.Controllers;

namespace TheCaveSurvivor.Scene{

	/*
	 * Classe: <<abstract>>
	 * 			  Stages
	 * 
	 * 	Herda de: <<abstract>>
	 * 			  	 Scenes
	 * 
	 * 	Define comportamento para as fases
	 * 
	 * */

	public abstract class Stages : Scenes {

		protected			float 					stageTime;	
		protected			Player					player;
		protected			Vector2 				defaultSpawnPosition;
		protected	static	bool	 				lockCommands, stopTime, liberateKeys, pressedButton, enterEnd, pausedGame;	
		protected			GameObject 				endStageObject;

		public Vector2 GetDefaultSpawnPosition{
			get{
				return defaultSpawnPosition;
			}
		}						
		public static bool LockCommands{
			get {
				return lockCommands;
			}
		}
		public static bool StopTime{
			get{
				return stopTime;
			}
		}
		public static bool PausedGame{
			get{
				return pausedGame;
			}
		}
			
		protected abstract void EndDetector ();
		protected abstract void GoToNextStage();
		protected abstract IEnumerator CourseEnd ();
		protected abstract IEnumerator PauseGame();

	}

}
