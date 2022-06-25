using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TheCaveSurvivor.Characters;
using TheCaveSurvivor.Enemy;
using TheCaveSurvivor.Controllers;

namespace TheCaveSurvivor.Scene{

	/*
	 * Classe: DeathDetector
	 * 
	 * 	Utilizada para detectar objetos que passam do limite do cenário
	 * 
	 * */

	public class LimitDetector : MonoBehaviour {

		//Detecta se algo passou do limite
		private void OnTriggerEnter2D(Collider2D col){			
			if (col.gameObject.layer == LayerMask.NameToLayer ("Player")) {
				if (col.gameObject.CompareTag ("Player")) {				
					GameCamera.GetClass.LockCamera ();
					StartCoroutine (UIController.GetClass.Fade_OutScene (0.5f));
					StartCoroutine (col.gameObject.GetComponent<Player> ().Die ());
				}		
			}
			else {
				Destroy (col.gameObject);
			}

		}

	}

}