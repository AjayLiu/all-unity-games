using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarScript : MonoBehaviour {

    GameScript game;

    void Start() {
        game = GameObject.Find("GAME").GetComponent<GameScript>();
    }

    void Update() {
        if (transform.position.y < GameScript.screenBottom)
            Destroy(this.gameObject);
    }

	void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player")) {
            game.UseStar();
            Destroy(this.gameObject);
        }
    }
}
