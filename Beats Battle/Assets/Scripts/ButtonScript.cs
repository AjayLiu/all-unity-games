using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ButtonScript : MonoBehaviour {

    bool isP1;
    GameCon game;

	// Use this for initialization
	void Start () {
        game = Camera.main.GetComponent<GameCon>();
        isP1 = this.gameObject.tag == "P1 Button" ? true : false;
        gameObject.GetComponent<Image>().material = game.colors[int.Parse(this.gameObject.name)];           
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    

    public void ButtonPress() {

        if(isP1) {
            if(game.status == GameStatus.P1Place) {
                if (game.barCount < game.barLimit) {
                    Vector3 camPos = Camera.main.ScreenToWorldPoint(this.gameObject.transform.position);
                    GameObject ins = Instantiate(game.barPrefab, new Vector3(camPos.x, camPos.y, 0), Quaternion.identity);
                    game.bars.Add(ins);
                    game.barCount++;
                }
            }
            if(game.status == GameStatus.P1Receive) {

            }            
        } else {

        }        
    }
}
