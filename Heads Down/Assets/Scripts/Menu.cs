using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    public GameObject deckPrefab;

    [HideInInspector]public string playDeck;

    dontDestroyScript dontdestroyInstance;
        
    int STANDARD_DECK_COUNT = 20;

	// Use this for initialization
	void Start () {
        dontdestroyInstance = GameObject.Find("DONTDESTROY").GetComponent<dontDestroyScript>();

        //LOAD DECKS
        if (!PlayerPrefs.HasKey("customDeckCount")) {
            PlayerPrefs.SetInt("customDeckCount", 0);
        }

        print(PlayerPrefs.GetInt("customDeckCount"));

        //load titles for custom decks
        for (int i = 0; i < PlayerPrefs.GetInt("customDeckCount"); i++) {
            if (PlayerPrefs.HasKey("CUSTOM " + i)) {
                string[] s = new string[2];
                s = PlayerPrefs.GetString("CUSTOM " + i).Split(new string[] { "[title]" }, System.StringSplitOptions.None);

                dontdestroyInstance.deckTitles.Add(s[0]);
            } else {
                print("CUSTOM DECK MISSING");
            }
        }

        //SPAWN SELECTION BUTTONS FOR STANDARD DECKS
        for (int i = 0; i < STANDARD_DECK_COUNT; i++) {
            GameObject instance = Instantiate(deckPrefab, GameObject.Find("GridWithOurElements").transform);
            Text deckText = instance.GetComponentInChildren<Text>();
            deckText.text = dontdestroyInstance.deckTitles[i];

            Image image = instance.GetComponentInChildren<Image>();
            image.color = Color.white;

            instance.GetComponent<deckButtonScript>().thisDeckID = "STANDARD " + i;
        }

        // SPAWN SELECTION DECKS FOR CUSTOM DECKS
        for(int i = STANDARD_DECK_COUNT; i < PlayerPrefs.GetInt("customDeckCount") + STANDARD_DECK_COUNT; i++) {
            GameObject instance = Instantiate(deckPrefab, GameObject.Find("GridWithOurElements").transform);
            Text deckText = instance.GetComponentInChildren<Text>();
            deckText.text = dontdestroyInstance.deckTitles[i];

            Image image = instance.GetComponentInChildren<Image>();
            image.color = Color.gray;

            instance.GetComponent<deckButtonScript>().thisDeckID = "CUSTOM " + (i - STANDARD_DECK_COUNT);
        }
    }

    public void LoadCustomCreator() {
        SceneManager.LoadScene("CustomDeckEditorScene");
    }

    public void ClearCustomDecks() {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("MenuScene");
    }

}
