using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TheCaveSurvivor.Characters;

namespace TheCaveSurvivor.Controllers{

	/*
	 * Classe: HUDManager
	 * 
	 * 	Gerencia informações gráficas do jogador na tela
	 * 
	 * */

	public class HUDManager : MonoBehaviour {

		public		static 		HUDManager				instance;

		public					SpriteRenderer[]		heartSprites;
		public 					Text 					lifesText, keysText;
		private					Player					player;

		private void Awake(){
			player = GameObject.FindObjectOfType<Player> ();
			instance = this;
		}			

		public static HUDManager GetClass{
			get{ 
				return instance; 
			}
		}

		//Gerencia os corações
		public void HeartsManager(){			
			for (int x = 0; x < heartSprites.Length; x++) {
				heartSprites [x].enabled = false;
			}
			for (int x = 0; x < player.PlayerHearts; x++) {
				heartSprites [x].enabled = true;
			}
		}

		//Gerencia as vidas
		public void LifesManager(){
			lifesText.text = player.PlayerLifes.ToString();
		}

		//Gerencia as chaves
		public void KeysManager(){
			keysText.text = player.KeysQuantity.ToString ();
		}					


	}

}