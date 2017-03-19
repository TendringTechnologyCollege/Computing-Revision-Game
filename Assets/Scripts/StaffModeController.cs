using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class StaffModeController : MonoBehaviour {

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
				SceneManager.LoadScene ("Scene_Questions", LoadSceneMode.Single);
				break;
			} else if (Down) {
				Down = false;
				SceneManager.LoadScene ("Scene_Results", LoadSceneMode.Single);
				break;
			}
			yield return null;
		}
	}
}