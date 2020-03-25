using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;


public class GameScript : MonoBehaviour {
    GameObject player;
    PlayerScript play;
    public GameObject platformPrefab;

    public GameObject startPlatform;

    [SerializeField]public Color[] colors;

    public static float screenLeft, screenRight, screenBottom, screenTop;
    float platformWidth = 1;

    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        play = player.GetComponent<PlayerScript>();

        screenLeft = Camera.main.ViewportToWorldPoint(Vector3.zero).x + platformWidth / 2;
        screenRight = Camera.main.ViewportToWorldPoint(Vector3.one).x - platformWidth / 2;
        screenBottom = Camera.main.ViewportToWorldPoint(Vector3.zero).y;
        screenTop = Camera.main.ViewportToWorldPoint(Vector3.one).y;

        pausePanel.SetActive(false);

        startMenuCanvas.gameObject.SetActive(true);
        gameCanvas.gameObject.SetActive(false);
        play.rb.constraints = RigidbodyConstraints.FreezePositionX;
        play.rb.constraints = RigidbodyConstraints.FreezePositionY;
        play.allowTouch = false;

        DisplayHighScoreAndPreviousScore();
        if (!PlayerPrefs.HasKey("HighScore"))
            PlayerPrefs.SetInt("HighScore", 0);
        if (!PlayerPrefs.HasKey("PreviousScore"))
            PlayerPrefs.SetInt("PreviousScore", 0);

        difficulties = GameObject.Find("Difficulties").GetComponentsInChildren<DifficultyStageCustomizer>();

