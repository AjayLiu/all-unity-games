using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameControllerScript : MonoBehaviour
{
    DrawLine drawScript;
    public GameObject ball;
    public Transform ballSpawn;
    int score;
    public Text scoreText, warningText, levelText;
    public Button leftButton, rightButton;
    public GameObject drawZone;
    bool isDraw = true;

    public GameObject tracer;

    // Start is called before the first frame update
    void Start()
    {

        SpawnBall();
        levelText.text = "Level 1";
        drawScript = GetComponent<DrawLine>();
        Time.timeScale = 0;
        bgRenderer.sprite = backgrounds[0];
        leftButton.gameObject.SetActive(false);
        rightButton.gameObject.SetActive(false);
    }

    public List<Vector3> difficulties = new List<Vector3>();
    int difficultyIndex = 0;


    public List<Sprite> backgrounds = new List<Sprite>(); 

    // Update is called once per frame
    void Update()
    {
        CheckIfDead();

        TraceBall();
        SpinDifficulty();

        score = (int)Time.timeSinceLevelLoad;
        scoreText.text = "Score: " + score.ToString();


        if (score > difficulties[difficultyIndex].x) {
            OnDifficultyChange();
        }

        if (drawScript.isDraw != isDraw) {
            OnDrawChange();
        }

        

        if (Input.GetKeyDown(KeyCode.Space)) {
            SceneManager.LoadScene(1);
        }

    }

    public SpriteRenderer bgRenderer;

    void OnDifficultyChange() {
        ball.transform.position = ballSpawn.position;
        Time.timeScale = 0;
        drawScript.leftPress = drawScript.rightPress = false;
        drawScript.inkLimit = difficulties[difficultyIndex].y;
        drawScript.DeleteLine();
        Destroy(currentBall);
        SpawnBall();
        OnDrawChange();
        drawScript.isDraw = true;
        Destroy(GameObject.FindGameObjectWithTag("Hazard"));
        Destroy(GameObject.FindGameObjectWithTag("Star"));

        bgRenderer.sprite = backgrounds[difficultyIndex+1];
        levelText.text = "Level " + (difficultyIndex+2).ToString();
        difficultyIndex++;

        
    }

    void OnDrawChange() {
        isDraw = drawScript.isDraw;
        warningText.gameObject.SetActive(drawScript.isDraw);
        drawZone.gameObject.SetActive(drawScript.isDraw);
        levelText.gameObject.SetActive(drawScript.isDraw);
        leftButton.gameObject.SetActive(!drawScript.isDraw);
        rightButton.gameObject.SetActive(!drawScript.isDraw);

    }

    GameObject currentBall;

    public void SpawnBall() {
        currentBall = Instantiate(ball, ballSpawn.position, Quaternion.identity);
    }

    void TraceBall() {
        if(currentBall.transform.position.y > 5) {
            tracer.gameObject.SetActive(true);
            tracer.transform.position = new Vector3(currentBall.transform.position.x, tracer.transform.position.y, tracer.transform.position.z);
        } else {
            tracer.gameObject.SetActive(false);
        }
    }

    public int pointsPerStar = 1;

    public GameObject starPrefab, hazardPrefab;

    public void OnStar() {
        score += pointsPerStar;
        SpawnStar();
    }
    public void OnHazard() {
        GameOver();
    }

    public void SpawnStar() {
        Instantiate(starPrefab, RandomPosInMap(), Quaternion.identity);
    }

    public void SpawnHazard() {
        if(difficultyIndex >= 1) {
            Instantiate(hazardPrefab, RandomPosInMap(), Quaternion.identity);
        }
    }


    void SpinDifficulty() {
        if(drawScript.startGame && difficulties[difficultyIndex].z != 1f) {
            drawScript.RotateDrawing(difficulties[difficultyIndex].z);
        }
    }

    Vector2 RandomPosInMap() {
        return new Vector2(Random.Range(-drawScript.xBounds, drawScript.xBounds), Random.Range(0, 4f));
    }

    void CheckIfDead() {
        if(currentBall.transform.position.y < -5) {
            GameOver();
        }
    }

    public void GameOver() {
        PlayerPrefs.SetInt("score", score);
        SceneManager.LoadScene(2);
    }
}
