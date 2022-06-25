using UnityEngine;
using System.Collections;
using TheCaveSurvivor.Characters;
using TheCaveSurvivor.Controllers;
using TheCaveSurvivor.Enemy;

namespace TheCaveSurvivor.Scenario{

	/*
	 * Classe: DropPlatform
	 * 
	 * 	Responsável por controlar as plataformas que caem
	 * 
	 * */

	public class Spikes : MonoBehaviour {

		private			int 		amountDamage = 1;
		public			bool 		damagePlayer, damageSlime, damageBat, damageZombie;

		//Detecta algo que entrou na área do colisor e permaneceu
		private void OnTriggerStay2D(Collider2D col){			
			if (col.gameObject.CompareTag ("Player") && damagePlayer) {
				if (damagePlayer) {
					StartCoroutine (col.gameObject.GetComponent<Player> ().Damage (amountDamage)); 			
				}			
			}
			else if (col.gameObject.CompareTag ("Slime") && damageSlime) {
				if (damageSlime) {
					StartCoroutine (col.gameObject.GetComponent<Slime> ().Damage (amountDamage));
				}
			}
			else if (col.gameObject.CompareTag ("Bat") && damageBat) {
				if (damageBat) {
					StartCoroutine (col.gameObject.GetComponent<Bat> ().Damage (amountDamage)); 
				}
			} 
			else if (col.gameObject.CompareTag ("Zombie") && damageZombie) {
				if (damageZombie) {
					StartCoroutine (col.gameObject.GetComponent<Zombie> ().Damage (amountDamage, transform)); 
				}
			}

		}

	}

}