using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleScript : MonoBehaviour {


    GameCon game;

    // Update is called once per frame
    int i = 0;
	void Update () {
        if (i > 2)
            game = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameCon>();
        i++;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.CompareTag("Player")) {
			game.ObstacleHit();
		}
	}
}
