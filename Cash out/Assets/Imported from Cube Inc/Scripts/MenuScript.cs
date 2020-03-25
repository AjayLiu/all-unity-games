using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


/* Written by Ajay Liu, 2019 */

public class MenuScript : MonoBehaviour {

    public GameObject popupMessageObject, settingsObject;
    public Text popupText;

    public TextAsset creditsTextFile, howToPlayTextFile;

    public Dropdown qualityDropdown;

    AudioSource music;

	// Use this for initialization
	void Start () {
        popupMessageObject.SetActive(false);
        warningWindow.SetActive(false);
        music = GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>();

        UpdateSettings();
        
    }

    void UpdateSettings(){

        if (!PlayerPrefs.HasKey("Quality Level"))
            PlayerPrefs.SetInt("Quality Level", 1);        
        qualityDropdown.value = PlayerPrefs.GetInt("Quality Level");

        if (!PlayerPrefs.HasKey("Music Volume")) {
            PlayerPrefs.SetFloat("Music Volume", 0.5f);
        }
        volumeSlider.value = PlayerPrefs.GetFloat("Music Volume");
        music.volume = volumeSlider.value;

    }

    public void OnStartButton(){        
        SceneManager.LoadScene(1);
    }

    public void OnHowToPlayButton(){
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

    public void OnXButton(){
        popupMessageObject.gameObject.SetActive(false);
    }

    public void OnQualitySettingChanged(){
        QualitySettings.SetQualityLevel(qualityDropdown.value);
        PlayerPrefs.SetInt("Quality Level", qualityDropdown.value);
    }



    public Slider volumeSlider; 
    public void OnVolumeChanged() {
        PlayerPrefs.SetFloat("Music Volume", volumeSlider.value);
        music.volume = volumeSlider.value;
    }

    public GameObject warningWindow;

    public void OnClearAllProgressButton(){
        warningWindow.SetActive(true);
    }

    public void OnYesButton(){
        PlayerPrefs.DeleteAll();
        UpdateSettings();
        warningWindow.SetActive(false);
        popupMessageObject.SetActive(false);
    }
    public void OnNoButton() {
        warningWindow.SetActive(false);
        popupMessageObject.SetActive(false);

    }

    void DisplayPopupMessage(string message){
        popupMessageObject.gameObject.SetActive(true);
        settingsObject.SetActive(false);
        popupText.gameObject.SetActive(true);
        popupText.text = message;
    }

    

}
