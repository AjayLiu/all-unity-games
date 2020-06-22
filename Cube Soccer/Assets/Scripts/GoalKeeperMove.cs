using UnityEngine;
using System.Collections;

public class GoalKeeperMove : MonoBehaviour {

	float timer;

	float randomTimeSwap;
	float chooseSwapFrequency = 0.5f;

	float rnd;

	// Use this for initialization
	void Start () {
		rnd = Random.Range (0, 2);
	
		if (rnd == 0) {
			moveLeft = true;
		} else {
			moveLeft = false;
		}
	}

	float timerForSwap;

	// Update is called once per frame
	void Update () {
		
		timer += Time.deltaTime;
		timerForSwap += Time.deltaTime;

		if(timerForSwap >= chooseSwapFrequency){
			ChooseNewRandom ();
			timerForSwap = 0;
		}
		if(timer >= randomTimeSwap){
			SwapDirection();
			timer = 0;
		}


		if (moveLeft) {
			transform.Translate (Vector3.forward * 10 * Time.deltaTime);
		} else {
			transform.Translate (Vector3.back * 10 * Time.deltaTime);
		}
			
	}

	void ChooseNewRandom(){
		randomTimeSwap = Random.Range(0.8f, 2f);
	}

	bool moveLeft;

	void SwapDirection(){
		moveLeft = !moveLeft;
	}

	void OnCollisionEnter(Collision other){
		if(other.gameObject.CompareTag("GoalFrame")){
			SwapDirection ();
		}
	}
}
