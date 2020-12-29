using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour {

    public GameObject popupMessageObject, settingsObject;
    public Text popupText;

    public TextAsset creditsTextFile, howToPlayTextFile;

    public Text[] bindDescriptions, bindButtonTexts;

    AudioSource music;

    // Use this for initialization
    void Start() {
        popupMessageObject.SetActive(false);
        music = GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>();

        UpdateSettings();
    }    

    void UpdateSettings() {
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 0.5f);
        music.volume = volumeSlider.value;

        for (int i = 0; i < bindButtonTexts.Length; i++) {
            if(PlayerPrefs.HasKey("Click"+i))
                bindButtonTexts[i].text = PlayerPrefs.GetString("Click"+i);
        }
    }

    public void OnStartButton() {
        SceneManager.LoadScene("Selection Menu");
    }

    public void OnBeatmapMakerButton() {
        SceneManager.LoadScene("Beatmap Maker");
    }

    public void OnHowToPlayButton() {
        DisplayPopupMessage(howToPlayTextFile.text);
    }

    public void OnSettingsButton() {
        popupMessageObject.SetActive(true);
        popupText.gameObject.SetActive(false);
        settingsObject.SetActive(true);
    }

    public void OnCreditsButton() {
        DisplayPopupMessage(creditsTextFile.text);
    }

    public void OnXButton() {
        popupMessageObject.gameObject.SetActive(false);
    }


    public Slider volumeSlider;
    public void OnVolumeChanged() {
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        music.volume = volumeSlider.value;
    }


    public void ChangeBind(int bindID) {
        StartCoroutine(BindProcess(bindID));        
    }

    KeyCode userPressedKey;

    IEnumerator BindProcess(int bindID) {
        bindButtonTexts[bindID].text = "...";
        string originalDescription = bindDescriptions[bindID].text;
        bindDescriptions[bindID].text = "Press key to bind";

        yield return new WaitUntil(()=>Input.anyKeyDown);

        foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode))) {
            if (Input.GetKeyDown(kcode)) {
                bindButtonTexts[bindID].text = kcode.ToString();
                bindDescriptions[bindID].text = originalDescription;
                PlayerPrefs.SetString("Click" + bindID, kcode.ToString());
            }
        } 
    }



    void DisplayPopupMessage(string message) {
        popupMessageObject.gameObject.SetActive(true);
        settingsObject.SetActive(false);
        popupText.gameObject.SetActive(true);
        popupText.text = message;
    }


}
