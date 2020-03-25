using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum Status { Flying, Spinning, Starting };

public class GameCon : MonoBehaviour {

    void Awake() {
        GameSetup();
    }

    // Use this for initialization
    void Start () {
        ShowStartMenu();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        switch(status) {
            case Status.Starting:
                CameraTrackForStart();
                break;
            case Status.Spinning:
                CameraTrackForSpinning();
                break;
            case Status.Flying:
                CameraTrackForFlying();
                break;
            default:
                break;
        }
    }
    
    void Update() {
        CheckScores();
        CheckGameOver();
        SpreadMovement();
        AskRevive();
    }

    public GameObject inputSelectCanvas;
    public bool isKeyboard;

    public void SetToKeyboard() {
        isKeyboard = true;
        inputSelectCanvas.SetActive(false);
    }
    public void SetToMobile() {
        isKeyboard = false;
        inputSelectCanvas.SetActive(false);
    }


    #region start

    [HideInInspector]public bool isGameSetup = true;

    public Status status = Status.Starting;

    public GameObject REDMARKER, BLUEMARKER;

    [HideInInspector]
    public List<GameObject> everything = new List<GameObject>();

    [SerializeField]
    GameObject spinnerPrefab, playerPrefab, bumperPrefab, starPrefab, obstaclePrefab;

    [HideInInspector]
    public GameObject player;
    GameObject firstSpinner;
    PlayerScript play;

    [SerializeField]
    Vector3 playerSpawnPos, startSpinnerPos;

    public Vector3 spaceDimensions; // the whole map's size. ie: (5, 5) is 5x5 size world

    [HideInInspector]public bool lockInput = true;

    void GameSetup() {
        SetColors();
        reviveButton.gameObject.SetActive(false);
        leftSteerButton.gameObject.SetActive(false);
        rightSteerButton.gameObject.SetActive(false);
        upSteerButton.gameObject.SetActive(false);

        lockInput = true;

        cam = Camera.main;

        startZoom = cam.orthographicSize;

        player = Instantiate(playerPrefab, playerSpawnPos, Quaternion.identity);

        firstSpinner = Instantiate(spinnerPrefab, startSpinnerPos, Quaternion.identity);

        everything.Add(player);
        everything.Add(firstSpinner);

		play = player.GetComponent<PlayerScript>();

        play.currentSpinner = firstSpinner;

        player.GetComponent<HingeJoint2D>().connectedBody = firstSpinner.GetComponent<Rigidbody2D>();

        //$$$

        NewSpace(howManyRowsOfSquads, squadSize, Random.Range(spinnersInEachSquadMIN, spinnersInEachSquadMAX), Random.Range(bumpersInEachSquadMIN, bumpersInEachSquadMAX), Random.Range(starsInEachSquadMIN, starsInEachSquadMAX),player.transform.position, true);

        status = Status.Starting;
    }

    float waitTimeInterval = 0.5f;
    int waitTimeAmount = 3;

    [SerializeField]Text countdownText;

    IEnumerator GameStart() {
        gameCanvas.gameObject.SetActive(true);
        StartCoroutine("SpreadStartMenu");
        countdownText.gameObject.SetActive(true);

        for (int i  = waitTimeAmount; i > 0; i--) {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(waitTimeInterval);
        }

        countdownText.gameObject.SetActive(false);

        status = Status.Spinning;
        lockInput = false;
    }

    [SerializeField]
    float spreadSpeed;

    bool isSpread = false;

    Transform[] canvasElements;

    IEnumerator SpreadStartMenu() {
        isSpread = true;
        canvasElements = startMenuCanvas.GetComponentsInChildren<Transform>();

        yield return new WaitForSeconds(0.5f);

        for(int i = 0; i < canvasElements.Length; i++) {
            Destroy(canvasElements[i].gameObject);
        }
        isSpread = false;
        startMenuCanvas.gameObject.SetActive(false);
    }

