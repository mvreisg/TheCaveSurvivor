using UnityEngine;
using System.Collections;

namespace TheCaveSurvivor.Controllers{

	/*
	 * Classe: AudioController
	 * 
	 * 	Contém métodos para controle de áudio
	 * 
	 * */

	public class AudioController : MonoBehaviour {

		private static		AudioController		instance;

		private void Awake(){
			instance = this;
		}

		public static AudioController GetClass{
			get{
				return instance;
			}
		}

		//Toca um audio com base no AudioSource, no volume e se ele vai se repetir
		public IEnumerator StartAudio(AudioSource audioSource, AudioClip audioClip, float volume, bool loop){						
			audioSource.clip = audioClip;
			audioSource.volume = volume;
			audioSource.loop = loop;
			audioSource.Play ();
			yield return null;
		}

		public IEnumerator StartAudio(AudioSource audioSource, AudioClip audioClip, float volume, bool loop, float delay){			
			yield return new WaitForSeconds(delay);
			audioSource.clip = audioClip;
			audioSource.volume = volume;
			audioSource.loop = loop;
			audioSource.Play ();
		}

		public IEnumerator PlaySequenced(AudioSource audioSource, AudioClip[] audioClip, float volume, bool infinite){			
			for (int cp = 0; cp < audioClip.Length; cp++){
				audioSource.clip = audioClip [cp];
				audioSource.volume = volume;
				audioSource.loop = false;
				audioSource.Play();
				print (cp);
				yield return new WaitUntil (() => !audioSource.isPlaying);
			}
			if (infinite) {	
				StartCoroutine(PlaySequenced (audioSource, audioClip, volume, infinite)); 
			}
		}						

		public IEnumerator DownVolume(AudioSource audioSource, float decay, float volume){
			for (float deg = audioSource.volume; deg > volume; deg -= decay * Time.deltaTime) {
				audioSource.volume = deg;
				yield return null;
			}
		}

		public IEnumerator UpVolume(AudioSource audioSource, float rise, float volume){
			for (float deg = audioSource.volume; deg <= 1.0; deg += rise * Time.deltaTime) {
				audioSource.volume = deg;
				yield return null;
			}
		}

		//Pausa o audio
		public IEnumerator PauseAudio(AudioSource audioSource, bool pause){
			if (pause) {				
				audioSource.Pause ();
			}
			else {
				audioSource.UnPause ();
			}
			yield return null;
		}

		//Para o audio
		public IEnumerator StopAudio(AudioSource audioSource){
			audioSource.Stop ();
			yield return null;
		}
			
	}

}