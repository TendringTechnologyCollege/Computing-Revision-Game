using UnityEngine;
using System.Collections;

public class StudentModeController : MonoBehaviour {

	public Sprite playerSprite;

	private Player player;

	class Character {
		private int health;
		private Sprite characterSprite;
		private GameObject instance;
		private Rigidbody rigidbody;
		private SpriteRenderer spriteRenderer;
		public Character(Sprite sprite, string name) {
			health = 100;
			characterSprite = sprite;
			instance = new GameObject(name);
			instance.AddComponent<SpriteRenderer> ();
			instance.AddComponent<Rigidbody> ();
			rigidbody = instance.GetComponent<Rigidbody>();
			rigidbody.isKinematic = true;
			rigidbody.useGravity = false;
			spriteRenderer =  instance.GetComponent<SpriteRenderer>();
			spriteRenderer.sprite = characterSprite;
		}
		public void setHealth(int currentHealth) {
			health = currentHealth;
		}
		public int getHealth() {
			return health;
		}
		public void move(float x, float y) {
			rigidbody.position = new Vector3 (x, y, 0f);
		}
		public void deactivate() {
			instance.SetActive (false);
		}
	}

	class Player : Character {
		public Player (Sprite sprite, string name) : base(sprite,name) {
			
		}
	}

	class Quiz {
		
	}

	// Use this for initialization
	void Start () {
		Player player = new Player (playerSprite,"Joe");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