    void SpreadMovement() {
        if(isSpread) {
            for (int i = 0; i < canvasElements.Length; i++) {
                switch (canvasElements[i].gameObject.tag) {
                    case "SpreadLeft":
                        canvasElements[i].Translate( Vector2.left *spreadSpeed * Time.deltaTime);
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

    #region spawn

    float EVERYTHING_DEFAULT_SIZE = 0.1f;
    [SerializeField]float spawnBuffer;
    [SerializeField]
	int howManyRowsOfSquads, spinnersInEachSquadMIN, spinnersInEachSquadMAX, bumpersInEachSquadMIN, bumpersInEachSquadMAX, starsInEachSquadMIN, starsInEachSquadMAX, obstaclesInEachSquadMIN, obstaclesInEachSquadMAX;
	[SerializeField]float spinnerScaleMIN, spinnerScaleMAX, bumperScaleMIN, bumperScaleMAX, obstacleScaleMIN, obstacleScaleMAX;

    [SerializeField]float squadSize;

    void NewSpace(int howManySquads, float eachSquadSize, int spinnersPerSquad, int bumpersPerSquad, int starsPerSquad, Vector3 centerSpawn, bool isStart)
	{

		//clear everything
		if(!isStart) {
			for(int i = 0; i < everything.Count; i++) {
				if(!everything[i].CompareTag("Player") && everything[i] != play.currentSpinner) {
					Destroy(everything[i]);
					everything.RemoveAt(i);
				}
			}
		}

		spaceDimensions.x = howManySquads * eachSquadSize;
		spaceDimensions.y = howManySquads * eachSquadSize;

		//in every squad
		Vector3 squadPointer = new Vector3(-spaceDimensions.x / 2 + centerSpawn.x,-spaceDimensions.y / 2 + centerSpawn.y);
		List<GameObject> squadObjs = new List<GameObject>();

		squadObjs.Add(player);
		squadObjs.Add(play.currentSpinner);

		Instantiate(REDMARKER, squadPointer, Quaternion.identity);

		for(int row = 0; row < howManySquads; row++) {
			for(int col = 0; col < howManySquads; col++) {
				for(int spinnersSpawned = 0; spinnersSpawned < spinnersPerSquad; spinnersSpawned++) {
					Vector3 loc = new Vector3(Random.Range(squadPointer.x + spawnBuffer / 2, squadPointer.x + eachSquadSize - spawnBuffer / 2),Random.Range(squadPointer.y + spawnBuffer / 2, squadPointer.y + eachSquadSize - spawnBuffer / 2));
                    while(!VerifyLocation(loc, squadObjs.ToArray())) {
                        print("UNVERIFIED");
                        loc = new Vector3(Random.Range(squadPointer.x + spawnBuffer / 2, squadPointer.x + eachSquadSize - spawnBuffer / 2), Random.Range(squadPointer.y + spawnBuffer / 2, squadPointer.y + eachSquadSize - spawnBuffer / 2));
                    }
                    GameObject ins = Instantiate(spinnerPrefab, loc, Quaternion.identity);
					float randScaleNum = Random.Range(spinnerScaleMIN, spinnerScaleMAX);
                    ins.transform.localScale = new Vector3(randScaleNum, randScaleNum);
                    everything.Add(ins);
                    squadObjs.Add(ins);
                }
                for (int bumpersSpawned = 0; bumpersSpawned < bumpersPerSquad; bumpersSpawned++) {
                    Vector3 loc = new Vector3(Random.Range(squadPointer.x + spawnBuffer / 2, squadPointer.x + eachSquadSize - spawnBuffer / 2), Random.Range(squadPointer.y + spawnBuffer / 2, squadPointer.y + eachSquadSize - spawnBuffer / 2));
                    while (!VerifyLocation(loc, squadObjs.ToArray())) {
                        print("UNVERIFIED");
                        loc = new Vector3(Random.Range(squadPointer.x + spawnBuffer / 2, squadPointer.x + eachSquadSize - spawnBuffer / 2), Random.Range(squadPointer.y + spawnBuffer / 2, squadPointer.y + eachSquadSize - spawnBuffer / 2));
                    }
					Vector3 randScale = new Vector3(Random.Range(bumperScaleMIN, bumperScaleMAX), Random.Range(bumperScaleMIN, bumperScaleMAX));

                    GameObject ins = Instantiate(bumperPrefab, loc, Quaternion.identity);
					ins.transform.localScale = randScale;
					ins.transform.eulerAngles = new Vector3(0f, 0f, Random.Range(0f, 180f));
					everything.Add(ins);
                    squadObjs.Add(ins);
                }
				for (int starsSpawned = 0; starsSpawned < bumpersPerSquad; starsSpawned++) {
                    Vector3 loc = new Vector3(Random.Range(squadPointer.x + spawnBuffer / 2, squadPointer.x + eachSquadSize - spawnBuffer / 2), Random.Range(squadPointer.y + spawnBuffer / 2, squadPointer.y + eachSquadSize - spawnBuffer / 2));
                    while (!VerifyLocation(loc, squadObjs.ToArray())) {
                        print("UNVERIFIED");
                        loc = new Vector3(Random.Range(squadPointer.x + spawnBuffer / 2, squadPointer.x + eachSquadSize - spawnBuffer / 2), Random.Range(squadPointer.y + spawnBuffer / 2, squadPointer.y + eachSquadSize - spawnBuffer / 2));
                    }

                    GameObject ins = Instantiate(starPrefab, loc, Quaternion.identity);
					everything.Add(ins);
                    squadObjs.Add(ins);
                }
				for (int obsSpawned = 0; obsSpawned < bumpersPerSquad; obsSpawned++) {
                    Vector3 loc = new Vector3(Random.Range(squadPointer.x + spawnBuffer / 2, squadPointer.x + eachSquadSize - spawnBuffer / 2), Random.Range(squadPointer.y + spawnBuffer / 2, squadPointer.y + eachSquadSize - spawnBuffer / 2));
                    while (!VerifyLocation(loc, squadObjs.ToArray())) {
                        print("UNVERIFIED");
                        loc = new Vector3(Random.Range(squadPointer.x + spawnBuffer / 2, squadPointer.x + eachSquadSize - spawnBuffer / 2), Random.Range(squadPointer.y + spawnBuffer / 2, squadPointer.y + eachSquadSize - spawnBuffer / 2));
                    }

                    GameObject ins = Instantiate(obstaclePrefab, loc, Quaternion.identity);


                    float randScale = Random.Range(obstacleScaleMIN, obstacleScaleMAX);
                    ins.transform.localScale = new Vector3(randScale, randScale);
                    ins.transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 180));
					everything.Add(ins);
                    squadObjs.Add(ins);
                }
                squadObjs.Clear();
                squadObjs.Add(player);
                squadObjs.Add(play.currentSpinner);

                squadPointer.x += eachSquadSize;
            }
            squadPointer.x = -spaceDimensions.x / 2 + centerSpawn.x;
            squadPointer.y += eachSquadSize;
        }
    }

    bool VerifyLocation(Vector3 location, GameObject[] squadObjects) {
        //check if location is valid (if its too close to other objects)

        bool allFine = true;

        for (int j = 0; j < squadObjects.Length; j++) {
            Vector3 diff = new Vector3(Mathf.Abs(location.x - squadObjects[j].transform.position.x), Mathf.Abs(location.y - squadObjects[j].transform.position.y));
            if (diff.x < EVERYTHING_DEFAULT_SIZE * squadObjects[j].transform.localScale.x + spawnBuffer || diff.y < EVERYTHING_DEFAULT_SIZE * squadObjects[j].transform.localScale.x + spawnBuffer) {
                allFine = false;
                break;
            }
        }

        if (allFine)
            return true;
        else
            return false;
    
    }
    #endregion

    #region color

    public List<ColorCombo> cCombos = new List<ColorCombo>();

    public static Material material1;
    public static Material material2;
    public static Material material3;
    public static Material material4;
    public static Material material5;

    [SerializeField]
    Button startButton;

    void SetColors() {
        int randSet = Random.Range(0, cCombos.Count - 1);
        print(randSet);
        material1 = cCombos[randSet].m1;
        material2 = cCombos[randSet].m2;
        material3 = cCombos[randSet].m3;
        material4 = cCombos[randSet].m4;
        material5 = cCombos[randSet].m5;
    }

    #endregion

    #region camera

    Camera cam;
    [SerializeField] float cameraSpeed;
    [SerializeField]
    float spinSpeedToZoomRatio, flySpeedToZoomRatio;
    [SerializeField]
    float cameraZoomSpeedForSpinning, cameraZoomSpeedForFlying, cameraZoomSpeedForImpact;
    float startZoom;
    [SerializeField]float minZoom;
    [SerializeField]
    float cameraRotateSpeed;

    void CameraTrackForStart() {
        cam.orthographicSize = startZoom;
    }

    void CameraTrackForSpinning() {
        stayAtMinZoom = false;

        cam.transform.position = new Vector3(Mathf.Lerp(cam.transform.position.x, play.currentSpinner.transform.position.x, cameraSpeed / 2), Mathf.Lerp(cam.transform.position.y, play.currentSpinner.transform.position.y, cameraSpeed / 2), cam.transform.position.z);

        //if the camera is zooming out
        if(cam.orthographicSize < play.currentSpinSpeed / spinSpeedToZoomRatio) {
            if (play.currentSpinSpeed / spinSpeedToZoomRatio < minZoom)
                cam.orthographicSize = minZoom;
            else
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, play.currentSpinSpeed / spinSpeedToZoomRatio, cameraZoomSpeedForSpinning);
        } else {
            //zooming in
            if (cam.orthographicSize > minZoom)
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, play.currentSpinSpeed / spinSpeedToZoomRatio, cameraZoomSpeedForImpact);            
        }
    }

