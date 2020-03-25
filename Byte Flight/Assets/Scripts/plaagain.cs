using System.Collections;using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class plaagain : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("highscore"))
        {
            PlayerPrefs.SetInt("highscore", 0);
        }
        if (PlayerPrefs.GetInt("score") > PlayerPrefs.GetInt("highscore"))
        {
            PlayerPrefs.SetInt("highscore", PlayerPrefs.GetInt("score"));

        }
        highscoretext.text = "high score: "+ PlayerPrefs.GetInt("highscore");

    }
    public Text highscoretext;
    // Update is called once per frame
    public void playagain()
    {
        SceneManager.LoadScene(1);
    }
}
