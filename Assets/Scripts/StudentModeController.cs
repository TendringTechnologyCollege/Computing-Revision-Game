using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class StudentModeController : MonoBehaviour {

	public Sprite playerSprite;						// Passed to the Script: Player

	public Sprite[] mapSprites;						// Passed to the Script: Game Control
	public Sprite emptySprite;
	public GameObject[] map;

	public Sprite[] enemySprites; 					// Passed to the Script: Quizzes

	public Sprite[] itemSprites; 					// Passed to the Script: Items
	public GameObject itemOutlinePrefab;

	public MySQLconnector databaseScript; 			// Passed to the Script: Database

	public InputField inputField; 					// Passed to the Script: UI 
	public Text questionText;
	public Image quizBackground;
	public Slider playerSilder;
	public Image playerBackground;
	public Text playerHealthText;
	public Slider enemySilder;
	public Image enemyBackground;
	public Text enemyHealthText;
	public Image attackOverhead;
	public Image defenceOverhead;
	public Image healthOverhead;
	public Image scoreOverhead;
	public Text attackOverheadText;
	public Text defenceOverheadText;
	public Text healthOverheadText;
	public Text scoreOverheadText;
	public Image popupBackground;
	public Image popupItem;
	public Text popupDetails;
	public Text popupText;



	private Player player; 							// Created by the script: Player
	private string playerID;
	private bool playerIsALevel;
	private int playerHealth;
	private int playerAttack;
	private int playerDefence;
	private int score;

	private bool enter;								// Created by the script: Game control								 
	private bool space;
	private bool escape;
	private bool move;
	private bool gameOver;
	private int[] gridPosition;
	private bool[] directions;
	private Camera camera;
	private int mapQuizzes;
	private int mapItems;

	private Quiz[,] quizArray;						// Created by the script: Quizzes
	private string[,] questions;
	private int enemyHealth;

	private Item[,] itemArray; 						// Created by the script: Items
	private Item[] inventory;
	private GameObject itemOutline;
	private int[] importantItems;
	private Item[] equipped;

	class Object {
		private Sprite objectSprite;
		private GameObject instance;
		private Rigidbody rigidbody;
		private SpriteRenderer spriteRenderer;
		private string objectName;
		public Object (Sprite sprite, string name) {									//Constructor for New Character
			objectName = name;
			objectSprite = sprite;
			instance = new GameObject(objectName);										//Creates a new game object for the character
			instance.AddComponent<SpriteRenderer> ();
			instance.AddComponent<Rigidbody> ();
			rigidbody = instance.GetComponent<Rigidbody>();
			rigidbody.isKinematic = true;
			rigidbody.useGravity = false;
			spriteRenderer =  instance.GetComponent<SpriteRenderer>();
			spriteRenderer.sprite = objectSprite;
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
		public Sprite getSprite() {
			return objectSprite;
		}
		public string getName() {
			return objectName;
		}
	}

	class Player : Object {																//Player class inheriting character
		private int health;
		private int attack;
		private int defence;
		private Sprite hatSprite;
		private Sprite topSprite;
		private Sprite weaponSprite;
		private SpriteRenderer hatRenderer;
		private SpriteRenderer topRenderer;
		private SpriteRenderer weaponRenderer;
		private GameObject equipped;
		private GameObject hat;
		private GameObject top;
		private GameObject weapon;
		public Player (Sprite sprite, string name) : base(sprite, name) {
			health = 100;
			attack = 0;
			defence = 0;
			equipped = new GameObject("Equipped");
			hat = new GameObject("Hat");
			hat.AddComponent<SpriteRenderer> ();
			hatRenderer = hat.GetComponent<SpriteRenderer>();
			hatRenderer.sortingLayerName = "Equipped Items";
			hat.transform.parent = equipped.transform;
			hat.transform.position = new Vector3 (0, 0.11f, 0f);
			top = new GameObject("Top");
			top.AddComponent<SpriteRenderer> ();
			topRenderer = top.GetComponent<SpriteRenderer>();
			topRenderer.sortingLayerName = "Equipped Items";
			top.transform.parent = equipped.transform;
			top.transform.position = new Vector3 (0.015f,-0.13f, 0f);
			weapon = new GameObject("Weapon");
			weapon.AddComponent<SpriteRenderer> ();
			weaponRenderer = weapon.GetComponent<SpriteRenderer>();
			weaponRenderer.sortingLayerName = "Equipped Items";
			weaponRenderer.sortingOrder = 1;
			weapon.transform.localScale = new Vector3 (0.6f,0.6f,1f);
			weapon.transform.parent = equipped.transform;
			weapon.transform.position = new Vector3 (0.19f, -0.1f, 0f);
		}
		public void move (float x, float y) {
			base.move (x,y);
			equipped.transform.position = new Vector3 (x,y,0f);
		}
		public void scaler (float scale) {
			base.scaler (scale);
			equipped.transform.localScale = new Vector3 (scale, scale, 1f);
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
		public void setHat (Sprite sprite) {
			hatRenderer.sprite = sprite;
		}
		public void setTop (Sprite sprite) {
			topRenderer.sprite = sprite;
		}
		public void setWeapon (Sprite sprite) {
			weaponRenderer.sprite = sprite;
		}
	}

	class Quiz : Object {
		private int topicNumber;
		public Quiz (int _topicNumber, Sprite sprite, string name) : base (sprite, name) {
			topicNumber = _topicNumber;
		}
		public int getTopicNumber () {
			return topicNumber;
		}
	}

	class Item : Object {
		private int effect;
		private int type;
		public Item (int _effect, int _type, Sprite sprite, string name) : base (sprite, name) {
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
		score = 0;

		enter = false;							// Initialise: Game control 
		space = false;
		escape = false;
		move = false;
		gridPosition = new int[2];
		directions = new bool[4];
		gameOver = false;
		camera = this.GetComponent<Camera> ();
		toggleUI (false);
		togglePopupUI (false);

		quizArray = new Quiz[5,5];				// Initialise: Quizzes

		itemArray = new Item[5,5]; 				// Initialise: Items
		inventory = new Item[16];
		importantItems = new int[4];
		importantItems [0] = 0;
		equipped = new Item[3];
		itemOutline = Instantiate (itemOutlinePrefab, new Vector3 (8.25f, 3.5f, 0f), Quaternion.identity) as GameObject;

		StartCoroutine (gameLoop());
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.Return)) {
			enter = true;
		}
		if (Input.GetKeyDown (KeyCode.Space)) {
			space = true;
		}
		if (Input.GetKeyDown (KeyCode.Escape)) {
			escape = true;
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
				inventory [i].move (count%4 + 8.25f, 3.5f - count/4*1f);
				if (i == importantItems[0]) {
					itemOutline.transform.position = new Vector3 (count%4 + 8.25f, 3.5f - count/4*1f, 0f);
				}
				count++;
			}
		}
		for (int i = 0; i < equipped.Length; i++) {
			if (equipped [i] != null) {
				equipped [i].move (7f, 3.5f - i * 1.5f);
			}
		}
		attackOverheadText.text = "Attack: " + playerAttack.ToString();
		defenceOverheadText.text = "Defence: " + playerDefence.ToString();
		healthOverheadText.text = "Health: " + playerHealth.ToString();
		scoreOverheadText.text = "Score: " + score; 
	}

	public IEnumerator gameLoop () {
		player = new Player (playerSprite,"Player");
		gridPosition = new int[2] {0,0};
		directions = new bool[4] { false, false, false, false };
		populate ();
		yield return StartCoroutine (login());
		while (!gameOver) {
			int x = gridPosition [0];
			int y = gridPosition [1];
			if (quizArray [x,y] != null) {
				zoomIn (x, y, 0.375f);
				quizArray [x, y].scaler (0.5f);
				quizArray [x, y].move (x+0.2f,y+0.2f);
				player.scaler (0.5f);
				player.move (x-0.2f,y);
				int topicNumber = quizArray [x, y].getTopicNumber();
				yield return StartCoroutine (runQuiz (topicNumber));
				zoomOut ();
				quizArray [x, y].destroy ();
				quizArray [x, y] = null;
				player.scaler (1f);
				player.move (x, y);
				mapQuizzes--;
			} 
			if (itemArray [gridPosition [0],gridPosition [1]] != null) {
				while (inventorySpace () == false) {
					togglePopupUI (true);
					popupItem.sprite = itemArray [gridPosition [0], gridPosition [1]].getSprite ();
					string name = itemArray [gridPosition [0], gridPosition [1]].getName ();
					string effect = itemArray [gridPosition [0], gridPosition [1]].getEffect ().ToString();
					escape = false;
					space = false;
					while (escape == false && space == false) {
						yield return null;
					}
					if (escape) {
						zoomIn (9, 2, 2.5f);
						yield return StartCoroutine (runInventory ());
						zoomOut ();
					} else if (space) {
						itemArray [gridPosition [0], gridPosition [1]].destroy ();
						itemArray [gridPosition [0], gridPosition [1]] = null;
						break;
					}
					yield return null;
				}
				togglePopupUI (false);
				for (int i=0; i<inventory.Length; i++) {
					if (inventory[i] == null) {
						inventory [i] = itemArray [x, y];
						break;
					}
				}
				itemArray [x, y] = null;
				mapItems--;
			}
			if (mapQuizzes == 0 && mapItems == 0) {
				populate ();
			}
			directions = new bool[4] { false, false, false, false };
			escape = false;
			move = false;
			while (move == false) {
				if (escape) {
					zoomIn (9, 2, 2.5f);
					yield return StartCoroutine (runInventory());
					zoomOut ();
					escape = false;
				}
				if (directions[0] || directions[1] || directions[2] || directions[3]) {
					yield return StartCoroutine (mover ());
				}
				yield return null;
			}
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
		SpriteRenderer sRenderer;
		mapQuizzes = 0;
		mapItems = 0;
		for (int i=0;i<5;i++) {
			for (int j=0;j<5;j++) {
				randomNumber = Random.Range (1,12);
				if (randomNumber > 6) {
					int topic = randomNumber - 6;
					randomNumber = Random.Range (0, enemySprites.Length);
					quizArray [i, j] = new Quiz (topic, enemySprites [randomNumber], "Enemy");
					quizArray [i, j].scaler (0.6f);
					quizArray [i, j].move (i, j + 0.2f);
					randomNumber = Random.Range (0, mapSprites.Length);
					sRenderer = map [i + j * 5].GetComponent <SpriteRenderer> ();
					sRenderer.sprite = mapSprites [randomNumber];
					mapQuizzes++;
				} else {
					sRenderer = map [i + j * 5].GetComponent <SpriteRenderer> ();
					sRenderer.sprite = emptySprite;
				}
				int numberOfItems = databaseScript.findNumberOfItems ();
				randomNumber = Random.Range (1,numberOfItems*2);
				if (randomNumber > numberOfItems) {
					string[] newItem = databaseScript.findItem (randomNumber - numberOfItems);
					Sprite itemSprite = null;
					for (int k=0; k<itemSprites.Length; k++) {
						if (itemSprites [k].name == newItem [0] + "_0") {
							itemSprite = itemSprites [k];
						}
					}
					itemArray [i, j] = new Item (int.Parse(newItem[1]), int.Parse(newItem[2]), itemSprite, newItem[0]);
					itemArray [i, j].scaler (0.6f);
					itemArray [i, j].move (i, j-0.2f);
					mapItems++;
				}
			}
		}
		if (quizArray [x, y] != null) {
			quizArray [x, y].destroy ();
			quizArray [x, y] = null;
			mapQuizzes--;
		}
		if (itemArray [x, y] != null) {
			itemArray [x, y].destroy ();
			itemArray [x, y] = null;
			mapItems--;
		}
		randomNumber = Random.Range (0, mapSprites.Length);
		sRenderer = map[x + y * 5].GetComponent<SpriteRenderer>();
		sRenderer.sprite = mapSprites [randomNumber];
	}

	public IEnumerator mover() {
		int x = gridPosition [0];
		int y = gridPosition [1];
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
		attackOverhead.gameObject.SetActive (!state);
		defenceOverhead.gameObject.SetActive (!state);
		healthOverhead.gameObject.SetActive (!state);
		attackOverheadText.gameObject.SetActive (!state);
		defenceOverheadText.gameObject.SetActive (!state);
		healthOverheadText.gameObject.SetActive (!state);
	}

	public void togglePopupUI (bool state) {
		popupBackground.gameObject.SetActive (state);
		popupItem.gameObject.SetActive (state);
		popupDetails.gameObject.SetActive (state);
		popupText.gameObject.SetActive (state);
	}

	public void zoomIn(int x, int y, float size) {
		camera.orthographicSize = size;
		camera.transform.position = new Vector3 (x, y, -10);
	} 

	public void zoomOut() {
		camera.orthographicSize = 2.7f;
		camera.transform.position = new Vector3 (2,2.23f,-10);
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
			while (enter == false) {
				yield return null;
			}
			if (inputField.text.ToUpper() == questions [index , 2]) {
				questionText.text = "Correct";
				enemyHealth = enemyHealth - 10 - playerAttack;
				enemySilder.value = enemyHealth;
				enemyHealthText.text = enemyHealth.ToString() + " / 100";
				databaseScript.newAnswerInstance (playerID, true, questions[index,0]);
				score++;
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
		escape = false;
		enter = false;
		space = false;
		directions = new bool[4] { false, false, false, false };
		while (escape == false) {
			if (directions[0] || directions[1] || directions[2] || directions[3]) {
				move = false;
				yield return StartCoroutine (inventoryMover ());
				move = false;
				directions = new bool[4] { false, false, false, false };
			}
			if (space) {
				inventory [importantItems [0]].destroy ();
				inventory [importantItems [0]] = null;
				importantItems [0] = inventoryFoucusAdjust (importantItems [0]);
				space = false;
			}
			if (enter) {
				if (importantItems[0] != null) {
					if (inventory [importantItems [0]].getType () == 4) {
						player.setHealth (playerHealth + inventory [importantItems [0]].getEffect ());
						inventory [importantItems [0]].destroy ();
						inventory [importantItems [0]] = null; 
					} else if (inventory [importantItems [0]].getType () == 1) {
						Item temp = equipped [0];
						equipped [0] = inventory [importantItems [0]];
						inventory[importantItems [0]] = temp;
						player.setHat (equipped[0].getSprite());
					} else if (inventory [importantItems [0]].getType () == 2) {
						Item temp = equipped [1];
						equipped [1] = inventory [importantItems [0]];
						player.setDefence (inventory[importantItems[0]].getEffect());
						inventory [importantItems [0]] = temp;
						player.setTop (equipped[1].getSprite());
					} else if (inventory [importantItems [0]].getType () == 3) {
						Item temp = equipped [2];
						equipped [2] = inventory [importantItems [0]];
						player.setAttack (inventory[importantItems[0]].getEffect());
						inventory [importantItems [0]] = temp;
						player.setWeapon (equipped [2].getSprite ());
					}
					if (importantItems != null) {
						importantItems [0] = inventoryFoucusAdjust (importantItems [0]);
					}
				}
				enter = false;
			}
			yield return null;	
		}
	}

	public int inventoryFoucusAdjust (int current) {
		for (int i = current; i < inventory.Length; i++) {
			if (inventory[i] != null && i != current) {
				return i;
			}
		}
		for (int i = current; i > 0; i--) {
			if (inventory [i] != null && i != current) {
				return i;
			}
		}
		return 0;
	}

	public bool inventorySpace() {
		bool space = false;
		for (int i = 0; i < inventory.Length; i++) {
			if (inventory [i] == null) {
				space = true;
			}
		}
		return space;
	}

	public IEnumerator inventoryMover() {
		if (directions [0] == true) {
			int count = 0;
			for (int i = importantItems [0]; i >= 0; i--) {
				if (inventory [i] != null) {
					count++;
					if (count == 5) {
						importantItems [0] = i;
					}
				}
			}
		} else if (directions [1] == true) {
			int count = 0;
			for (int i = importantItems [0]; i < inventory.Length; i++) {
				if (inventory [i] != null) {
					count++;
					if (count == 5) {
						importantItems [0] = i;
					}
				}
			}
		} else if (directions [2] == true && importantItems [0] > 0) {
			int count = 0;
			for (int i = importantItems [0]; i >= 0; i--) {
				if (inventory [i] != null) {
					count++;
					if (count == 2) {
						importantItems [0] = i;
					}
				}
			}
		} else if (directions [3] == true) {
			int count = 0;
			for (int i = importantItems [0]; i < inventory.Length; i++) {
				if (inventory [i] != null) {
					count++;
					if (count == 2) {
						importantItems [0] = i;
					}
				}
			}
		}
		yield return null;
	}
}