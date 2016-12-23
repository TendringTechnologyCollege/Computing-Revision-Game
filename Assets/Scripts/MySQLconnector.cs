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

	public string[,] findTopicQuestions (int topicNo) {
		query = "SELECT question.questionID, question.questionText, " +
			"question.answer FROM question WHERE question._topicID =" + topicNo;
		command = new MySqlCommand (query, connection);
		reader = command.ExecuteReader();
		int count = 0;
		while (reader.Read ()) {
			count++;
		}
		string[,] questions = new string[count,3];
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
		return questions;
	}
}
