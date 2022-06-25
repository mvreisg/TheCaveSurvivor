using UnityEngine;
using System.Collections;
using TheCaveSurvivor.Controllers;
using TheCaveSurvivor.Characters;
using TheCaveSurvivor.Enemy;

namespace TheCaveSurvivor.Scenario{

	/*
	 * Classe: Water
	 * 
	 * 	Classe para objetos nadáveis pelo jogador
	 * 
	 * */

	public class Water : MonoBehaviour {

		//Constante

		private				Collider2D 			waterTriggerCol;
		private				SpriteRenderer		waterRenderer;
		private				AudioSource			waterSource;
		private				AudioClip			enterexitWater_Clip,
												waterambience_Clip,
												insideWater_Clip;

		private		const	float				spriteOffset = 0.000025f;
		public		const	float				fluidDensity = 0.5f;

		public AudioClip EnterExitWaterClip{
			get{
				return this.enterexitWater_Clip;
			}
		}

		public AudioClip WaterAmbienceClip{
			get{
				return this.waterambience_Clip;
			}
		}

		public AudioClip InsideWaterClip{
			get{
				return this.insideWater_Clip;
			}
		}


		//Chamado apenas uma vez
		private void Start(){			
			waterTriggerCol = GetComponent<Collider2D> ();
			enterexitWater_Clip = Resources.Load<AudioClip> ("Audio/SFX/Ambience/water_enter");
			waterambience_Clip = Resources.Load<AudioClip>("Audio/SFX/Ambience/water_ambience");
			insideWater_Clip = Resources.Load<AudioClip> ("Audio/SFX/Ambience/water_under");
			waterSource = GetComponent<AudioSource> ();
			waterRenderer = GetComponent<SpriteRenderer> ();
			StartCoroutine(MoveSprite (waterRenderer));		
			StartCoroutine(AudioController.GetClass.StartAudio (waterSource, waterambience_Clip, 0.15f, true));
		}

		//Move o sprite da água
		private IEnumerator MoveSprite(SpriteRenderer renderer){	
			while (renderer.material.mainTextureOffset.x <= 0.5f) {
				renderer.material.mainTextureOffset += new Vector2 (spriteOffset, 0f);
				yield return null;
			}
			yield return new WaitForSeconds (1f);
			while (renderer.material.mainTextureOffset.x >= 0f) {
				renderer.material.mainTextureOffset -= new Vector2 (spriteOffset, 0f);			
				yield return null;
			}
			yield return new WaitForSeconds (1f);
			StartCoroutine(MoveSprite (renderer));
		}

		//Detecta algo que entrou na água
		private void OnTriggerEnter2D(Collider2D col){
			if (col.gameObject.layer == LayerMask.NameToLayer ("Player")) {
				if (col.gameObject.CompareTag ("Player")) {
					col.attachedRigidbody.gravityScale = fluidDensity;
					StartCoroutine(AudioController.GetClass.StopAudio (waterSource));
					StartCoroutine(AudioController.GetClass.StartAudio (col.gameObject.GetComponent<Player> ().GetPlayerAmbienceSource, enterexitWater_Clip, 0.15f, false));
					StartCoroutine(AudioController.GetClass.StartAudio (col.gameObject.GetComponent<Player> ().GetPlayerAmbienceSource, insideWater_Clip, 0.45f, true, 0.2f));
				}
			}
			else {
				if (col.gameObject.GetComponent<AudioSource> () != null && !col.gameObject.CompareTag("Pickaxe")) {
					StartCoroutine(AudioController.GetClass.StartAudio (col.gameObject.GetComponent<AudioSource> (), enterexitWater_Clip, 0.15f, false));
				}
				col.attachedRigidbody.gravityScale = fluidDensity;
			}
		}

		//Detecta algo que está na água 
		private void OnTriggerStay2D(Collider2D col){
			if (col.gameObject.CompareTag ("Player")) {										
				col.attachedRigidbody.gravityScale = fluidDensity;
			}
			else {
				col.attachedRigidbody.gravityScale = fluidDensity;
			}
		}

		//Detecta algo que saiu da água
		private void OnTriggerExit2D(Collider2D col){
			if (col.gameObject.CompareTag ("Player")) {		
				StartCoroutine(AudioController.GetClass.StartAudio (col.gameObject.GetComponent<Player> ().GetPlayerAmbienceSource, enterexitWater_Clip, 0.15f, false));
				StartCoroutine(AudioController.GetClass.StartAudio (waterSource, waterambience_Clip, 0.15f, true));
				col.attachedRigidbody.gravityScale = Player.playerGravity;
			}
			else if (col.gameObject.CompareTag ("Slime")) {				
				col.attachedRigidbody.gravityScale = Slime.gravityScale;
			}
			else if (col.gameObject.CompareTag ("Bat")) {				
				col.attachedRigidbody.gravityScale = Bat.gravityScale;
			}
			else if (col.gameObject.CompareTag ("Zombie")) {
				col.attachedRigidbody.gravityScale = Zombie.gravityScale;
			}
		}

	}

}