    bool stayAtMinZoom = false;

    void CameraTrackForFlying() {
        cam.transform.position = new Vector3(Mathf.Lerp(cam.transform.position.x, player.transform.position.x, cameraSpeed), Mathf.Lerp(cam.transform.position.y, player.transform.position.y, cameraSpeed), cam.transform.position.z);

        if (stayAtMinZoom) {
            cam.orthographicSize = minZoom;
        } else {
            if (cam.orthographicSize < minZoom)
                stayAtMinZoom = true;
            else
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, play.currentFlySpeed / flySpeedToZoomRatio, cameraZoomSpeedForFlying);
        }

        cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, player.transform.rotation,cameraRotateSpeed);
    }
    
    #endregion
    
    #region score

    [SerializeField]
    Text scoreText;
    [SerializeField]
    Text spinnersHitText;
    [SerializeField]
    Text spaceGoalText;


    [SerializeField]
    Text highScoreText, previousScoreText;

    public int score = 0;
    public int spinnersHit = 0;
    public int spaceGoal = 20;
    public int goalsReached = 0;
    int highestAchievedScore = 0;

    [SerializeField] int goalIncrementMIN, goalIncrementMAX;
    [SerializeField]
    float spinnersPerGoal, bumpersPerGoal;

    public void SpinComplete() {
        if (!play.isMaxSpeed) {
            score++;
        }
    }

    [SerializeField]int starScoreAddAmount;

    public void StarCollected() {
    	score+=starScoreAddAmount;
	}

    [SerializeField]
    float randability = 1;

    void CheckScores() {
        spinnersHitText.text = spinnersHit.ToString();
        spaceGoalText.text = spaceGoal.ToString();
        scoreText.text = score.ToString();

        //Space goal reached
        if (spinnersHit >= spaceGoal) {
            //$$$
			NewSpace(howManyRowsOfSquads, squadSize, Random.Range(spinnersInEachSquadMIN, spinnersInEachSquadMAX), Random.Range(bumpersInEachSquadMIN, bumpersInEachSquadMAX), Random.Range(starsInEachSquadMIN, starsInEachSquadMAX),player.transform.position, false);

            spaceGoal += Random.Range(goalIncrementMIN, goalIncrementMAX);
            goalsReached++;
        }

        if(score == 0) {
            lockSteer = true;
            rightSteerButton.gameObject.SetActive(false);
            leftSteerButton.gameObject.SetActive(false);
            upSteerButton.gameObject.SetActive(false);
        }

        if (score > highestAchievedScore) {
            highestAchievedScore = score;
        }
    }

    void StartMenuScores() {

        if (!PlayerPrefs.HasKey("HighScore"))
            PlayerPrefs.SetInt("HighScore", 0); 
        if(!PlayerPrefs.HasKey("PreviousScore")) {
            PlayerPrefs.SetInt("PreviousScore", 0);
        }            
          
        highScoreText.text = PlayerPrefs.GetInt("HighScore").ToString();
        previousScoreText.text = PlayerPrefs.GetInt("PreviousScore").ToString();
    }

    #endregion

    #region restart

    public void OnRestartPress() {
        Time.timeScale = 1;
        GameOver();
    }

    #endregion

    #region steering

    [SerializeField] float steerAmount;
    [HideInInspector]public bool lockSteer = true;
    [SerializeField]
    int steerCost;
    public Button leftSteerButton, rightSteerButton, upSteerButton;

    public void SteerLeftPress() {
        if(!lockSteer && score > 0) {
            score -= steerCost;
            play.rd.AddRelativeForce(Vector2.left * steerAmount, ForceMode2D.Impulse);
        }
    }

    public void SteerRightPress() {
        if (!lockSteer && score > 0) {
            score -= steerCost;
            play.rd.AddRelativeForce(Vector2.right * steerAmount, ForceMode2D.Impulse);
        }
    }

	public void SteerUpPress() {
        if (!lockSteer && score > 0) {
            score -= steerCost;
            play.rd.AddRelativeForce(Vector2.up * steerAmount * 2f, ForceMode2D.Impulse);
        }
    }

    #endregion

    #region canvases and menus

    [SerializeField]
    Canvas gameCanvas, startMenuCanvas, pauseMenuCanvas;

    [SerializeField]
    Text helpText;

    void ShowStartMenu() {
        gameCanvas.gameObject.SetActive(false);
        startMenuCanvas.gameObject.SetActive(true);

        StartMenuScores();
    }

    public void PauseGame() {
        Time.timeScale = 0;
        pauseMenuCanvas.gameObject.SetActive(true);
        gameCanvas.gameObject.SetActive(false);
    }

    public void ResumeGame() {
        StartCoroutine("ResumeCountdown");
    }

    public void StartGameButtonPress() {        
        StartCoroutine("GameStart");
    }

    [SerializeField]
    int resumeCountdownSeconds;

    IEnumerator ResumeCountdown() {
        pauseMenuCanvas.gameObject.SetActive(false);
        gameCanvas.gameObject.SetActive(true);

        countdownText.gameObject.SetActive(true);

        for (int i = resumeCountdownSeconds; i > 0; i--) {
            countdownText.text = i.ToString();
            yield return new WaitForSecondsRealtime(waitTimeInterval);
        }

        Time.timeScale = 1;
        countdownText.gameObject.SetActive(false);
    }

    #endregion

    #region game over

    public void ObstacleHit(){
    	GameOver();
    }

    [SerializeField]float gameoverSpeed;
    bool isGGSpeed = false;
    bool isCheckingGG = false;
    [SerializeField]
    int secondsTilGameover; 

    void CheckGameOver() {
        if(status == Status.Flying && Mathf.Abs(play.rd.velocity.x) < gameoverSpeed && Mathf.Abs(play.rd.velocity.y) < gameoverSpeed) {
            isGGSpeed = true;
        } else {
            isGGSpeed = false;
        }

        if(isGGSpeed) {
            if(!isCheckingGG)
                StartCoroutine("GameOverCountdown");
        } else {
            if(isCheckingGG) { 
                StopCoroutine("GameOverCountdown");
                isCheckingGG = false;
                countdownText.gameObject.SetActive(false);
            }
        }
    }

    IEnumerator GameOverCountdown() {
        isCheckingGG = true;
        countdownText.gameObject.SetActive(true);

        for(int i = secondsTilGameover; i > 0; i--) {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(waitTimeInterval);    
        }

        countdownText.gameObject.SetActive(false);

        if(!reviveUsed) {
            reviveButton.gameObject.SetActive(true);
            askForRevive = true;
            reviveImg = reviveButton.GetComponent<Image>();
        } else {
            GameOver();
        }
    }

    [SerializeField]
    float reviveAskTime;
    bool askForRevive = false;
    [SerializeField]
    Button reviveButton;
    Image reviveImg;

    float elapsed = 0;

    bool reviveUsed = false;

    void AskRevive() {
        if(askForRevive) {
            elapsed += Time.deltaTime;
            reviveImg.fillAmount = elapsed / reviveAskTime;            
            if(elapsed > reviveAskTime) {
                elapsed = 0;
                askForRevive = false;
                reviveButton.gameObject.SetActive(false);
                GameOver();
            }
        }
    }

    public void Revive() {
        reviveUsed = true;
        elapsed = 0;
        askForRevive = false;
        reviveButton.gameObject.SetActive(false);
        int randIndex = Random.Range(0, everything.Count);
        while(!everything[randIndex].CompareTag("Spinner")) {
            randIndex = Random.Range(0, everything.Count);
        }
        player.transform.position = new Vector3(everything[randIndex].transform.position.x - (spawnBuffer - 0.001f), everything[randIndex].transform.position.y);
        play.rd.AddForce(Vector2.right * 5f);        
    }

    void GameOver() {
        PlayerPrefs.SetInt("PreviousScore", highestAchievedScore);

        //compare high score
        if(highestAchievedScore > PlayerPrefs.GetInt("HighScore")) {
            PlayerPrefs.SetInt("HighScore", highestAchievedScore);
        }

        SceneManager.LoadScene("Game");
    }

    #endregion

}




[System.Serializable]
public class ColorCombo {
    public Material m1, m2, m3, m4, m5;
}
