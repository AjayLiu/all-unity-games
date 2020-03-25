using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarScript : MonoBehaviour {

    GameCon game;

    public bool isInZone = false;

	// Use this for initialization
	void Start () {
        game = Camera.main.GetComponent<GameCon>();
    }

    // Update is called once per frame
    void Update () {
        transform.Translate(Vector2.up * game.barSpeed * Time.deltaTime);
	}
}
