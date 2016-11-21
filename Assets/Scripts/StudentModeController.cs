using UnityEngine;
using System.Collections;

public class StudentModeController : MonoBehaviour {

	private Player player;

	class Character {
		private int health;
		private Sprite characterSprite;
		private GameObject instance;
		public Character(Sprite sprite) {
			health = 100;
			characterSprite = sprite;
			instance = new GameObject("Hello World");
			instance.AddComponent<SpriteRenderer> ();
		}
		public void setHealth(int currentHealth) {
			health = currentHealth;
		}
		public int getHealth() {
			return health;
		}
		public void move() {
			
		}
		public void deactivate() {
			Destroy (instance);
		}
	}

	class Player : Character {
		public void 
	}

	class Quiz {
		
	}

	// Use this for initialization
	void Start () {
		Character character = new Character ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
