using UnityEngine;
using System.Collections;

public class StudentModeController : MonoBehaviour {

	private Player player;

	class Character {
		private int health;
		private Sprite characterSprite;
		private GameObject instance;
		public void newCharacter(Sprite sprite) {
			health = 100;
			characterSprite = sprite;
			instance;

		}
	}

	class Player {

	}

	class Quiz {
		
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
