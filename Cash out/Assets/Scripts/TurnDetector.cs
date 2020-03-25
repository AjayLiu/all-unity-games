using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnDetector : MonoBehaviour
{
    public bool isLeft = true;
    GameControllerScript game;

    // Start is called before the first frame update
    void Start()
    {
        game = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other){
        if(other.tag == "Player") {            
            game.IntersectionTurn(isLeft, transform.position);            
        }
        
    }
}
