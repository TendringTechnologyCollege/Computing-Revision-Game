using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuController : MonoBehaviour {

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
				Up = false;
				SceneManager.LoadScene ("Scene_StudentMode", LoadSceneMode.Single);
				break;
			} else if (Down) {
				Down = false;
				SceneManager.LoadScene ("Scene_StaffMode", LoadSceneMode.Single);
				break;
			}
			yield return null;
		}
	}
}