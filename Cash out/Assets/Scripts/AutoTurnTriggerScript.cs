using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoTurnTriggerScript : MonoBehaviour{
    PlayerScript play;

    // Start is called before the first frame update
    void Start() {
        play = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    void Update() {

    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            play.AutoTurn();
        }

    }
}
