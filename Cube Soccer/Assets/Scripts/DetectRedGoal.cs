using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DetectRedGoal : MonoBehaviour {

	public ScoreManager scoreManager;
	
	bool hasCollided = false;

	void OnTriggerEnter(Collider other){

		if(other.CompareTag("SoccerBall") && !hasCollided){
			hasCollided = true;
			ScoreManager.blueScore++;
			scoreManager.BlueGoal();
			ScoreManager.goal = true;
		}
	}
}