        warning.SetActive(false);
    }

    bool allowCheckSpawn = false;

    void Update () {
        screenBottom = Camera.main.ViewportToWorldPoint(Vector3.zero).y;
        screenTop = Camera.main.ViewportToWorldPoint(Vector3.one).y;

        CheckIfDead();
        if(allowCheckSpawn)
            CheckWhetherToSpawn();
        UpdateScore();
        SpreadMovement();

        if (checkStarTimeLeft)
            CheckStarTimeLeft();

        if (isStar) {
            starTimeLeft -= Time.deltaTime;
        }
    }


    #region spawn

    public Sprite crackedBlockLeft, crackedBlockRight   ;

    DifficultyStageCustomizer[] difficulties;

    public float platformMoveSpeed;

    int spawnIterationCount = 0;

    void CheckWhetherToSpawn() {
        if(topPlatformPos.y < 1.5f * screenTop) {
            Spawner(topPlatformPos);
        }
    }

    Vector3 topPlatformPos;

    public int currentDifficultyIndex = 0;

    public GameObject starPrefab;

    List<PlatformProperties> ppList = new List<PlatformProperties>();
        

    void Spawner(Vector3 spawnOrigin) {

        //See if difficulty should increase
        if (currentDifficultyIndex < difficulties.Length-1) {
            if (tipScore > difficulties[currentDifficultyIndex + 1].scoreUntilTrigger) {
                currentDifficultyIndex++;
                platformMoveSpeed = difficulties[currentDifficultyIndex].moveSpeed;
            }
        }

		Vector3 spawnLocation = new Vector3(0, spawnOrigin.y);
		for(int i = 0; i < difficulties[currentDifficultyIndex].platformCount; i++) {
			spawnLocation = new Vector3(Random.Range(screenLeft, screenRight),spawnLocation.y + Random.Range(difficulties[currentDifficultyIndex].spawnMin, difficulties[currentDifficultyIndex].spawnMax));
            GameObject ins = Instantiate(platformPrefab, spawnLocation, Quaternion.identity, GameObject.Find("Platforms").transform);
			PlatformProperties pp = ins.GetComponent<PlatformProperties>();
            //Spawn Color
            int r = Random.Range(0, 100);
			if(r < difficulties[currentDifficultyIndex].colorPercentage) {
                r = Random.Range(0, difficulties[currentDifficultyIndex].colorPercentage - 1);
				Color pColor = colors[r / (difficulties[currentDifficultyIndex].colorPercentage / colors.Length)];
				pp.platformColor = pColor;
				ins.GetComponentInChildren<SpriteRenderer>().color = new Color(pColor.r,pColor.g,pColor.b,1f);
			}
			if(i == difficulties[currentDifficultyIndex].platformCount - 1) {
				topPlatformPos = ins.transform.position;
			}

			r = Random.Range(0, 100);

            //Spawn Moving
			if(r < difficulties[currentDifficultyIndex].movePercentage) {
				pp.isMove = true;
			}

            r = Random.Range(0, 100);

            //Star
            if (r < difficulties[currentDifficultyIndex].starPercentage) {
                spawnLocation = new Vector3(Random.Range(screenLeft, screenRight), spawnLocation.y + Random.Range(difficulties[currentDifficultyIndex].spawnMin, difficulties[currentDifficultyIndex].spawnMax));
                Instantiate(starPrefab, spawnLocation, Quaternion.identity, this.gameObject.transform);
            }

            r = Random.Range(0, 100);

            //Cracked
            if (r < difficulties[currentDifficultyIndex].crackedPercentage) {
                pp.isCracked = true;
            }
        }

        spawnIterationCount++;  
    }

    public void UseStar() {
        if(isStar) {
            StopCoroutine("StarTime");
            StartCoroutine("StarTime");
        } else {
            StartCoroutine("StarTime");
        }

    }

    public GameObject platformList;

    public float defaultJumpAmount;
    public float starJumpAmount;
    public float starDuration;

    public bool isStar = false;
    public float warnAnimationDuration;
    float starTimeLeft;

    IEnumerator StarTime() {
        starTimeLeft = starDuration;
        isStar = true;
        play.jumpAmount = starJumpAmount;

        player.GetComponentInChildren<rainbowColorScript>().isOn = true;

        yield return new WaitForSeconds(starDuration - warnAnimationDuration);

        StartCoroutine("StarWarning");

        yield return new WaitForSeconds(warnAnimationDuration);

        isStar = false;
        play.jumpAmount = defaultJumpAmount;

        player.GetComponentInChildren<rainbowColorScript>().isOn = false;
        play.ResetColorFromStar();
    }

    public GameObject warning;
    bool checkStarTimeLeft = false;

    Animator anim;

    IEnumerator StarWarning() {
        checkStarTimeLeft = true;
        anim = warning.GetComponent<Animator>();
        warning.SetActive(true);
        anim.enabled = true;
        yield return new WaitForSeconds(warnAnimationDuration);
        ResetWarningAnimation();
    }

    void ResetWarningAnimation() {
        warning.SetActive(false);
        anim.enabled = false;
        checkStarTimeLeft = false;
    }

    bool tripped = false;

    void CheckStarTimeLeft() {
        if(starTimeLeft > warnAnimationDuration) {
            StopCoroutine("StarWarning");
            ResetWarningAnimation();
            tripped = true;
        }else if (tripped) {
            StartCoroutine("StarWarning");
            tripped = false;
        }
        

    }

    

    #endregion

    #region gamestate

    void CheckIfDead() {
        if (player.transform.position.y < screenBottom)
            GameOver();
    }

    void StartGame() {
        Spawner(startPlatform.transform.position);
        allowCheckSpawn = true;
        StartCoroutine("StartCountdown");
    }

    void OnGameStart() {
        play.allowTilt = true;
    }

    public void GameOver() {
        RecordScore();
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    #endregion

    #region canvases
        public Text scoreText;
        public Canvas gameCanvas;


        int tipScore = 0;
        public float scoreInflation;
    
        public void UpdateScore() {
            int score = Mathf.RoundToInt(player.transform.position.y * scoreInflation);
            if (score > tipScore) {
                tipScore = score;
                scoreText.text = tipScore.ToString();
            } 
        }
        
        void RecordScore() {
            if(tipScore > PlayerPrefs.GetInt("HighScore"))
                PlayerPrefs.SetInt("HighScore", tipScore);
            PlayerPrefs.SetInt("PreviousScore", tipScore);
        }

        public Text highScore, previousScore;

        void DisplayHighScoreAndPreviousScore() {
            highScore.text = PlayerPrefs.GetInt("HighScore").ToString();
            previousScore.text = PlayerPrefs.GetInt("PreviousScore").ToString();              
        }        

        public void EnablePlatformColliders() {
            BroadcastMessage("EnableCollider");
        }
        public void DisablePlatformColliders() {
            BroadcastMessage("DisableCollider");
        }


        public Button pauseButton;
        public GameObject pausePanel;
        public Text countdownText;

        public void PauseButtonPress() {
            pauseButton.gameObject.SetActive(false);
            pausePanel.SetActive(true);
            play.allowTouch = false;
            Time.timeScale = 0;       
        }
        public void ResumeButtonPress() {
            StartCoroutine("ResumeCountdown");
        }

        public void ReturnButtonPress() {

        }

        [SerializeField]
        int resumeCountdownSeconds;

        IEnumerator ResumeCountdown() {
            pausePanel.gameObject.SetActive(false);
                
            countdownText.gameObject.SetActive(true);
            play.allowTouch = false;
                
            for (int i = resumeCountdownSeconds; i > 0; i--) {
                countdownText.text = i.ToString();
                yield return new WaitForSecondsRealtime(1f);
            }
        
            Time.timeScale = 1;
            play.allowTouch = true;
            countdownText.gameObject.SetActive(false);
            pauseButton.gameObject.SetActive(true);
        }
        IEnumerator StartCountdown() {
            gameCanvas.gameObject.SetActive(false);

            countdownText.gameObject.SetActive(true);
            play.allowTouch = false;

            for (int i = resumeCountdownSeconds; i > 0; i--) {
                countdownText.text = i.ToString();
                yield return new WaitForSecondsRealtime(1f);
            }

            Time.timeScale = 1;
            play.allowTouch = true;
            countdownText.gameObject.SetActive(false);
            gameCanvas.gameObject.SetActive(true);
            play.rb.constraints = RigidbodyConstraints.None;
            play.rb.freezeRotation = true;

            OnGameStart();
    }

    [SerializeField]
        float spreadSpeed;

        bool isSpread = false;
        public Canvas startMenuCanvas;
        Transform[] canvasElements;

        public void StartButtonPress() {
            StartCoroutine("SpreadStartMenu");
        }

        IEnumerator SpreadStartMenu() {
            isSpread = true;
            canvasElements = startMenuCanvas.GetComponentsInChildren<Transform>();

            StartGame();

            yield return new WaitForSeconds(0.2f);

            for (int i = 0; i < canvasElements.Length; i++) {
                Destroy(canvasElements[i].gameObject);
            }
            isSpread = false;
            startMenuCanvas.gameObject.SetActive(false);

        }

        void SpreadMovement() {
            if (isSpread) {
                for (int i = 0; i < canvasElements.Length; i++) {
                    switch (canvasElements[i].gameObject.tag) {
                        case "SpreadLeft":
                            canvasElements[i].Translate(Vector2.left * spreadSpeed * Time.deltaTime);
                            break;
                        case "SpreadRight":
                            canvasElements[i].Translate(Vector2.right * spreadSpeed * Time.deltaTime);
                            break;
                        case "SpreadDown":
                            canvasElements[i].Translate(Vector2.down * spreadSpeed * Time.deltaTime);
                            break;
                        case "SpreadUp":
                            canvasElements[i].Translate(Vector2.up * spreadSpeed * Time.deltaTime);
                            break;
                    }
                }
            }
        }
        #endregion
}
