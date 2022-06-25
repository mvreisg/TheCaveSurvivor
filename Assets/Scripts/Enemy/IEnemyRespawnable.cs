using UnityEngine;
using System.Collections;

namespace TheCaveSurvivor.Enemy{

	/*
	 * Interface: IEnemyRespawnable
	 * 
	 *	Interface para permitir renascimento ao inimigo, porque bosses não renascem,
	 *		portanto este método não pode estar na <<abstract>>Enemies
	 * 
	 * */
	public interface IEnemyRespawnable{
		void Respawn(float distance);
	}

}