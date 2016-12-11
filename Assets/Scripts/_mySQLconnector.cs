﻿using UnityEngine;
using System;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using MySql.Data;
using MySql.Data.MySqlClient;

public class _mySQLconnector : MonoBehaviour {

	public string database;
	public string host;
	public string user;
	public string password;
	public bool pooling = true;

	private string connectionString;
	private MySqlConnection con = null;
	private MySqlCommand cmd = null;
	private MySqlDataReader rdr = null;

	void Awake () {
		DontDestroyOnLoad (this.gameObject);
		connectionString = "Server="+host+";Database="+database+";User="+user+";Password="+password+";Pooling=";
		if (pooling) {
			connectionString += "true;";
		} else {
			connectionString += "false;";
		}
		try {
			con = new MySqlConnection(connectionString);
			con.Open();
			Debug.Log("MySQL State: "+con.State);
		} catch (Exception e) {
			Debug.Log (e);
		}
	}

	void OnApplicationQuit () {
		if (con != null) {
			if (con.State.ToString () != "Closed") {
				con.Close();
				Debug.Log ("My SQL Connection Closed");
			}
		}
	}

	public string GetConnectionState () {
		return con.State.ToString();
	}
}
