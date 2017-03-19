using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuestionsController : MonoBehaviour {

	public MySQLconnector databaseScript;
	public GameObject[] questionsDisplay;
	public Text topicNumberText;
	public Image questionOutline;

	public Image inputFormBackground;
	public InputField topicField;
	public Text topicText;
	public InputField questionField;
	public Text questionText;
	public InputField answerField;
	public Text answerText;
	public Text confirmText;
	public Text abandonText;
	public Image AYSBackground;
	public Text AYSText;
	public Text AYSQuestion;

	private bool up;						//Background
	private bool down;						
	private bool left;
	private bool right;
	private bool enter;
	private bool escape;
	private bool a;
	private Question[] questions;
	private int topicNumber;

	private Row[] rows;						//Display
	private int focusRow;
	private int displayStartIndex;

	class Question {
		private string questionID;
		private string questionText;
		private string questionAnswer;
		private string questionDifficulty;
		public Question (string ID, string text, string answer, string difficulty) {
			questionID = ID;
			questionText = text;
			questionAnswer = answer;
			questionDifficulty = difficulty;
		}
		public string getID() {
			return questionID;
		} 
		public string getText() {
			return questionText;
		}
		public string getAnswer() {
			return questionAnswer;
		}
		public string getDifficulty() {
			return questionDifficulty;
		}
	}

	class Row {
		private Text rowID;
		private Text rowQuestion;
		private Text rowAnswer;
		private Text rowDifficulty;
		public Row (GameObject rowParent) {
			rowID = rowParent.transform.Find ("ID Text").GetComponent<Text> ();
			rowQuestion = rowParent.transform.Find ("Question Text").GetComponent<Text> ();
			rowAnswer = rowParent.transform.Find ("Answer Text").GetComponent<Text> ();
			rowDifficulty = rowParent.transform.Find ("Difficulty").GetComponent<Text> ();
		}
		public void setID (string ID) {
			rowID.text = ID;
		}
		public void setQuestion (string question) {
			rowQuestion.text = question;
		}
		public void setAnswer (string answer) {
			rowAnswer.text = answer;
		}
		public void setDifficulty (string difficulty) {
			rowDifficulty.text = difficulty;
		}
		public string getID () {
			return rowID.text;
		}
		public string getQuestion () {
			return rowQuestion.text;
		}
		public string getAnswer () {
			return rowAnswer.text;
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
		topicNumber = 1;
		displayStartIndex = 0;
		rows = new Row[questionsDisplay.Length];
		focusRow = 0;
		for (int i=0; i<questionsDisplay.Length; i++) {
			rows [i] = new Row (questionsDisplay[i]);
		}
		toggleInputUI (false);
		toggleAYSUI (false);
		StartCoroutine (RunLoop());
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
		topicNumberText.text = "Topic Name: "+ databaseScript.findTopicName(topicNumber);
		questionOutline.rectTransform.localPosition = new Vector3 (0f, 170f-focusRow*45, 0f);
	}

	public void Populate () {
		string[,] topicQuestions = databaseScript.findTopicQuestions (topicNumber);
		questions = new Question[topicQuestions.Length / 4];
		for (int i=0; i<questions.Length; i++) {
			questions [i] = new Question (topicQuestions [i, 0], topicQuestions [i, 1],
				topicQuestions [i, 2], topicQuestions [i, 3]);
		}
		for (int i=0; i<rows.Length; i++) {
			if (i + displayStartIndex < questions.Length) {
				rows [i].setID (questions [i+displayStartIndex].getID ());
				rows [i].setQuestion (questions [i+displayStartIndex].getText ());
				rows [i].setAnswer (questions [i+displayStartIndex].getAnswer ());
				rows [i].setDifficulty (questions [i+displayStartIndex].getDifficulty ());	
			} else {
				rows [i].setID (string.Empty);
				rows [i].setQuestion (string.Empty);
				rows [i].setAnswer (string.Empty);
				rows [i].setDifficulty (string.Empty);
			}
		}
	}

	public IEnumerator RunLoop () {
		Populate ();
		while (true) {
			if (left == true || right == true) {
				if (left == true && topicNumber > 1) {
					topicNumber--;
					displayStartIndex = 0;
					focusRow = 0;
					Populate ();
				}
				if (right == true && topicNumber < 12) {
					topicNumber++;
					displayStartIndex = 0;
					focusRow = 0;
					Populate ();
				}
				left = false;
				right = false;
			}
			if (up == true) {
				if (focusRow > 0) {
					focusRow--;
				} else if (displayStartIndex > 0) {
					displayStartIndex--;
					Populate ();
				}
				up = false;
			}
			if (down == true) {
				if (focusRow < rows.Length - 1 && focusRow < questions.Length - displayStartIndex - 1) {
					focusRow++;
				} else if (displayStartIndex < questions.Length - rows.Length) {
					displayStartIndex++;
					Populate ();
				}
				down = false;
			}
			if (escape == true) {
				if (rows[focusRow].getID() != "") {
					yield return StartCoroutine(runAYS ());
					Populate ();
				}
				escape = false;
			}
			if (enter == true) {
				if (rows [focusRow].getID () != "") {
					toggleInputUI (true);
					topicField.text = topicNumber.ToString();
					questionField.text = rows [focusRow].getQuestion ();
					answerField.text = rows [focusRow].getAnswer ();
					enter = false;
					escape = false;
					while (true) {
						if (enter) {
							databaseScript.updateQuestion (questionField.text, answerField.text, topicField.text, rows [focusRow].getID());
							Populate ();
							break;
						}
						if (escape) {
							break;
						}
						yield return null;
					}
					toggleInputUI (false);
				}
				enter = false;
				escape = false;
				a = false;
			}
			if (a) {
				toggleInputUI (true);
				topicField.text = string.Empty;
				questionField.text = string.Empty;
				answerField.text = string.Empty;
				enter = false;
				escape = false;
				while (true) {
					if (enter) {
						databaseScript.addQuestion (questionField.text, answerField.text, topicField.text);
						Populate ();
						break;
					}
					if (escape) {
						break;
					}
					yield return null;
				}
				toggleInputUI (false);
				enter = false;
				escape = false;
				a = false;
			}
			yield return null;
		}
		yield return null;
	}

	public IEnumerator runAYS () {
		toggleAYSUI (true);
		enter = false;
		escape = false;
		AYSQuestion.text = rows [focusRow].getQuestion ();
		while (true) {
			up = false;
			down = false;
			left = false;
			right = false;
			if (enter == true) {
				Debug.Log (rows[focusRow].getID());
				databaseScript.deleteQuestion (rows [focusRow].getID());
				break;
			}
			if (escape == true) {
				break;
			}
			yield return null;
		}
		enter = false;
		escape = false;
		toggleAYSUI (false);
		yield return null;
	}

	public void toggleInputUI (bool state) {
		inputFormBackground.gameObject.SetActive(state);
		topicField.gameObject.SetActive(state);
		topicText.gameObject.SetActive(state);
		topicField.text = string.Empty;
		questionField.gameObject.SetActive(state);
		questionText.gameObject.SetActive(state);
		questionField.text = string.Empty;
		answerField.gameObject.SetActive(state);
		answerText.gameObject.SetActive(state);
		answerField.text = string.Empty;
		confirmText.gameObject.SetActive(state);
		abandonText.gameObject.SetActive(state);
	}

	public void toggleAYSUI (bool state) {
		AYSBackground.gameObject.SetActive (state);
		AYSText.gameObject.SetActive (state);
		AYSQuestion.gameObject.SetActive (state);
	}
}
