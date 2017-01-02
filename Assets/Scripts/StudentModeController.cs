using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class StudentModeController : MonoBehaviour {

	public Sprite playerSprite;
	public Material playerMaterial;
	public Sprite enemySprite;
	public Material enemyMaterial;
	public Sprite itemSprite;
	public Material itemMaterial;
	public MySQLconnector databaseScript;
	public InputField inputField;
	public Text questionText;
	public Image quizBackground;
	public Slider playerSilder;
	public Image playerBackground;
	public Text playerHealthText;
	public Slider enemySilder;
	public Image enemyBackground;
	public Text enemyHealthText;

	private Player player;
	private string playerID;
	private bool playerIsALevel;
	private bool enter; 																// Boolean Key Variables for interactivity 
	private bool space;
	private bool iKey;
	private bool move;
	private bool gameOver;
	private Quiz[,] quizArray;															//Arrays for populating the game map with items and quizzes
	private Item[,] itemArray;
	private int[] gridPosition; 														//Arrays for movement
	private bool[] directions;
	private string[,] questions;
	private Camera camera;
	private int playerHealth;
	private int playerAttack;
	private int playerDefence;
	private int enemyHealth;
	private Item[] inventory;

	class Object {
		private Sprite objectSprite;
		private Material objectMaterial;
		private GameObject instance;
		private Rigidbody rigidbody;
		private SpriteRenderer spriteRenderer;
		public Object (Sprite sprite, string name, Material material) {				//Constructor for New Character
			objectSprite = sprite;
			objectMaterial = material;
			instance = new GameObject(name);											//Creates a new game object for the character
			instance.AddComponent<SpriteRenderer> ();
			instance.AddComponent<Rigidbody> ();
			rigidbody = instance.GetComponent<Rigidbody>();
			rigidbody.isKinematic = true;
			rigidbody.useGravity = false;
			spriteRenderer =  instance.GetComponent<SpriteRenderer>();
			spriteRenderer.sprite = objectSprite;
			spriteRenderer.material = objectMaterial;
		}
		public void move(float x, float y) {
			rigidbody.position = new Vector3 (x, y, 0f);
		}
		public void toggleActive(bool state) {
			instance.SetActive (state);
		}
		public void scaler(float scale) {
			instance.transform.localScale = new Vector3 (scale, scale, 1);
		}
		public void destroy() {
			Destroy (instance);
		}
	}

	class Player : Object {																//Player class inheriting character
		private int health;
		private int attack;
		private int defence;
		public Player (Sprite sprite, string name, Material material) : base(sprite,name, material) {
			health = 100;
			attack = 0;
			defence = 0;
		}
		public void setHealth(int currentHealth) {										//Setter and Getter for Health
			health = currentHealth;
		}
		public int getHealth() {
			return health;
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

	class Quiz : Object {
		private int topicNumber;
		public Quiz (int _topicNumber, Sprite sprite, string name, Material material) : base (sprite, name, material) {
			topicNumber = _topicNumber;
		}
		public int getTopicNumber () {
			return topicNumber;
		}
	}

	class Item : Object {
		private int effect;
		private int type;
		public Item (int _effect, int _type, Sprite sprite, string name, Material material) : base (sprite, name, material) {
			effect = _effect;
			type = _type;
		}
		public int getEffect () {
			return effect;
		}
		public int getType () {
			return type;
		}
	}
		
	void Start () {
		enter = false;
		space = false;
		iKey = false;
		move = false;
		gameOver = false;
		quizArray = new Quiz[5,5];
		itemArray = new Item[5,5];
		gridPosition = new int[2];
		directions = new bool[4];
		inventory = new Item[16];
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
		playerHealth = player.getHealth();
		playerAttack = player.getAttack ();
		playerDefence = player.getDefence ();
		playerHealthText.text = playerHealth.ToString() + " / 100";
		playerSilder.value = playerHealth;
		int count = 0;
		for (int i=0;i < inventory.Length; i++) {
			if (inventory [i] != null) {
				inventory [i].move (count%4 + 7.25f, 3.5f - count/4*1f);
				count++;
			}
		}
	}

	public IEnumerator gameLoop () {
		player = new Player (playerSprite,"Player",playerMaterial);
		gridPosition = new int[2] {0,0};
		directions = new bool[4] { false, false, false, false };
		populate ();
		yield return StartCoroutine (login());
		while (!gameOver) {
			int x = gridPosition [0];
			int y = gridPosition [1];
			if (quizArray [x,y] != null) {
				zoomIn (x, y);
				quizArray [x, y].scaler (0.3f);
				quizArray [x, y].move (x+0.3f,y+0.3f);
				player.scaler (0.3f);
				player.move (x-0.3f,y-0.05f);
				int topicNumber = quizArray [x, y].getTopicNumber();
				yield return StartCoroutine (runQuiz (topicNumber));
				zoomOut ();
				quizArray [x, y].destroy ();
				quizArray [x, y] = null;
				player.scaler (1f);
				player.move (x, y);
			} 
			if (itemArray [gridPosition [0],gridPosition [1]] != null) {
				for (int i=0; i<inventory.Length; i++) {
					if (inventory[i] == null) {
						inventory [i] = itemArray [x, y];
						break;
					}
				}
				itemArray [x, y] = null;
			}
			if (iKey) {
				zoomIn (8, 2);
				yield return StartCoroutine (runInventory());
				zoomOut ();
			}
			yield return StartCoroutine (mover ());
			yield return null;
		}
		yield return null;
	}

	public IEnumerator login () {
		questionText.gameObject.SetActive (true);
		inputField.gameObject.SetActive (true);
		quizBackground.gameObject.SetActive (true);
		enter = false;
		questionText.text = "Please enter you candidate number: ";
		while (enter == false) {
			yield return null;
		}
		playerID = inputField.text;
		if (databaseScript.existCheck (playerID) == false) {
			questionText.text = "You are a new user, what class are you in?: ";
			enter = false;
			while (enter == false) {
				yield return null;
			}
			string playerClass = inputField.text;
			questionText.text = "What is your first name?: ";
			enter = false;
			while (enter == false) {
				yield return null;
			}
			string playerFirstName = inputField.text;
			questionText.text = "What is your last name?: ";
			enter = false;
			while (enter == false) {
				yield return null;
			}
			string playerLastName = inputField.text;
			databaseScript.newStudent (playerID, playerFirstName, playerLastName, playerClass);
		}
		playerIsALevel = databaseScript.isAlevel(playerID);
		questionText.gameObject.SetActive (false);
		inputField.gameObject.SetActive (false);
		quizBackground.gameObject.SetActive (false);
	}

	public void populate () {
		int randomNumber;
		int x = gridPosition [0];
		int y = gridPosition [1];
		for (int i=0;i<5;i++) {
			for (int j=0;j<5;j++) {
				randomNumber = Random.Range (1,12);
				if (randomNumber > 6) {
					quizArray [i, j] = new Quiz (randomNumber - 6, enemySprite, "Enemy", enemyMaterial);
					quizArray [i, j].scaler (0.3f);
					quizArray [i, j].move (i,j+0.2f);
				}
				randomNumber = Random.Range (1,34);
				if (randomNumber > 17) {
					string[] newItem = databaseScript.findItem (randomNumber - 17);
					itemArray [i, j] = new Item (int.Parse(newItem[1]), int.Parse(newItem[2]), itemSprite, newItem[0], itemMaterial);
					itemArray [i, j].scaler (0.3f);
					itemArray [i, j].move (i, j-0.2f);
				}
			}
		}
		if (quizArray [x, y] != null) {
			quizArray [x, y].destroy ();
			quizArray [x, y] = null;
		}
		if (itemArray [x, y] != null) {
			itemArray [x, y].destroy ();
			itemArray [x, y] = null;
		}
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
		enemyBackground.gameObject.SetActive (state);
		enemySilder.gameObject.SetActive (state);
		enemyHealthText.gameObject.SetActive (state);
	}

	public void zoomIn(int x, int y) {
		camera.orthographicSize = 0.5f;
		camera.transform.position = new Vector3 (x, y, -10);
	} 

	public void zoomOut() {
		camera.orthographicSize = 2.5f;
		camera.transform.position = new Vector3 (2,2,-10);
	}

	public void inventoryZoom () {
		
	}

	public IEnumerator runQuiz(int topicNumber) {
		enemyHealth = 100;
		if (playerIsALevel == true) {
			questions = databaseScript.findTopicQuestions (topicNumber + 6);	
		} else {
			questions = databaseScript.findTopicQuestions (topicNumber);
		}
		int noQuestions = questions.Length / 4;
		int count = 0;
		toggleUI (true);
		enemySilder.value = enemyHealth;
		enemyHealthText.text = enemyHealth.ToString() + " / 100";
		int index = 0;
		float difficulty;
		bool[] askedQuestions = new bool[noQuestions];
		while (playerHealth > 0 && count < noQuestions && enemyHealth > 0) {
			difficulty = 1;
			for (int i = 0; i < noQuestions; i++) {
				if (float.Parse(questions[i, 3]) <= difficulty && askedQuestions[i] != true) {
					index = i;
					difficulty = float.Parse(questions [i, 3]);
				}
			} 
			askedQuestions [index] = true;
			questionText.text = questions [index, 1];
			enter = false;
			Debug.Log(questions[index, 1] + " " + questions[index, 3]);
			while (enter == false) {
				yield return null;
			}
			if (inputField.text.ToUpper() == questions [index , 2]) {
				questionText.text = "Correct";
				enemyHealth = enemyHealth - 10 - playerAttack;
				enemySilder.value = enemyHealth;
				enemyHealthText.text = enemyHealth.ToString() + " / 100";
				databaseScript.newAnswerInstance (playerID, true, questions[index,0]);
			} else {
				questionText.text = "Incorrect";
				player.setHealth (playerHealth + playerDefence - 10);
				databaseScript.newAnswerInstance (playerID, false, questions[index,0]);
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

	public IEnumerator runInventory () {
		yield return null;
	}
}