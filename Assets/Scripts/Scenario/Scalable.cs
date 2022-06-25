using UnityEngine;
using System.Collections;
using TheCaveSurvivor.Characters;

namespace TheCaveSurvivor.Scenario{

	/*
	 * Classe: Scalable

	 * 	Classe para objetos escaláveis pelo jogador
	 * 
	 * */

	public class Scalable : MonoBehaviour {

		//Detecta algo que entrou na área de colisão do colisor deste objeto e permaneceu
		private void OnTriggerStay2D(Collider2D col){
			if (col.gameObject.CompareTag ("Player"))
				col.gameObject.GetComponent<Player> ().OnScalable = true;
		}

		//Detecta algo que saiu da área de colisão do colisor deste objeto
		private void OnTriggerExit2D(Collider2D col){
			if (col.gameObject.CompareTag ("Player")) 
				col.gameObject.GetComponent<Player> ().OnScalable = false;
		}


	}

}