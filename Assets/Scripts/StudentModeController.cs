using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StudentModeController : MonoBehaviour {

	public Sprite playerSprite;

	private Player player;
	private bool enter; 																// Boolean Key Variables for interactivity 
	private bool space;
	private bool iKey;
	private bool move;
	private bool gameOver;
	private Quiz[,] quizArray;															//Arrays for populating the game map with items and quizzes
	private int[,] itemArray;
	private int[] gridPosition; 														//Arrays for movement
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
		public void setHealth(int currentHealth) {										//Setter and Getter for Health
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
		public Quiz (int seed) {
			topicNumber = seed;
		}
	}
		
	void Start () {
		enter = false;
		space = false;
		iKey = false;
		move = false;
		gameOver = false;
		quizArray = new Quiz[5,5];
		itemArray = new int[5,5];
		gridPosition = new int[2];
		directions = new bool[4];
		StartCoroutine (gameLoop());
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.Return)) {
			enter = true;
		}
		if (Input.GetKeyDown (KeyCode.Space)) {
			space = true;
		}
		if (Input.GetKeyDown (KeyCode.I)) {
			iKey = true;
		}
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			directions [0] = true;
		}
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			directions[1] = true;
		}
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			directions[2] = true;
		}
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			directions[3] = true;
		}
	}

	public IEnumerator gameLoop () {
		player = new Player (playerSprite,"Player");
		gridPosition = new int[2] {0,0};
		directions = new bool[4] { false, false, false, false };
		int randomNumber;
		for (int i=0;i<5;i++) {
			for (int j=0;j<5;j++) {
				randomNumber = Random.Range (1,12);
				if (randomNumber > 6) {
					quizArray [i,j] = new Quiz (randomNumber - 6);
				}
				randomNumber = Random.Range (0,30);
				if (randomNumber > 9) {
					itemArray [i,j] = randomNumber - 10;
				}
			}
		}
		quizArray [0,0] = null;
		while (!gameOver) {
			if (quizArray [gridPosition [0],gridPosition [1]] != null) {
				SceneManager.LoadScene ("Scene_Quiz", LoadSceneMode.Additive);
			} else if (itemArray [gridPosition [0],gridPosition [1]] != 0) {
				
			} else if (iKey) {
				SceneManager.LoadScene ("Scene_Inventory", LoadSceneMode.Additive);
				iKey = false;
			}
			yield return StartCoroutine (mover ());
			yield return null;
		}
		yield return null;
	}

	public IEnumerator mover() {
		int x = gridPosition [0];
		int y = gridPosition [1];
		directions = new bool[4] {false,false,false,false};
		move = false;
		while (move == false) {
			if (directions [0] == true && gridPosition [1] < 4) {
				gridPosition [0] = x;
				gridPosition [1] = y + 1;
				move = true;
			} else if (directions [1] == true && gridPosition [1] > 0) {
				gridPosition [0] = x;
				gridPosition [1] = y - 1;
				move = true;
			} else if (directions [2] == true && gridPosition [0] > 0) {
				gridPosition [0] = x - 1;
				gridPosition [1] = y;
				move = true;
			} else if (directions [3] == true && gridPosition [0] < 4) {
				gridPosition [0] = x + 1;
				gridPosition [1] = y;
				move = true;
			}
			yield return null;
		}
		player.move(gridPosition[0],gridPosition[1]);
		yield return null;
	}
}