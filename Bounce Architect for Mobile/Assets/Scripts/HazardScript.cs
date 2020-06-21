using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardScript : MonoBehaviour
{
    GameControllerScript game;

    // Start is called before the first frame update
    void Start() {
        game = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Ball") {
            game.OnHazard();
            Destroy(gameObject);
        }
    }
}
