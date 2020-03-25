using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStates{
    game, dialogue
}

public class GameControl : MonoBehaviour {

    public GameStates gamestate;

    public DialogueScript dialogueScript;

    GameObject player;
    PlayerControl play;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        play = player.GetComponent<PlayerControl>();
        SetGameState(gamestate);

    }
	
	// Update is called once per frame
	void Update () {
    }

    public void SetGameState(GameStates newState){
        gamestate = newState;
        switch(newState){
            case GameStates.game:
                SetDialogue(false);                
                break;

            case GameStates.dialogue:
                SetDialogue(true);
                break;

            default:
                break;
        }
    }

    public void SetDialogue(bool on) {
        if (on) {
            Time.timeScale = 0;
            dialogueScript.transform.parent.gameObject.SetActive(true);
            dialogueScript.GoToLine(0);
        } else {
            Time.timeScale = 1;
            dialogueScript.transform.parent.gameObject.SetActive(false);
        }
    }
}
