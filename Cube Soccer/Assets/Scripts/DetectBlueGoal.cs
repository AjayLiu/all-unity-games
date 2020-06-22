using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DetectBlueGoal : MonoBehaviour {

	public ScoreManager scoreManager;

	bool hasCollided = false;

	void OnTriggerEnter(Collider other){

		if(other.CompareTag("SoccerBall") && !hasCollided){
			hasCollided = true;
			ScoreManager.redScore++;
			scoreManager.RedGoal();
			ScoreManager.goal = true;
		}
	}
}
