using UnityEngine;
using System;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using MySql.Data;
using MySql.Data.MySqlClient;

public class MySQLconnector : MonoBehaviour {

	public string database;
	public string host;
	public string user;
	public string password;

	private string connectionString;
	private MySqlConnection connection;
	private string query;
	private MySqlCommand command;
	private MySqlDataReader reader;

	void Awake () {
		DontDestroyOnLoad (this.gameObject);
		connectionString = "Server="+host+";Database="+database
			+";User="+user+";Password="+password+";Pooling=true";
		try {
			connection = new MySqlConnection(connectionString);
			connection.Open();
			Debug.Log("MySQL State: "+connection.State);
		} catch (Exception e) {
			Debug.Log (e);
		}
	}

	void OnApplicationQuit () {
		if (connection != null) {
			if (connection.State.ToString () != "Closed") {
				connection.Close();
				Debug.Log ("My SQL Connection Closed");
			}
		}
	}

	public string[] findItem (int itemID) {
		string[] item = new string[3];
		query = "SELECT * FROM item WHERE itemID =" + itemID;
		command = new MySqlCommand (query, connection);
		reader = command.ExecuteReader ();
		while (reader.Read()) {
			item [0] = reader.GetString (1);
			item [1] = reader.GetString (2);
			item [2] = reader.GetString (3);
		}
		reader.Close ();
		return item;
	} 

	public string[,] findTopicQuestions (int topicNo) {
		query = "SELECT question.questionID, question.questionText, " +
			"question.answer FROM question WHERE question._topicID =" + topicNo;
		command = new MySqlCommand (query, connection);
		reader = command.ExecuteReader();
		int count = 0;
		while (reader.Read ()) {
			count++;
		}
		string[,] questions = new string[count,4];
		count = 0;
		reader.Close ();
		reader = command.ExecuteReader ();
		while(reader.Read()) {
			questions[count,0] = reader.GetString(0);
			questions[count,1] = reader.GetString(1);
			questions[count,2] = reader.GetString(2);
			count++;
		}
		reader.Close ();
		for (int i = 0; i < count; i++) {
			questions [i, 3] = findDifficulty (questions[i,0]).ToString();
		}
		return questions;
	}
		
	public bool existCheck (string studentID) {
		query = "SELECT studentID FROM student;";
		command = new MySqlCommand (query, connection);
		reader = command.ExecuteReader();
		while (reader.Read ()) {
			if (studentID == reader.GetString(0)) {
				reader.Close ();
				return true;
			}
		}
		reader.Close ();
		return false;
	}

	public void newStudent (string studentID, string firstName, string lastName, string studentClass) {
		query = "INSERT INTO student (studentID, firstName, lastName, _classID) VALUES (" + studentID +", '"+ firstName +"', '"+ lastName +"', '" + studentClass +"');";
		command = new MySqlCommand (query, connection);
		reader = command.ExecuteReader ();
		reader.Close ();
	}

	public void newAnswerInstance (string studentID, bool correct, string questionID) {
		query = "INSERT INTO answerInstance (correct, _studentID, _questionID) VALUES ("+ correct +", "+ studentID +", "+ questionID +");";
		command = new MySqlCommand (query, connection);
		reader = command.ExecuteReader ();
		reader.Close ();
	}

	public float findDifficulty (string questionID) {
		float correct = 0;
		float total = 0;
		query = "SELECT correct FROM answerinstance WHERE _questionID = "+ questionID +";";
		command = new MySqlCommand (query, connection);
		reader = command.ExecuteReader();
		while (reader.Read ()) {
			if (reader.GetString (0) == "True") {
				correct++;
			}
			total++;
		}
		reader.Close ();
		if (total > 0) {
			return correct / total;
		} else {
			return 0;	
		}
	}

	public bool isAlevel (string playerID) {
		query = "SELECT student.studentID, class._examID FROM student INNER JOIN class ON student._classID = class.classID;";
		command = new MySqlCommand (query, connection);
		reader = command.ExecuteReader();
		while (reader.Read ()) {
			if (reader.GetString (0) == playerID) {
				if (reader.GetString(1) == "2") {
					reader.Close ();
					return true;
				}
			}
		}
		reader.Close ();
		return false;
	}
}
