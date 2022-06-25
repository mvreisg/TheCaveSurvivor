using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TheCaveSurvivor.Characters;

namespace TheCaveSurvivor.Controllers{

	/*
	 * Classe: GameCamera
	 * 
	 * 	Contém métodos para controle da camera no jogo
	 * 
	 * */

	public class GameCamera : MonoBehaviour {

		private static			GameCamera				instance;
		private					Player 					player;
		private					Camera					camera;
		private 				Plane[]					planes;
		private					Vector3 				newCameraPosition;

		private const			float 					cameraZPosition = -10f,
														maxDifference = 1.32f;
		private					float 					normalMovementSpeed = 5f;
		private					bool					movingOnAxis = false,
														lockCameraX = false, 
														lockCameraY = false;

		private void Awake(){
			instance = this;
			camera = GetComponent<Camera> ();
		}
			
		//Acesso a instancia
		public static GameCamera GetClass{
			get{ 
				return instance; 
			}
		}

		//Acesso a camera
		public Camera GetCamera{
			get{ 
				return camera; 
			}
		}

		//Acesso ao plano da camera (Matriz4x4 da tela)
		public Plane[] GetPlanes{
			get{ 
				return planes; 
			}
		}

		//Acesso ao lock X
		public bool LockCameraX{
			get{ 
				return lockCameraX; 
			}
		}

		//Acesso ao lock Y
		public bool LockCameraY{
			get{ 
				return lockCameraY; 
			}
		}

		//Acesso ao lock total
		public bool LockedAll{
			get{
				return lockCameraX && lockCameraY;
			}
		}

		private void Start () {
			if (GameObject.FindObjectOfType<Player> () != null) {
				player = GameObject.FindObjectOfType<Player> ();
				transform.position = new Vector3 (player.transform.position.x, transform.position.y, transform.position.z);
			}
		}

		//Move a câmera de acordo com a posição do jogador no plano espaço
		private void Update () {
			if (GameObject.FindObjectOfType<Player> () != null) {				
				if (!LockCameraX || !LockCameraY) {
					newCameraPosition.x = (LockCameraX) ? (transform.position.x) : (player.transform.position.x);
					newCameraPosition.y = (LockCameraY) ? (transform.position.y) : (player.transform.position.y + (maxDifference * 0.5f));
					transform.position = Vector3.LerpUnclamped (transform.position, newCameraPosition, normalMovementSpeed * Time.deltaTime);
				}
				transform.position = new Vector3 (transform.position.x, transform.position.y, cameraZPosition);
			}
			planes = GeometryUtility.CalculateFrustumPlanes (camera);
		}

		//Trava e destrava a camera no eixo X
		public void LockXAxis(){
			lockCameraX = true;
		}
		public void UnlockXAxis(){
			lockCameraX = false;
		}
			
		//Trava e destrava a camera no eixo Y
		public void LockYAxis(){
			lockCameraY = true;
		}
		public void UnlockYAxis(){
			lockCameraY = false;
		}
			
		//Trava e destrava a camera no eixo X e Y
		public void LockCamera(){
			lockCameraX = true;
			lockCameraY = true;
		}
		public void UnlockCamera(){
			lockCameraX = false;
			lockCameraY = false;
		}

		//Alinha a câmera, movendo a para um ponto no plano espaço
		public void AlignCameraToPoint(Vector3 pointOfReference){
			pointOfReference.z = cameraZPosition;
			transform.position = Vector2.LerpUnclamped (transform.position, pointOfReference, Time.deltaTime);
		}

		//Trava o eixo X e Y da camera e move para um lugar específico
		public void LockCameraAndMoveToSpecificPosition(Vector3 specPos){
			LockCamera ();
			specPos.z = cameraZPosition;
			transform.position = Vector3.LerpUnclamped (transform.position, specPos, Time.deltaTime);
		}

		//Trava o eixo X e Y da camera e move para um lugar específico com base em uma velocidade
		public void LockCameraAndMoveToSpecificPosition(Vector3 specPos, float speed){
			LockCamera ();
			specPos.z = cameraZPosition;
			transform.position = Vector3.LerpUnclamped (transform.position, specPos, Time.deltaTime * speed);
		}				

	}

}