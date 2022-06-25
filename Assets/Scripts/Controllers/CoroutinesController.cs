using UnityEngine;
using System.Collections;

namespace TheCaveSurvivor.Controllers{

	/*
	 * Classe: CoroutinesController
	 * 
	 * 	Contém métodos de retorno IEnumerator comuns entre as classes
	 * 
	 * */

	public class CoroutinesController : MonoBehaviour {

		private static		CoroutinesController	instance;

		private void Awake(){
			instance = this;
		}

		public static CoroutinesController GetClass{
			get{
				return instance;
			}
		}

		//Pisca o sprite e aplica cor
		public IEnumerator Blink_Sprite(SpriteRenderer renderer, Color color, float duration, float speed){
			for (float x = 0; x < duration; x += speed * Time.deltaTime) {
				renderer.enabled = renderer.enabled ? false : true;
				renderer.color = color;
				yield return null;
			}
			renderer.enabled = true;
			renderer.color = Color.white;
		}

	}

}