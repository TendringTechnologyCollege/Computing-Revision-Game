using UnityEngine;
using System.Collections;

public class QuizController : MonoBehaviour {

	void Start () {
		GameObject MySQLobject = GameObject.FindGameObjectWithTag ("MySQLconnector");
		_mySQLconnector MySQLscript = MySQLobject.GetComponent<_mySQLconnector> ();
	}

	void Update () {
	
	}
}
