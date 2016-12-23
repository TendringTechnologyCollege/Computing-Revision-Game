using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class StudentModeController : MonoBehaviour {

	public Sprite playerSprite;
	public MySQLconnector databaseScript;
	public InputField inputField;
	public Text questionText;
	public Image quizBackground;
	public Slider playerSilder;
	public Image playerBackground;
	public Text playerHealthText;

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
	private string[,] questions;
	private Camera camera;

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
		private Character enemy;
		private int topicNumber;
		public Quiz (int _topicNumber) {
			topicNumber = _topicNumber;
		}
		public int getTopicNumber () {
			return topicNumber;
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
		camera = this.GetComponent<Camera> ();
		toggleUI (false);
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
		playerHealthText.text = player.getHealth ().ToString();
		playerSilder.value = player.getHealth ();
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
			int x = gridPosition [0];
			int y = gridPosition [1];
			if (quizArray [x,y] != null) {
				zoomIn (x, y);
				int topicNumber = quizArray [x, y].getTopicNumber();
				yield return StartCoroutine (runQuiz (topicNumber));
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

	public void toggleUI (bool state) {
		inputField.gameObject.SetActive (state);
		questionText.gameObject.SetActive (state);
		quizBackground.gameObject.SetActive (state);
		playerBackground.gameObject.SetActive (state);
		playerSilder.gameObject.SetActive (state);
		playerHealthText.gameObject.SetActive (state);
	}

	public void zoomIn(int x, int y) {
		camera.orthographicSize = 0.5f;
		camera.transform.position = new Vector3 (x,y,-10);
	} 

	public IEnumerator runQuiz(int topicNumber) {
		questions = databaseScript.findTopicQuestions (11);
		int noQuestions = questions.Length / 3;
		int count = 0;
		int playerHealth;
		int playerDefence;
		toggleUI (true);
		while (player.getHealth() > 0 && count < noQuestions) {
			questionText.text = questions [count, 1];
			enter = false;
			while (enter == false) {
				yield return null;
			}
			if (inputField.text.ToUpper() == questions [count, 2]) {
				questionText.text = "Correct";
			} else {
				questionText.text = "Incorrect";
				playerHealth = player.getHealth();
				playerDefence = player.getDefence ();
				player.setHealth (playerHealth + playerDefence - 10);
			}
			inputField.text = string.Empty;
			enter = false;
			while (enter == false) {
				yield return null;
			}
			count++;
			yield return null;
		}
		toggleUI (false);
		yield return null;
	}
}