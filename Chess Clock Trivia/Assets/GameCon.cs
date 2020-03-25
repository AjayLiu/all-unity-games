using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameCon : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if (SceneManager.GetActiveScene().name == "Start") {
            Time.timeScale = 0;
        }
        if (SceneManager.GetActiveScene().name == "Game") {
            StartCoroutine("StartCountdown");
        }
        if (SceneManager.GetActiveScene().name == "GameOver") {
            ShowGameOver();
        }
    }

    float clockTime = 60f;

    bool isLeftTurn = true;

    float leftTimeRemain, rightTimeRemain;

	// Update is called once per frame
	void Update () {
		if(isLeftTurn) {
            leftButton.image.fillAmount = leftTimeRemain / clockTime;
            leftTimeRemain -= Time.deltaTime;
            if(leftTimeRemain <= 0) {
                GameOver();
                leftWon = false;
            }
        } else {
            rightButton.image.fillAmount = rightTimeRemain / clockTime;
            rightTimeRemain -= Time.deltaTime;
            if (rightTimeRemain <= 0) {
                GameOver();
                leftWon = true;
            }
        }
	}

    public Button leftButton, rightButton, skipButton;
    public Text prompt;

    int c = 0; // cards played counter

    public static int leftPoints = 0, rightPoints = 0;
    public static bool leftWon = false;

    public void LeftPressButton() {
        if(isLeftTurn) {
            c++;
            if (c == prompts.Count) {
                promptRanOut = true;
                GameOver();
            } else {
                leftPoints++;
                LoadPrompt(c);
            }
            isLeftTurn = false;
        }        
    }

    public void RightPressButton() {
        if(!isLeftTurn) {
            c++;
            if (c == prompts.Count) {
                promptRanOut = true;
                GameOver();
            } else {
                rightPoints++;
                LoadPrompt(c);
            }
            isLeftTurn = true;
        }           
    }

    float skipPenalty = 5f;

    public void SkipPressButton() {
        c++;
        if (c == prompts.Count) {
            promptRanOut = true;
            GameOver();
        }

        if (isLeftTurn) {
            leftTimeRemain -= skipPenalty;
            leftButton.image.fillAmount = leftTimeRemain / clockTime;
            isLeftTurn = false;
            LoadPrompt(c);
        } else {
            rightTimeRemain -= skipPenalty;
            rightButton.image.fillAmount = rightTimeRemain / clockTime;
            isLeftTurn = true;
            LoadPrompt(c);
        }
        
    }

    public void StartGameButton() {
        SceneManager.LoadScene("Game");
    }

    List<string> prompts = new List<string>();

    void LoadPrompt(int promptIndex) {
        prompt.text = prompts[promptIndex];
    }

    public Text countdownText;
    IEnumerator StartCountdown() {

        #region prompts
        //PROMPTS
        prompts.Add("TEST");
        prompts.Add("LOL");
        prompts.Add("YEEZYS");
        prompts.Add("WADDUP");
        #endregion

        Time.timeScale = 0;
        leftButton.gameObject.SetActive(false);
        rightButton.gameObject.SetActive(false);
        prompt.gameObject.SetActive(false);
        countdownText.gameObject.SetActive(true);
        skipButton.gameObject.SetActive(false);

        leftPoints = 0; rightPoints = 0;
        leftTimeRemain = clockTime; rightTimeRemain = clockTime;

        promptRanOut = false;

        for(int i = 3; i > 0; i--) {
            countdownText.text = i.ToString();
            yield return new WaitForSecondsRealtime(1f);
        }
        countdownText.gameObject.SetActive(false);  
        leftButton.gameObject.SetActive(true);
        rightButton.gameObject.SetActive(true);
        prompt.gameObject.SetActive(true);
        skipButton.gameObject.SetActive(true);
        Time.timeScale = 1f;

        //Shuffle deck
        for (int i = 0; i < prompts.Count; i++) {
            string temp = prompts[i];
            int randomIndex = Random.Range(i, prompts.Count);
            prompts[i] = prompts[randomIndex];
            prompts[randomIndex] = temp;
        }

        LoadPrompt(c);
    }

    public static bool promptRanOut = false;

    void GameOver() {
        SceneManager.LoadScene("GameOver");
    }

    public void PlayAgainButton() {
        SceneManager.LoadScene("Game");
    }

    public Text winnerText, leftPointsText, rightPointsText;

    void ShowGameOver() {
        if(promptRanOut) {
            winnerText.text = "LOL WE RAN OUT OF CARDS! xD";
        } else {
            if (leftWon) {
                winnerText.text = "The winner is P1!";
            } else {
                winnerText.text = "The winner is P2!";
            }
        }
       
        if(leftPoints == 1)
            leftPointsText.text = "P1 answered 1 question!";
        else 
            leftPointsText.text = "P1 answered " + leftPoints + " questions!";

        if (rightPoints == 1)
            rightPointsText.text = "P2 answered 1 question!";
        else
            rightPointsText.text = "P2 answered " + rightPoints + " questions!";
    }

}
