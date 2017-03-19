using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResultsController : MonoBehaviour {

	public MySQLconnector databaseScript;
	public GameObject[] display;
	public Image questionOutline;

	private bool up;						//Background
	private bool down;						
	private bool left;
	private bool right;
	private bool enter;
	private bool escape;
	private bool a;
	private DataItem[] dataItems;
	private int topicNumber;

	private Row[] rows;						//Display
	private int focusRow;
	private int displayStartIndex;

	class DataItem {
		private string ItemName;
		private string ItemData;
		public DataItem (string name, string data) {
			ItemName = name;
			ItemData = data;
		}
		public string getName() {
			return ItemName;
		} 
		public string getData() {
			return ItemData;
		}
	}

	class Row {
		private Text rowName;
		private Text rowData;
		public Row (GameObject rowParent) {
			rowName = rowParent.transform.Find ("Name Text").GetComponent<Text> ();
			rowData = rowParent.transform.Find ("Data Text").GetComponent<Text> ();
		}
		public void setName (string name) {
			rowName.text = name;
		}
		public void setData (string data) {
			rowData.text = data;
		}
		public string getID () {
			return rowName.text;
		}
		public string getQuestion () {
			return rowData.text;
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
		rows = new Row[display.Length];
		focusRow = 0;
		for (int i=0; i<display.Length; i++) {
			rows [i] = new Row (display[i]);
		}
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
		questionOutline.rectTransform.localPosition = new Vector3 (0f, 170f-focusRow*45, 0f);
	}

	public void Populate () {
		string[,] topicQuestions = databaseScript.findTopicQuestions (topicNumber);
		dataItems = new DataItem[topicQuestions.Length / 4];
		for (int i=0; i<dataItems.Length; i++) {
			dataItems [i] = new DataItem (topicQuestions [i, 1], topicQuestions [i, 3]);
		}
		for (int i=0; i<rows.Length; i++) {
			if (i + displayStartIndex < dataItems.Length) {
				rows [i].setName (dataItems [i+displayStartIndex].getName ());
				rows [i].setData (dataItems [i+displayStartIndex].getData ());
			} else {
				rows [i].setName (string.Empty);
				rows [i].setData (string.Empty);
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
				if (focusRow < rows.Length - 1 && focusRow < dataItems.Length - displayStartIndex - 1) {
					focusRow++;
				} else if (displayStartIndex < dataItems.Length - rows.Length) {
					displayStartIndex++;
					Populate ();
				}
				down = false;
			}
			yield return null;
		}
		yield return null;
	}
}