using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinnerScript : MonoBehaviour {

    GameCon game;
    GameObject player;
    PlayerScript play;
    Rigidbody2D rd;

    // Use this for initialization
    void Start () {
        game = GameObject.Find("GAMECON").GetComponent<GameCon>();
        player = game.player;
        play = player.GetComponent<PlayerScript>();
        rd = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.CompareTag("Player")) {
            play.SpinnerHit(this.gameObject, col);
        }

        if (col.gameObject.CompareTag("Spinner")) {
            print("SPINNER AND SPINNER CONTACT ERROR");
        }
    }
}
