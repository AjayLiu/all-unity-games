using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarScript : MonoBehaviour {

	GameCon game;
	
	// Use this for initialization
	int i = 0;
	void Update () {
		if (i > 2)
            game = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameCon>();
        i++;
	}

	void OnTriggerEnter2D(Collider2D other) {

		if(other.gameObject.CompareTag("Player")){
			game.StarCollected();
			Destroy(this.gameObject);
		}

	}
}
