using UnityEngine;
using System.Collections;

public class StudentModeController : MonoBehaviour {

	public Sprite playerSprite;

	private Player player;
	private bool enter; 
	private bool space;
	private bool iKey;
	private bool move;
	private bool gameOver;
	private Quiz[][] quizArray;
	private int[][] itemArray;
	private int[] gridPosition;
	private bool[] directions;

	class Character {
		private int health;
		private Sprite characterSprite;
		private GameObject instance;
		private Rigidbody rigidbody;
		private SpriteRenderer spriteRenderer;
		public Character(Sprite sprite, string name) { 									//Constructor for New Character
			health = 100;
			characterSprite = sprite;
			instance = new GameObject(name);											//Creates a new game object for the character
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

	class Player : Character {															//Player class inheriting character
		private int attack;
		private int defence;
		public Player (Sprite sprite, string name) : base(sprite,name) {
			attack = 0;
			defence = 0;
		}
		public void setAttack(int newAttack) {
			attack = newAttack;
		}
		public int getAttack() {
			return attack;
		}
		public void setDefence(int newDefence) {
			defence = newDefence;
		}
		public int getDefence() {
			return defence;
		}
	}

	class Quiz {
		public Character enemy;
		public int topicNumber;
	}
		
	void Start () {
		enter = false;
		space = false;
		iKey = false;
		move = false;
		gameOver = false;
		quizArray = new Quiz[5] [5];
		itemArray = new int[5] [5];
		gridPosition = new int[2];
		directions = new bool[4];
		StartCoroutine (gameLoop());
	}

	void Update () {
	
	}

	public IEnumerator gameLoop () {
		Player player = new Player (playerSprite,"Joe");
		gridPosition = new int[2] {0,0};
		directions = new bool[4] { false, false, false, false };
		int randomNumber;
		for (int i=0;i<5;i++) {
			for (int j=0;j<5;j++) {
				randomNumber = Random.Range (0,20);
				if (randomNumber > 9) {
					quizArray [i] [j] = new Quiz (randomNumber - 10);
				}
				randomNumber = Random.Range (0,30);
				if (randomNumber > 9) {
					quizArray [i] [j] = new Quiz (randomNumber - 10);
				}
			}
		}
		return null;
	}
}