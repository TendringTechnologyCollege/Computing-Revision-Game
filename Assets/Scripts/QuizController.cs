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
		questions = DatabaseScript.FindTopicQuestions (2);
		questionsString = "";
		text.text = questions [0, 1];
		//for(int i=0;i<questions.Length;i++) {
		//	questionsString = questionsString + questions [i, 1];
		//}
		//text.text = questionsString;
	}

	void Update () {
	
	}
}
