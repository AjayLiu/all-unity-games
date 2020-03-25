using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameEnd : MonoBehaviour {

    public Text highScoreText, scoreText;
    
    // Use this for initialization
	void Start () {
        highScoreText.text = "High Score: " + PlayerPrefs.GetInt("Highscore") + " rhymes";
        scoreText.text = "Your Score: " + PlayerPrefs.GetInt("Score") + " rhymes";
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnPlayPress() {
        SceneManager.LoadScene("Survival");
    }
    

}
