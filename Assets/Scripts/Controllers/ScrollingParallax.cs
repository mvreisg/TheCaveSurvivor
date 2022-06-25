using UnityEngine;
using System.Collections;

namespace TheCaveSurvivor.Controllers{

	/*
	 * Classe: ScrollingParallax
	 * 
	 * 	Classe responsáve por gerenciar as imagens de fundo e aplicar o efeito parallax 
	 * 
	 * */

	public class ScrollingParallax : MonoBehaviour {

		private		Renderer			renderer;
		public		float				offsetSpeed;
		public		bool 				scrollWithoutCamera;

		private void Start(){
			renderer = GetComponent<Renderer> ();
		}

		//Move as imagens com base na posição x da camera
		private void LateUpdate () {	
			if (!scrollWithoutCamera) {
				Vector2 offset2 = new Vector2 (GameCamera.GetClass.gameObject.transform.position.x * offsetSpeed, 0f);
				renderer.material.mainTextureOffset = offset2;
			}
			else {
				Vector2 offset2 = new Vector2 (offsetSpeed, 0f);
				renderer.material.mainTextureOffset += offset2;
			}
		}

	}

}