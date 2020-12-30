using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{
    public Transform songItemParent;
    public GameObject songListItemPrefab;
    SelectionMenuScript selectionMenuScript;

    public Text scoreText, highScoreText, accuracyText, comboText;


    // Start is called before the first frame update
    void Start()
    {
        selectionMenuScript = GameObject.FindGameObjectWithTag("SelectionGameController").GetComponent<SelectionMenuScript>();
        SongItemScript item = Instantiate(songListItemPrefab, songItemParent).GetComponent<SongItemScript>();
        item.Init(selectionMenuScript.songs[selectionMenuScript.currentIndex], 0);
        RectTransform rect = item.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
        rect.localPosition = Vector2.zero;
        //rect.localScale = Vector3.one * 0.6f;

        LoadStats();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    static GameData gameData;

    void LoadStats() {
        gameData = new GameData();
        scoreText.text = "Your Score: " + GameData.score.ToString();
        highScoreText.text = "High Score: " + GameData.highScore.ToString();
        float accuracy = 100f * GameData.numNotesHit / GameData.numNotesTotal;
        accuracyText.text = "Accuracy: " + (accuracy == 0 ? "0" : accuracy.ToString("#.##")) + "%";
        comboText.text = "Highest Combo: " + GameData.highestCombo.ToString();
    }

    public void PlayAgainButton() {
        selectionMenuScript.LoadSong(selectionMenuScript.currentIndex);
    }

    public void ReturnToMenuButton() {
        SceneManager.LoadScene("Selection Menu");
    }
}
