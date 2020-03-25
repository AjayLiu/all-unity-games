using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class deckButtonScript : MonoBehaviour {

    public string thisDeckID;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void LoadMainGame() {
        dontDestroyScript dontdestroyInstance;
        dontdestroyInstance = GameObject.Find("DONTDESTROY").GetComponent<dontDestroyScript>();
        string[] s = thisDeckID.Split(' ');
        dontdestroyInstance.deckIDNumber = int.Parse(s[1]);
        dontdestroyInstance.isStandard = s[0] == "STANDARD" ? true : false;
        print("SELECTED DECK IS " + thisDeckID);
        SceneManager.LoadScene("MainGame");
    }
}
