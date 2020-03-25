using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainGame : MonoBehaviour {

    [SerializeField]
    float CORRECT_PASS_THRESHOLD = 85f;

    [SerializeField]
    Text wordDisplayText;
    [SerializeField]
    Text timeLeftLabel;

    dontDestroyScript dontdestroy;

    List<string> words = new List<string>();

    string currentWord;
    int currentWordIndex = 0;

    int timeLimit = 60;
    float timeLeft;
    float beginningBufferTime = 5;
    bool lockInput = true;

    bool lockMobile = true;

    void Start() {

        wordDisplayText.gameObject.SetActive(false);

        lockInput = true;

        dontdestroy = GameObject.Find("DONTDESTROY").GetComponent<dontDestroyScript>();

        Screen.orientation = ScreenOrientation.LandscapeLeft;

        timeLeft = timeLimit + beginningBufferTime;

        if(dontdestroy.isStandard) {
            words = dontdestroy.deckContents[dontdestroy.deckIDNumber].Split(new string[] { "[word]" }, System.StringSplitOptions.None).ToList();         
        } else {
            string[] s = PlayerPrefs.GetString("CUSTOM " + dontdestroy.deckIDNumber).Split(new string[] { "[title]" }, System.StringSplitOptions.None);
            words = s[1].Split(new string[] { "[word]" }, System.StringSplitOptions.None).ToList();
        }

        for(int i = 0; i < words.Count; i++) {
            print(words[i]);
        }


        //SHUFFLE DECK
        for (int i = 0; i < words.Count; i++) {
            string temp = words[i];
            int randomIndex = Random.Range(i, words.Count);
            words[i] = words[randomIndex];
            words[randomIndex] = temp;
        }

        print("AFTER");
        for (int i = 0; i < words.Count; i++) {
            print(words[i]);
        }

        currentWord = words[currentWordIndex];
        wordDisplayText.text = currentWord; 
    }


    void Update () {

        if(timeLeft <= 0) {
            SceneManager.LoadScene("EndScene");
        }

        if(timeLeft <= timeLimit) {
            lockInput = false;
            wordDisplayText.gameObject.SetActive(true);
        }

        timeLeft -= Time.deltaTime;

        if(timeLeft > timeLimit) {
            timeLeftLabel.text = Mathf.Round(timeLeft - timeLimit).ToString();
        } else {
            timeLeftLabel.text = Mathf.Round(timeLeft).ToString();
        }

        if (!lockInput && !lockMobile) {
            //KEYBOARD
            if (Input.GetKeyDown(KeyCode.DownArrow)) {
                CorrectWord();
                SwitchWord();
            }
            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                SkipWord();
                SwitchWord();
            }

            //MOBILE
            if(Input.acceleration.z > CORRECT_PASS_THRESHOLD / 100) {
                CorrectWord();
                SwitchWord();
                lockMobile = true;
            }
            if (Input.acceleration.z < -CORRECT_PASS_THRESHOLD / 100) {
                SkipWord();
                SwitchWord();
                lockMobile = true;
            }
        }

        if (Input.acceleration.z < (CORRECT_PASS_THRESHOLD / 100) / 2 && Input.acceleration.z > -(CORRECT_PASS_THRESHOLD / 100)) {
            lockMobile = false;
        }
        
    }

    void SwitchWord() {
        if(currentWordIndex == words.Count - 1) {
            SceneManager.LoadScene("EndScene");
        }

        currentWordIndex++;
        currentWord = words[currentWordIndex];

        wordDisplayText.text = currentWord;
    }

    void CorrectWord() {
        dontdestroy.correctCount++;
        dontdestroy.correctWords.Add(currentWord);
    }

    void SkipWord() {
        dontdestroy.skipCount++;
        dontdestroy.skipWords.Add(currentWord);
    }
}
