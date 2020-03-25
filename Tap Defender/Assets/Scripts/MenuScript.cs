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
    public Slider camRotationSpeedSlider;

    AudioSource music;

	// Use this for initialization
	void Start () {
        popupMessageObject.SetActive(false);
        //warningWindow.SetActive(false);
        //music = GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>();

        UpdateSettings();
        PlaySlideShowIfNeeded();
    }

    void PlaySlideShowIfNeeded() {
        if(!PlayerPrefs.HasKey("Finished Slides")) {
            PlayerPrefs.SetInt("Finished Slides", 1);
            SceneManager.LoadScene(1);
        }
    }

    void UpdateSettings(){
        /*
        if (!PlayerPrefs.HasKey("Camera Pan Speed")){
            PlayerPrefs.SetFloat("Camera Pan Speed", 0.5f);
        }
        camRotationSpeedSlider.value = PlayerPrefs.GetFloat("Camera Pan Speed");

        if (!PlayerPrefs.HasKey("Quality Level"))
            PlayerPrefs.SetInt("Quality Level", 1);        
        qualityDropdown.value = PlayerPrefs.GetInt("Quality Level");

        */

        if (!PlayerPrefs.HasKey("Music Volume")) {
            PlayerPrefs.SetFloat("Music Volume", 0.5f);
        }
        volumeSlider.value = PlayerPrefs.GetFloat("Music Volume");
        //music.volume = volumeSlider.value;


        
        if (!PlayerPrefs.HasKey("BombEnabled")) {
            PlayerPrefs.SetInt("BombEnabled", 1);
        }
        ToggleBomb(); ToggleBomb();

        if (!PlayerPrefs.HasKey("DoubleEnabled")) {
            PlayerPrefs.SetInt("DoubleEnabled", 1);
        }
        ToggleMultiplier(); ToggleMultiplier();

        
    }

    public void OnStartButton(){
        SceneManager.LoadScene(3);
    }

    public void OnTutorialButton(){
        SceneManager.LoadScene(2);
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

    public void OnCameraRotationSpeedChanged(){
        PlayerPrefs.SetFloat("Camera Pan Speed", camRotationSpeedSlider.value);
    }


    public Slider volumeSlider; 
    public void OnVolumeChanged() {
        PlayerPrefs.SetFloat("Music Volume", volumeSlider.value);
        //music.volume = volumeSlider.value;
    }

    //public GameObject warningWindow;


    public Image bombIcon, multIcon, bombYesNoImg, multYesNoImg;
    public Sprite bombOnSprite, bombOffSprite, multOnSprite, multOffSprite, yesSprite, noSprite, yesHighlight, noHighlight;

    bool bombEnabled = true, multEnabled = true;

    public void ToggleBomb() {
        bool on = PlayerPrefs.GetInt("BombEnabled") == 1;
        on = !on;
        bombIcon.sprite = on ? bombOnSprite : bombOffSprite;
        bombYesNoImg.sprite = on ? yesSprite : noSprite;
        SpriteState ss = new SpriteState();
        ss.highlightedSprite = on ? yesHighlight : noHighlight;
        bombYesNoImg.GetComponent<Button>().spriteState = ss;


        PlayerPrefs.SetInt("BombEnabled", on ? 1 : 0);
    }

    public void ToggleMultiplier() {
        bool on = PlayerPrefs.GetInt("DoubleEnabled") == 1;
        on = !on;
        multIcon.sprite = on ? multOnSprite : multOffSprite;
        multYesNoImg.sprite = on ? yesSprite : noSprite;
        SpriteState ss = new SpriteState();
        ss.highlightedSprite = on ? yesHighlight : noHighlight;
        multYesNoImg.GetComponent<Button>().spriteState = ss;
        PlayerPrefs.SetInt("DoubleEnabled", on ? 1 : 0);
    }

    void DisplayPopupMessage(string message){
        popupMessageObject.gameObject.SetActive(true);
        settingsObject.SetActive(false);
        popupText.gameObject.SetActive(true);
        popupText.text = message;
    }

    public GameObject inputSelect;
    public void SetKeyboard() {
        PlayerPrefs.SetInt("isKeyboard", 1);
        inputSelect.SetActive(false);
    }
    public void SetMobile() {
        PlayerPrefs.SetInt("isKeyboard", 0);
        inputSelect.SetActive(false);
    }

}
