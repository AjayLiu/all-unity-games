using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class End : MonoBehaviour {

    dontDestroyScript dontdestroy;

    [SerializeField]
    Text correctText;
    [SerializeField]
    Text skipText;

	// Use this for initialization
	void Start () {
        dontdestroy = GameObject.Find("DONTDESTROY").GetComponent<dontDestroyScript>();

        Screen.orientation = ScreenOrientation.Portrait;


        correctText.text = "Correct Words: " + dontdestroy.correctCount;
        skipText.text = "Skip Words: " + dontdestroy.skipCount;
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void PlayAgain() {

        dontdestroy.correctWords.Clear();
        dontdestroy.skipWords.Clear();

        dontdestroy.correctCount = 0;
        dontdestroy.skipCount = 0;
        SceneManager.LoadScene("MenuScene");
    }
}
