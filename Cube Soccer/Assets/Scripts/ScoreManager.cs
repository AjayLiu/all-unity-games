using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {
	
	[SerializeField]Text goalText;
	public Text scoreText;

	[SerializeField]static public int redScore;
	[SerializeField]static public int blueScore;

	int goalsScored;

	static public bool goal = false;

	float timer;

	float cooldown1 = 1f;
	float cooldown3 = 3f;

	string playerColor;

	void Update(){

		if (goal) {
			timer += Time.deltaTime;
		}

		if(timer < cooldown1 && timer != 0){
			Vector2 midOfScreen = new Vector2 (Screen.width / 2, Screen.height / 2);
			goalText.text = playerColor + " Scored!";
			goalText.rectTransform.position = midOfScreen;
		}
		if (timer >= cooldown3) {
			goal = false;
			timer = 0;
			SceneManager.LoadScene("CubeSoccerGame");
		}


		goalsScored = redScore + blueScore;

		scoreText.text = "Red    Blue \n " + redScore + " - " + blueScore;
	}

	public void RedGoal(){
		playerColor = "Red";
	}

	public void BlueGoal(){
		playerColor = "Blue";
	}


}
