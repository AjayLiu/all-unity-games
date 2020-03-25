using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameControllerScript : MonoBehaviour
{

    public List<GameObject> redguyList = new List<GameObject>();

    public GameObject redGuyPrefab;
    public GameObject bombPrefab, doublePrefab;

    public float spawnInterval = 1;
    public float spawnTimeMultiplier = 0.98f;
    public float spawnIntervalMin = 0.1f;


    Vector3 topRight;
    Vector3 botLeft;

    // Start is called before the first frame update
    void Start()
    {
        topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        botLeft = Camera.main.ScreenToWorldPoint(Vector3.zero);
        doubleCountdownBar.transform.parent.gameObject.SetActive(false);

        if (!isTutorial) {
            SpawnEnemyAtBorder();

            Invoke("SpawnBomb", spawnBombInterval);
            Invoke("SpawnDouble", spawnDoubleInterval);
            Resume();
        }
    }


    public void SpawnEnemyAtBorder() {        

        float randX;
        float randY;

        if(Random.Range(0, 2) == 0) {
            //TOP or BOTTOM
            randX = Random.Range(botLeft.x, topRight.x);
            randY = Random.Range(0, 2) == 0 ? botLeft.y : topRight.y;
        } else {
            //LEFT OR RIGHT
            randY = Random.Range(botLeft.y, topRight.y);
            randX = Random.Range(0, 2) == 0 ? botLeft.x : topRight.x;
        }

        SpawnEnemyAt(new Vector2(randX, randY));
        
    }

    public void SpawnEnemyAt(Vector2 vec){
        redguyList.Add(Instantiate(redGuyPrefab, new Vector3(vec.x, vec.y, 0), Quaternion.identity));

        if (!isTutorial) {
            spawnInterval = Mathf.Max(spawnInterval * spawnTimeMultiplier, spawnIntervalMin);
            Invoke("SpawnEnemyAtBorder", spawnInterval);
        }
    }

    public float spawnBombInterval, spawnDoubleInterval;
    public float spawnBombTimeMultiplier, spawnDoubleTimeMultiplier;

    public GameObject SpawnBomb() {
        GameObject ins = new GameObject();
        if (PlayerPrefs.GetInt("BombEnabled") == 1 || isTutorial) {
            ins = Instantiate(bombPrefab, isTutorial? new Vector2(0, 50): RandomPosOnArea(), Quaternion.identity);
            spawnBombInterval *= spawnBombTimeMultiplier;
        }
        if(!isTutorial)
            Invoke("SpawnBomb", spawnBombInterval);
        return ins;
    }

    public GameObject SpawnDouble(){
        GameObject ins = new GameObject();
        if (PlayerPrefs.GetInt("DoubleEnabled") == 1 || isTutorial) {
            ins = Instantiate(doublePrefab, isTutorial ? new Vector2(0, -50) : RandomPosOnArea(), Quaternion.identity);
            spawnDoubleInterval *= spawnDoubleTimeMultiplier;
        }
        if (!isTutorial)
            Invoke("SpawnDouble", spawnDoubleInterval);
        return ins;
    }


    [HideInInspector]public bool allowTaps = true;
    
    // Update is called once per frame
    void Update()
    {
        if(allowTaps)
            DetectTaps();
    }

    void DetectTaps() {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10;

            Vector3 screenPos = Camera.main.ScreenToWorldPoint(mousePos);

            RaycastHit2D hit = Physics2D.Raycast(screenPos, Vector2.zero);

            if (hit) {
                if (hit.collider.gameObject.tag == "Enemy") {
                    hit.collider.gameObject.GetComponent<RedGuyScript>().Die(true);
                }
                if (hit.collider.gameObject.tag == "Bomb") {
                    hit.collider.gameObject.GetComponent<BombScript>().ClickedOn();
                }
                if (hit.collider.gameObject.tag == "Double") {
                    hit.collider.gameObject.GetComponent<DoubleScript>().ClickedOn();
                    if (currentDouble != null)
                        StopCoroutine(currentDouble);                
                    currentDouble = StartCoroutine(UseDouble());
                    
                }
            }
        }
    }

    public Image doubleCountdownBar;
    public float doubleDuration;
    int scoreMultiplier = 1;
    Coroutine currentDouble;

    IEnumerator UseDouble(){
        doubleCountdownBar.transform.parent.gameObject.SetActive(true);
        scoreMultiplier = 2;


        //ANIMATE COUNTDOWN BAR
        float timeElapsed = 0;
        while(timeElapsed < doubleDuration) {
            doubleCountdownBar.fillAmount = timeElapsed / doubleDuration;
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        scoreMultiplier = 1;
        doubleCountdownBar.transform.parent.gameObject.SetActive(false);
    }

    [HideInInspector] public int score = 0;
    public Text scoreText;

    public bool isTutorial;

    public void OnEnemyKilled() {
        score += scoreMultiplier;
        scoreText.text = "x" + score;
    }

    public float padding = 20;
    Vector2 RandomPosOnArea() { 
        float randX = Random.Range(botLeft.x + padding, topRight.x - padding);
        float randY = Random.Range(botLeft.y + padding, topRight.y - padding);
        return new Vector2(randX, randY);
    }

    public GameObject pausePanel;

    public void Pause(){
        Time.timeScale = 0;
        pausePanel.SetActive(true);
    }

    public void Resume(){
        Time.timeScale = 1;
        pausePanel.SetActive(false);
    }

    public void OnBackToMenuPress() {
        SceneManager.LoadScene(0);
    }

    public void GameOver() {
        PlayerPrefs.SetInt("Score", score);
        SceneManager.LoadScene(4);
    }
}
