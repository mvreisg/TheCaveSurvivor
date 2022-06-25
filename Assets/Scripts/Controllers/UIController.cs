using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace TheCaveSurvivor.Controllers{

	/*
	 * Classe: UIController
	 * 
	 * 	Classe que contém métodos de interface de usuário, como texto, imagem e fading
	 * 
	 * */

	public class UIController : MonoBehaviour {

		private	static		UIController 		instance;
		private static 		GameObject 			UIFather;
		private	static		Font				UIFont;

		private void Awake(){
			instance = this;
			UIFather = GameObject.Find ("UI");
			UIFont = Resources.Load<Font> ("Fonts/PixellettersFull");
		}

		public static UIController GetClass{
			get{
				return instance;
			}
			private set{ }
		}
		public static GameObject GetUIFather{
			get{
				return UIFather;
			}
			private set{ }
		}
		public static Font GetUIFont{
			get{
				return UIFont;
			}
			private set{ }
		}

		//Pisca um texto na tela
		public IEnumerator Blink_Text(Text text, bool infinite){			
			text.enabled = true;
			yield return new WaitForSeconds (2f);
			text.enabled = false;
			yield return new WaitForSeconds (1f);
			if (infinite) { 
				StartCoroutine(Blink_Text (text, infinite));			
			}
		}

		public IEnumerator Blink_Text(Text text, bool infinite, float fixedTime){			
			text.enabled = !text.enabled;
			yield return new WaitForSeconds (fixedTime);
			if (infinite) { 
				StartCoroutine(Blink_Text (text, infinite));			
			}
		}

		//Fade in na tela, cria uma imagem
		public IEnumerator Fade_InScene(float speed){
			float alpha = 1f;
			Image fadeImage = Image_Generate (GameCamera.GetClass.GetCamera.rect.center, new Vector2(Screen.width, Screen.height), new Vector2(Screen.width, Screen.height));
			fadeImage.color = new Color(0f, 0f, 0f, alpha);
			while (fadeImage.color.a >= 0f) {
				alpha -= speed * Time.deltaTime;
				fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);
				yield return null;
			}
		}

		//Fade in na tela, recebe uma imagem
		public IEnumerator Fade_InScene(float speed, Image image){
			float alpha = 1f;
			Image fadeImage;
			fadeImage = image;
			fadeImage.color = new Color(0f, 0f, 0f, alpha);
			while (fadeImage.color.a >= 0f) {
				alpha -= speed * Time.deltaTime;
				fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);
				yield return null;
			}

		}

		//Fade out na tela, cria uma imagem
		public IEnumerator Fade_OutScene(float speed){
			float alpha = 0f;
			Image fadeImage = Image_Generate (GameCamera.GetClass.GetCamera.rect.center, new Vector2(Screen.width, Screen.height), new Vector2(Screen.width, Screen.height));
			fadeImage.color = new Color(0f, 0f, 0f, alpha);
			while (fadeImage.color.a < 1f) {
				alpha += speed * Time.deltaTime;
				fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);
				yield return null;
			}
		}

		//Fade out na tela, recebe uma imagem
		public IEnumerator Fade_OutScene(float speed, Image image){
			float alpha = 0f;
			Image fadeImage;
			fadeImage = image;
			fadeImage.color = new Color(0f, 0f, 0f, alpha);
			while (fadeImage.color.a < 1f) {
				alpha += speed * Time.deltaTime;
				fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);
				yield return null;
			}
		}

		public Text Text_Generate(Vector3 localPosition, Vector3 localScale, Vector2 sizeDelta){
			GameObject objectPorter = new GameObject ();
			objectPorter.transform.SetParent (UIFather.transform);
			objectPorter.name = "UIText" + gameObject.GetInstanceID().ToString();
			Text newText = objectPorter.AddComponent<Text> ();
			RectTransform newTextRect = newText.gameObject.GetComponent<RectTransform> ();
			newTextRect.localPosition = localPosition;
			newTextRect.localScale = localScale;
			newTextRect.sizeDelta = sizeDelta;
			return newText;
		}

		public Text Text_Generate(Vector3 localPosition, Vector3 localScale, Vector3 sizeDelta, string message, Color color, int fontSize, TextAnchor anchor){
			GameObject objectPorter = new GameObject ();
			objectPorter.transform.SetParent (UIFather.transform);
			objectPorter.name = "UIText" + gameObject.GetInstanceID().ToString();
			Text newText = objectPorter.AddComponent<Text> ();
			newText.text = message;
			newText.color = color;
			newText.font = UIFont;
			newText.fontSize = fontSize;
			newText.alignment = anchor;
			RectTransform newTextRect = newText.gameObject.GetComponent<RectTransform> ();
			newTextRect.localPosition = localPosition;
			newTextRect.localScale = localScale;
			newTextRect.sizeDelta = sizeDelta;
			return newText;
		}			

		public Image Image_Generate(Vector3 localPosition, Vector3 localScale, Vector3 sizeDelta){
			GameObject objectPorter = new GameObject ();
			objectPorter.transform.SetParent (UIFather.transform);
			objectPorter.name = "UIImage" + gameObject.GetInstanceID().ToString();
			Image newImage = objectPorter.AddComponent<Image> ();
			RectTransform newImageRect = newImage.gameObject.GetComponent<RectTransform> ();
			newImageRect.localPosition = localPosition;
			newImageRect.localScale = localScale;
			newImageRect.sizeDelta = sizeDelta;
			return newImage;
		}

	}

}