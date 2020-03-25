using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameStatus {
    P1Place, P2Place, P1Receive, P2Receive
};

public class GameCon : MonoBehaviour {

    public GameStatus status;

    public Material[] colors = new Material[4];
    public GameObject barPrefab;

    public GameObject[] buttons = new GameObject[8];
    [HideInInspector]public GameObject[] detectors = new GameObject[8];
    public GameObject detectorPrefab;

    [HideInInspector]public List<GameObject> bars = new List<GameObject>();

    public int barLimit = 8;
    public int barCount = 0;

    public float barSpeed = 10;

    bool p1Turn;

	// Use this for initialization
	void Start () {

        

        if (SceneManager.GetActiveScene().name == "Start") {
            Time.timeScale = 0;
        }
        if (SceneManager.GetActiveScene().name == "Game") {
            StartCoroutine("StartCountdown");
        }
        if (SceneManager.GetActiveScene().name == "GameOver") {
            ShowGameOver();
        }
    }

	void Update () {
		
	}

    void PlacementStart() {
        
    }

    void ReceiveStart() {

    }

    public void StartGameButton() {
        SceneManager.LoadScene("Game");
    }

    public Text countdownText;
    IEnumerator StartCountdown() {

        Time.timeScale = 0;

        countdownText.gameObject.SetActive(true);

        for(int i = 3; i > 0; i--) {
            countdownText.text = i.ToString();
            yield return new WaitForSecondsRealtime(1f);
        }

        countdownText.gameObject.SetActive(false);  
        Time.timeScale = 1f;

        SpawnDetectors();
    }

    void GameOver() {
        SceneManager.LoadScene("GameOver");
    }

    public void PlayAgainButton() {
        SceneManager.LoadScene("Game");
    }

    void ShowGameOver() {
        
    }

    void SpawnDetectors() {
        for(int i = 0; i < buttons.Length; i++) {
            Vector3 camPos = Camera.main.ScreenToWorldPoint(buttons[i].transform.position);
            GameObject ins = Instantiate(detectorPrefab, new Vector3(camPos.x, camPos.y, 0), Quaternion.identity);
        }
    }

}
