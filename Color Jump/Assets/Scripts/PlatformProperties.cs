using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformProperties : MonoBehaviour {
    [HideInInspector]public bool hasBeenTouched = false;
    public bool isMove;
    public bool isCracked;

    GameScript game;

    public Color platformColor;

    Collider col;
    
    void Start() {
        col = GetComponent<Collider>();
        game = GameObject.Find("GAME").GetComponent<GameScript>();

        if (isCracked) {
            SpriteRenderer spr = GetComponentInChildren<SpriteRenderer>();
            spr.sprite = game.crackedBlockLeft;

            GameObject ins = Instantiate(spr.gameObject, new Vector3(transform.position.x, transform.position.y - 0.1f, 0f), Quaternion.identity, transform);
            ins.GetComponent<SpriteRenderer>().sprite = game.crackedBlockRight;
        }
    }

    void Update() {
		if(transform.position.y < GameScript.screenBottom)
			Destroy(this.gameObject);
		if(platformColor != new Color(0,0,0,0) && PlayerScript.playerColor != platformColor && game.isStar == false)
			DisableCollider();

		if(isMove) {
			Move();
		}
        
    }

    public void DisableCollider() {
        col.enabled = false;
    }
    public void EnableCollider() {
        if (PlayerScript.playerColor == platformColor || platformColor == new Color(0, 0, 0, 0) || game.isStar == true)
            col.enabled = true;
    }

    bool moveRight = true;
    void Move() {
   		if(transform.position.x - GameScript.screenLeft < 0) {
			moveRight = true;
		}
		if(GameScript.screenRight - transform.position.x < 0) {
			moveRight = false;
		}

		if(moveRight) {
			Vector3 v;
			v = new Vector3(transform.position.x + Time.deltaTime * game.platformMoveSpeed,transform.position.y);			
			transform.position = v;
		} else {
			Vector3 v;
			v = new Vector3(transform.position.x - Time.deltaTime * game.platformMoveSpeed,transform.position.y);			
			transform.position = v;
		}
		
	}

    public void BreakPlatform() {
        Destroy(this.gameObject);
    }

}
