using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{

    public Text scoreText, highScoreText;

    // Start is called before the first frame update
    void Start()
    {
        DisplayScores();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DisplayScores(){
        scoreText.text = "Your Score: " + PlayerPrefs.GetInt("Score");

        //UPDATE HIGH SCORE
        PlayerPrefs.SetInt("HighScore", Mathf.Max(PlayerPrefs.GetInt("HighScore"), PlayerPrefs.GetInt("Score")));

        highScoreText.text = "High Score: " + PlayerPrefs.GetInt("HighScore");
    }

    public void OnBackToMenuPress(){
        SceneManager.LoadScene(0);
    }
}
