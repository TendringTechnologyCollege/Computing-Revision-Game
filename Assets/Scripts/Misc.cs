using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Misc : MonoBehaviour {

	public MySQLconnector databaseScript; 
	public Sprite barSprite;
	public GameObject canvas;
	public Font font;

	private bool up;
	private bool down;						
	private bool left;
	private bool right;
	private bool enter;
	private bool escape;
	private bool a;

	class Bar {
		private GameObject barBackground;
		private GameObject barTextObject;
		private Image barImageComponent;
		private Text barTextComponent;
		private RectTransform backgroundTransform;
		private RectTransform textTransform;
		private float barMagnitude;
		public Bar (Sprite sprite, GameObject parent, string text, Font font, string difficulty) {									
			barBackground = new GameObject("BarBackground");										
			barBackground.AddComponent<Image> ();
			barImageComponent =  barBackground.GetComponent<Image>();
			barImageComponent.sprite = sprite;
			barImageComponent.transform.parent = parent.transform;
			backgroundTransform = barBackground.GetComponent<RectTransform> ();
			barMagnitude = float.Parse(difficulty);
			backgroundTransform.sizeDelta = new Vector2 (10,466*barMagnitude);
			barTextObject = new GameObject ("BarText");
			barTextObject.AddComponent<Text> ();
			barTextComponent = barTextObject.GetComponent<Text>();
			barTextComponent.text = text;
			barTextComponent.font = font;
			barTextComponent.color = Color.black;
			barTextObject.transform.parent = parent.transform;
			barTextObject.transform.rotation  = Quaternion.AngleAxis(90, Vector3.forward);
			textTransform = barTextObject.GetComponent<RectTransform> ();
			textTransform.sizeDelta = new Vector2 (466*barMagnitude, 20);
		}
		public void move(float x, float y) {
			barBackground.transform.position = new Vector3 (x, y, 0f);
			barTextObject.transform.position = new Vector3 (x, y, 0f);
		}
		public void scaler(float scale) {
			barBackground.transform.localScale = new Vector3 (scale, scale, 1);
			barTextObject.transform.localScale = new Vector3 (scale, scale, 1);
		}
		public void destroy() {
			Destroy (barBackground);
			Destroy (barTextObject);
		}
	}

	void Start () {
		up = false;
		down = false;
		left = false;
		right = false;
		enter = false;
		escape = false;
		a = false;
		StartCoroutine (runLoop());
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			up = true;
		}
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			down = true;
		}
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			left = true;
		}
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			right = true;
		}
		if (Input.GetKeyDown (KeyCode.Return)) {
			enter = true;
		}
		if (Input.GetKeyDown (KeyCode.Escape)) {
			escape = true;
		}
		if (Input.GetKeyDown (KeyCode.A)) {
			a = true;
		}
	}

	void Populate() {
		string[,]  results = databaseScript.gatherResults();
		Bar[] bars = new Bar[results.Length / 4];
		for (int i=0; i<results.Length / 4; i++) {
			bars[i] = new Bar (barSprite, canvas, results[i,0], font, "0.5");
		}
	}

	public IEnumerator runLoop () {
		Populate ();
		while (true) {
			yield return null;
		}
	}
}
