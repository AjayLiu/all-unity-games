using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    public GameObject pausePanel, gameUIObject;
    DrawLine draw;

    // Start is called before the first frame update
    void Start()
    {
        draw = GameObject.FindGameObjectWithTag("GameController").GetComponent<DrawLine>();
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape)) {
            Pause();
        }
    }

    bool isPause;

    public void Pause() {
        if (draw.startGame) {
            isPause = true;
            Time.timeScale = 0;
            UpdateUI();
        }      
    }

    public void Resume() {
        isPause = false;
        Time.timeScale = 1;
        UpdateUI();

    }

    public void Quit() {
        Application.Quit();
    }

    void UpdateUI(){
        pausePanel.SetActive(isPause);
        gameUIObject.SetActive(!isPause);
    }
}
