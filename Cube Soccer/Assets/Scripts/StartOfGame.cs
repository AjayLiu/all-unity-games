using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StartOfGame : MonoBehaviour {

	public Text startText;

	public bool gameStart = false;

	public Transform redTransform, blueTransform;

	float timer;
	bool tickTimer = true;

	float cooldown1 = 1f;
	float cooldown2 = 2f;
	float cooldown3 = 3f;

	float randomX;
	float randomZ;

	Vector3 spawnPointForRed;
	Vector3 spawnPointForBlue;

	bool isPositioned = false;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if(tickTimer){
			timer += Time.deltaTime;
		}
		if(timer < cooldown1 && timer != 0){
			Vector2 midOfScreen = new Vector2 (Screen.width / 2, Screen.height / 2);
			startText.text =  "3";
			startText.rectTransform.position = midOfScreen;
		}
		if(timer >= cooldown1 && timer <= cooldown2){
			startText.text = "2";
		}
		if(timer >= cooldown2 && timer <= cooldown3){
			startText.text = "1";
		}

		if (timer >= cooldown3) {
			startText.text = "Start!";
			gameStart = true;
		}
		if (timer >= cooldown3 + 0.5f) {
			startText.text = " ";
			timer = 0;
			tickTimer = false;
		}


		//Randomize Spawn Point
		if (!isPositioned) {
			randomX = Random.Range (10f, 25f);
			randomZ = Random.Range (-13, 13);

			spawnPointForRed = new Vector3 (randomX, 2f, randomZ);
			spawnPointForBlue = new Vector3 (-randomX, 2f, -randomZ);

			redTransform.position = spawnPointForRed;
			blueTransform.position = spawnPointForBlue;

			isPositioned = true;
		}


	}
}
