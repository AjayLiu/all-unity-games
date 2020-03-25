using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBlockScript : MonoBehaviour {

    public float speed;
    public int block_hp;
    GameControllerScript game;
    GameObject player;

    TextMesh textMesh;

    // Use this for initialization
    void Start () {
        player = GameObject.Find("Player");
        game = GameObject.Find("GAME_CONTROLLER").GetComponent<GameControllerScript>();
        SetText();
	}
	
	// Update is called once per frame
	void Update () {
        MoveToPlayer();
        SetColor();
        SetText();
	}

    void SetText() {
        textMesh = GetComponentInChildren<TextMesh>();
        textMesh.text = block_hp.ToString();
        textMesh.fontStyle = FontStyle.Bold;
    }

    void MoveToPlayer() {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, step);
        
        if(block_hp <= 0) {
            game.OnEnemyKilled();
            PlayerScript.player_hp += 10;
            Destroy(this.gameObject);
        }
    }

    void DamagePlayer() {
        PlayerScript.player_hp -= block_hp;
      player.GetComponent<PlayerScript>().onhit();
    } 


    void SetColor() {
        GetComponent<Renderer>().material.color = GameControllerScript.colors[block_hp / GameControllerScript.hpToColorRatio];
    }


    void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Player") {
            DamagePlayer();
            Destroy(this.gameObject);
        }
    }
}
