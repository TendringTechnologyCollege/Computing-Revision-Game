using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class QuizController : MonoBehaviour {

	public MySQLconnector DatabaseScript; 
	public Text text;

	private string[,] questions;
	private string questionsString;

	void Start () {
		questions = DatabaseScript.FindTopicQuestions (7);
		questionsString = "";
		Debug.Log (questions.Length);
		for(int i=0;i<(questions.Length / 3);i++) {
			questionsString = questionsString + questions [i, 0] + ": ";
			questionsString = questionsString + questions [i, 1] + "\n";
			questionsString = questionsString + "A: " + questions [i, 2] + "\n";
		}
		text.text = questionsString;
	}

	void Update () {
	
	}
}
