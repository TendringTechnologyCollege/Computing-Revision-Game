using UnityEngine;
using System.Collections;

public class MainMenuController : MonoBehaviour {

	public StudentModeController studentScript;

	private bool Up;
	private bool Down;

	void Start () {
		Up = false;
		Down = false;
		StartCoroutine (inputLoop ());
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			Up = true;
		}
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			Down = true;
		}
	}

	public IEnumerator inputLoop() {
		while (true) {
			if (Up) {
				yield return StartCoroutine (studentScript.gameLoop());
			}
		}
	}
}
