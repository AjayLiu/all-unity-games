using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongItemScript : MonoBehaviour
{
    [HideInInspector] public int index;
    public Image logo, difficultyImage, highScoreImage;
    public Text titleText, descriptionText, difficultyText, highScoreText;
    Button button;

    SelectionMenuScript menuScript;

    // Start is called before the first frame update
    void Start()
    {
        menuScript = GameObject.FindGameObjectWithTag("SelectionGameController").GetComponent<SelectionMenuScript>();
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }    

    public void Init(SongInformation info, int index) {
        this.index = index;

        titleText.text = info.title;
        descriptionText.text = info.description;
        logo.sprite = info.img;

        difficultyText.text = info.difficulty.ToString();
        difficultyImage.fillAmount = info.difficulty / 5f;


        int highScore = PlayerPrefs.GetInt(titleText.text, 0);
        highScoreText.text = highScore.ToString();
        highScoreImage.fillAmount = 1.0f * highScore / info.data.CalculateMaxScore();
    }

    void OnClick() {
        menuScript.LoadSong(index);
    }


}
