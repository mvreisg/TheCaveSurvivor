using UnityEngine;
using System.Collections;
using TheCaveSurvivor.Characters;

namespace TheCaveSurvivor.Enemy{

	/*
	 * Classe: <<Abstract>>
	 * 			 Enemies
	 * 
	 * 	Define comportamento para os inimigos do jogo
	 * 
	 * */

	public abstract class Enemies : MonoBehaviour {
			
		//Declaração de variáveis que serão utilizadas nas classes da herança, pelos inimigos.
		protected			Player					player;
		protected			GameObject				heartDrop, 
													potionDrop,
													lifeDrop,
													keyDrop,
													collectablesObject,
													enemyPrefab;

		protected			Material				lightMaterial;
		protected			Animator 				enemyAnims;
		protected			SpriteRenderer			enemySprite;
		protected			AudioSource 			enemyPlayer;
		protected			Rigidbody2D 			enemyRigid;
		protected			AudioClip[]				enemyClips;
		protected			Collider2D[]			enemyCollider;
		protected			Vector3					startPosition;

		protected			const float				distanceToSpawn = 5f,
													lootMinValue = 0f, 
													lootMaxValue = 10f, 
													lootMaxItemWait = 5f,
													deathWaitTime = 0.75f;

		protected			float					distanceToMove, 									
													moveForce, 
													moveSpeed,
													jumpForce, 
													hitWaitTime;

		protected			int 					healthPoints,													
													enemyHit,
													playerLayer,
													enemiesLayer, 
													roofLayer, 
													groundLayer,
													waterLayer;


		protected			bool					canMove,
													canTakeDamage,
													enemyDied;

		public				bool					destroyEnemy;


		protected abstract void Update ();
		protected abstract void Move(float distance);
		protected abstract void Hit(int hit);
		public abstract IEnumerator Damage(int damage);
		protected abstract IEnumerator Die ();
		protected abstract IEnumerator Loot (Vector3 posToSpawnLoot);

		protected virtual void Start(){

			//Dá acesso ao script do Player
			player = GameObject.FindObjectOfType<Player> ();

			//Carregamento de Prefabs
			heartDrop = Resources.Load ("Prefabs/Items/Heart") as GameObject;
			lifeDrop = Resources.Load ("Prefabs/Items/Life") as GameObject;
			potionDrop = Resources.Load ("Prefabs/Items/Potion") as GameObject;
			keyDrop = Resources.Load<GameObject> ("Prefabs/Items/Key") as GameObject;
			collectablesObject = GameObject.Find ("Collectables");

			//Material para o shader
			lightMaterial = Resources.Load ("Materials/Light") as Material;

			//Declaração das layers
			groundLayer = LayerMask.NameToLayer("Ground");
			enemiesLayer = LayerMask.NameToLayer("Enemies");
			waterLayer = LayerMask.NameToLayer("Water");
			roofLayer = LayerMask.NameToLayer("Rooftop");
			playerLayer = LayerMask.NameToLayer("Player");

			//Ignorar colisão com outros inimigos
			Physics2D.IgnoreLayerCollision (enemiesLayer, enemiesLayer);

		}

		//Método para controlar movimento
		protected virtual IEnumerator CanMoveCondition(float timeToWait){ 
			canMove = false;
			yield return new WaitForSecondsRealtime (timeToWait);
			canMove = true;
		}

		//Método para calcular distância entre o inimigo e o player
		protected virtual float CalculateDistance(){
			float playerdistance = Vector2.Distance(player.transform.position, transform.position);
			return playerdistance;
		}

	}

}