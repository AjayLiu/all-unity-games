using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PowerUp : MonoBehaviour {
	public Sprite speed, shrink, expand, shield, freeze, helpWall, slowness;

	public Image redPowerUpImage;
	public Image bluePowerUpImage;

	public AudioSource dingSound;

	public MeshRenderer rend;

	bool isRed = false;
	bool isBlue = false;

	// Use this for initialization
	void Start () {
		rend = GetComponent<MeshRenderer>();
	}

	float timerForRandomRoll = 0;
	bool tickTimer = false;

	float switchInterval = 0.08f;

	float switchPicTimer;

	int timesThePicSwapped = 0;

	bool firstSwap = false;

	// Update is called once per frame
	void Update () {

		if(decreaseFillAmount){
			durationBarImage.fillAmount = powerUpTimer / powerUpDuration;

			if (isRed) {
				durationBar.color = Color.red;
			} 
			if(isBlue){
				durationBar.color = Color.cyan;
			}
		}

		if(tickThePowerUpTimer){
			powerUpTimer += Time.deltaTime;
		} else{
			powerUpTimer = 0;
		}


		if(tickTimer){
			timerForRandomRoll += Time.deltaTime;
		}

		if(timerForRandomRoll <= 2f && timerForRandomRoll != 0){
			
			switchPicTimer += Time.deltaTime;

			if(switchPicTimer >= switchInterval){
				timesThePicSwapped++;
				switchPicTimer = 0;
				ChangePicture();
			}

			if(timesThePicSwapped == 1){
				firstSwap = true;
			} else {
				firstSwap = false;
			}
		}

		if(timerForRandomRoll >= 2f){
			
			tickTimer = false;
			timerForRandomRoll = 0;
		}

		if(checkIfThisIsLastPower){
			elapsedTimeOfPowerUp += Time.deltaTime;
		} else {
			elapsedTimeOfPowerUp = 0;
		}
		// Types: 0 speed, 1 shrink, 2 expand, 3 shield, 4 freeze, 5 helpWall, 6 slowness
		if(elapsedTimeOfPowerUp >= 2f){
			switch(displayTypeOfPower){
				case 0:
					isSpeed = true;
					tickThePowerUpTimer = true;
					break;
				case 1:
					isShrink = true;
					tickThePowerUpTimer = true;
					break;
				case 2:
					isExpand = true;
					tickThePowerUpTimer = true;
					break;
				case 3:
					isShield = true;
					tickThePowerUpTimer = true;
					break;
				case 4:
					isFreeze = true;
					tickThePowerUpTimer = true;
					break;
				case 5:
					isHelpWall = true;
					tickThePowerUpTimer = true;
					break;
				case 6:
					isSlowness = true;
					tickThePowerUpTimer = true;
					break;
			}

		}


		if(isSpeed){
			if(powerUpTimer >= powerUpDuration){
				powerUpTimer = 0;
				tickThePowerUpTimer = false;
				print("disable timer");
				powerUpSpawner.SpawnPowerOrb();

				checkIfThisIsLastPower = false;

				if(isRed){
					redMoveScript.speed = 2800f;
					redPowerUpImage.sprite = null;
					powerUpText.text = "";

					durationBarImage.enabled = false;
					durationBar.enabled = false;

					decreaseFillAmount = false;
				}

				if(isBlue){
					blueMoveScript.speed = 2800f;
					bluePowerUpImage.sprite = null;
					powerUpText.text = "";

					durationBarImage.enabled = false;
					durationBar.enabled = false;

					decreaseFillAmount = false;
				}

				isRed = false;
				isBlue = false;

				isSpeed = false;
			}

			if(powerUpTimer < powerUpDuration && powerUpTimer != 0){
				if(isRed){
					redMoveScript.speed = 3500f;
					powerUpText.text = "Red Got Speed!";
					powerUpText.color = Color.red;

					durationBarImage.enabled = true;
					durationBar.enabled = true;

					decreaseFillAmount = true;
				}

				if(isBlue){
					blueMoveScript.speed = 3500f;
					powerUpText.text = "Blue Got Speed!";
					powerUpText.color = Color.cyan;

					durationBarImage.enabled = true;
					durationBar.enabled = true;

					decreaseFillAmount = true;
				}
			}
		}

		if(isShrink){
			if(powerUpTimer >= powerUpDuration){
				powerUpTimer = 0;
				tickThePowerUpTimer = false;
				print("disable timer");
				powerUpSpawner.SpawnPowerOrb();

				checkIfThisIsLastPower = false;

				if(isRed){
					redTransform.localScale = new Vector3(4, 4, 4);
					redTransform.position = new Vector3(redTransform.position.x, 2f, redTransform.position.z);
					redPowerUpImage.sprite = null;
					powerUpText.text = "";

					durationBarImage.enabled = false;
					durationBar.enabled = false;

					decreaseFillAmount = false;

				}

				if(isBlue){
					blueTransform.localScale = new Vector3(4, 4, 4);
					blueTransform.position = new Vector3(blueTransform.position.x, 2f, blueTransform.position.z);
					bluePowerUpImage.sprite = null;
					powerUpText.text = "";

					durationBarImage.enabled = false;
					durationBar.enabled = false;

					decreaseFillAmount = false;
				}

				isRed = false;
				isBlue = false;

				isShrink = false;
			}

			if(powerUpTimer < powerUpDuration && powerUpTimer != 0){
				if(isRed){
					redTransform.localScale = new Vector3(2, 2, 2);
					redTransform.position = new Vector3(redTransform.position.x, 1f, redTransform.position.z);

					powerUpText.text = "Red is Shrunken!";
					powerUpText.color = Color.red;

					durationBarImage.enabled = true;
					durationBar.enabled = true;

					decreaseFillAmount = true;
				}

				if(isBlue){
					blueTransform.localScale = new Vector3(2, 2, 2); 
					blueTransform.position = new Vector3(blueTransform.position.x, 2f, blueTransform.position.z);

					powerUpText.text = "Blue is Shrunken!";
					powerUpText.color = Color.cyan;

					durationBarImage.enabled = true;
					durationBar.enabled = true;

					decreaseFillAmount = true;
				}
			}
		}

		if(isExpand){
			if(powerUpTimer >= powerUpDuration){
				powerUpTimer = 0;
				tickThePowerUpTimer = false;
				print("disable timer");
				powerUpSpawner.SpawnPowerOrb();

				checkIfThisIsLastPower = false;

					if(isRed){

					redTransform.localScale = new Vector3(4, 4, 4);
					redTransform.position = new Vector3(redTransform.position.x, 2f, redTransform.position.z);
					redPowerUpImage.sprite = null;
					powerUpText.text = "";

					durationBarImage.enabled = false;
					durationBar.enabled = false;

					decreaseFillAmount = false;
				}

				if(isBlue){
					blueTransform.localScale = new Vector3(4, 4, 4);
					blueTransform.position = new Vector3(blueTransform.position.x, 2f, blueTransform.position.z);
					bluePowerUpImage.sprite = null;
					powerUpText.text = "";

					durationBarImage.enabled = false;
					durationBar.enabled = false;

					decreaseFillAmount = false;
				}


				isRed = false;
				isBlue = false;

				isExpand = false;
			}

			if(powerUpTimer < powerUpDuration && powerUpTimer != 0){
				if(isRed){
					redTransform.localScale = new Vector3(8, 8, 8);
					redTransform.position = new Vector3(redTransform.position.x, 4f, redTransform.position.z);
					powerUpText.text = "Red Expanded!";
					powerUpText.color = Color.red;

					durationBarImage.enabled = true;
					durationBar.enabled = true;

					decreaseFillAmount = true;
				}

				if(isBlue){
					blueTransform.localScale = new Vector3(8, 8, 8); 
					blueTransform.position = new Vector3(blueTransform.position.x, 4f, blueTransform.position.z);

					powerUpText.text = "Blue Expanded!";
					powerUpText.color = Color.cyan;

					durationBarImage.enabled = true;
					durationBar.enabled = true;

					decreaseFillAmount = true;
				}
			}
		}

		if(isShield){
			if(powerUpTimer >= powerUpDuration){
				powerUpTimer = 0;
				tickThePowerUpTimer = false;
				print("disable timer");
				powerUpSpawner.SpawnPowerOrb();

				checkIfThisIsLastPower = false;

				if(isRed){
					redGoalShield.SetActive(false);
					redPowerUpImage.sprite = null;
					powerUpText.text = "";

					durationBarImage.enabled = false;
					durationBar.enabled = false;

					decreaseFillAmount = false;
				}

				if(isBlue){
					blueGoalShield.SetActive(false);
					bluePowerUpImage.sprite = null;
					powerUpText.text = "";

					durationBarImage.enabled = false;
					durationBar.enabled = false;

					decreaseFillAmount = false;
				}


				isRed = false;
				isBlue = false;

				isShield = false;
			}

			if(powerUpTimer < powerUpDuration && powerUpTimer != 0){
				if(isRed){
					redGoalShield.SetActive(true);
					powerUpText.text = "Red Got Goal Shield!";
					powerUpText.color = Color.red;

					durationBar.enabled = true;
					durationBarImage.enabled = true;

					decreaseFillAmount = true;
				}

				if(isBlue){
					blueGoalShield.SetActive(true);
					powerUpText.text = "Blue Got Goal Shield!";
					powerUpText.color =	Color.cyan;

					durationBarImage.enabled = true;
					durationBar.enabled = true;

					decreaseFillAmount = true;
				}
			}
		}

		if(isFreeze){
			if(powerUpTimer >= powerUpDuration){
				powerUpTimer = 0;
				tickThePowerUpTimer = false;
				print("disable timer");
				powerUpSpawner.SpawnPowerOrb();

				checkIfThisIsLastPower = false;

				if(isRed){
					redMoveScript.speed = 2800f;
					redPowerUpImage.sprite = null;
					powerUpText.text = "";

					durationBarImage.enabled = false;
					durationBar.enabled = false;

					decreaseFillAmount = false;
				}

				if(isBlue){
					blueMoveScript.speed = 2800f;
					bluePowerUpImage.sprite = null;
					powerUpText.text = "";

					durationBarImage.enabled = false;
					durationBar.enabled = false;

					decreaseFillAmount = false;
				}


				isRed = false;
				isBlue = false;

				isFreeze = false;
			}

			if(powerUpTimer < powerUpDuration && powerUpTimer != 0){
				if(isRed){
					redMoveScript.speed = 0f;
					powerUpText.text = "Red is Frozen!";
					powerUpText.color = Color.red;
		
					durationBarImage.enabled = true;
					durationBar.enabled = true;

					decreaseFillAmount = true;

				}

				if(isBlue){
					blueMoveScript.speed = 0f;
					powerUpText.text = "Blue is Frozen!";
					powerUpText.color = Color.cyan;

					durationBarImage.enabled = true;
					durationBar.enabled = true;

					decreaseFillAmount = true;
				}
			}
		}

		if(isHelpWall){
			if(powerUpTimer >= powerUpDuration){
				powerUpTimer = 0;
				tickThePowerUpTimer = false;
				print("disable timer");
				powerUpSpawner.SpawnPowerOrb();

				checkIfThisIsLastPower = false;

				if(isRed){
					redPowerUpImage.sprite = null;
					powerUpText.text = "";

					blueGoalAssistWall.SetActive (false);
					blueGoalAssistWall2.SetActive (false);

					durationBarImage.enabled = false;
					durationBar.enabled = false;

					decreaseFillAmount = false;
					
				}

				if(isBlue){
					bluePowerUpImage.sprite = null;
					powerUpText.text = "";

					redGoalAssistWall.SetActive (false);
					redGoalAssistWall2.SetActive (false);

					durationBarImage.enabled = false;
					durationBar.enabled = false;

					decreaseFillAmount = false;
				}

				isRed = false;
				isBlue = false;

				isHelpWall = false;
			}

			if(powerUpTimer < powerUpDuration && powerUpTimer != 0){
				if(isRed){
					powerUpText.text = "Red Got Support Walls!";
					powerUpText.color = Color.red;

					blueGoalAssistWall.SetActive (true);
					blueGoalAssistWall2.SetActive (true);

					durationBarImage.enabled = true;
					durationBar.enabled = true;

					decreaseFillAmount = true;
					
				}

				if(isBlue){
					powerUpText.text = "Blue Got Support Walls!";
					powerUpText.color = Color.cyan;

					redGoalAssistWall.SetActive (true);
					redGoalAssistWall2.SetActive (true);

					durationBarImage.enabled = true;
					durationBar.enabled = true;

					decreaseFillAmount = true;
				}
			}
		}

		if(isSlowness){
			if(powerUpTimer >= powerUpDuration){
				powerUpTimer = 0;
				tickThePowerUpTimer = false;
				print("disable timer");
				powerUpSpawner.SpawnPowerOrb();

				checkIfThisIsLastPower = false;

				if(isRed){
					redMoveScript.speed = 2800f;
					redPowerUpImage.sprite = null;
					powerUpText.text = "";

					durationBarImage.enabled = false;
					durationBar.enabled = false;

					decreaseFillAmount = false;
				}

				if(isBlue){
					blueMoveScript.speed = 2800f;
					bluePowerUpImage.sprite = null;
					powerUpText.text = "";

					durationBarImage.enabled = false;
					durationBar.enabled = false;

					decreaseFillAmount = false;
				}


				isRed = false;
				isBlue = false;

				isSlowness = false;
			}

			if(powerUpTimer < powerUpDuration && powerUpTimer != 0){
				if(isRed){
					redMoveScript.speed = 1000f;

					powerUpText.text = "Red Got Slowness!";
					powerUpText.color = Color.red;

					durationBarImage.enabled = true;
					durationBar.enabled = true;

					decreaseFillAmount = true;
				}

				if(isBlue){
					blueMoveScript.speed = 1000f;

					powerUpText.text = "Blue Got Slowness!";
					powerUpText.color = Color.cyan;

					durationBarImage.enabled = true;
					durationBar.enabled = true;

					decreaseFillAmount = true;
				}
			}
		}
	}

	float elapsedTimeOfPowerUp;

	// Types: 0 speed, 1 shrink, 2 expand, 3 shield, 4 freeze, 5 helpWall, 6 slowness
	public bool isFirstCollide = true;

	int randomStart;

	void OnTriggerEnter(Collider other){

		rend.enabled = false;

		if(isFirstCollide){
			
			randomStart = Random.Range(0, 7);

			dingSound.Play();

			if(other.gameObject.CompareTag("CubeRed")){
				isRed = true;
				tickTimer = true;
			}
			if(other.gameObject.CompareTag("CubeBlue")){
				isBlue = true;
				tickTimer = true;
			}

			isFirstCollide = false;
		}
	}


	public int displayTypeOfPower;

	public bool checkIfThisIsLastPower;

	void ChangePicture(){

		if(firstSwap){
			displayTypeOfPower = randomStart;
		} else {
			checkIfThisIsLastPower = false;

			displayTypeOfPower++;
		}
		if(displayTypeOfPower >= 7){
			displayTypeOfPower = 0;
		}
		// Types: 0 speed, 1 shrink, 2 expand, 3 shield, 4 freeze, 5 helpWall, 6 slowness
		if(isRed){
			switch(displayTypeOfPower){
				case 0:
					redPowerUpImage.sprite = speed;
					checkIfThisIsLastPower = true;
					break;
				case 1:
					redPowerUpImage.sprite = shrink;
					checkIfThisIsLastPower = true;
					break;
				case 2:
					redPowerUpImage.sprite = expand;
					checkIfThisIsLastPower = true;
					break;
				case 3:
					redPowerUpImage.sprite = shield;
					checkIfThisIsLastPower = true;
					break;
				case 4:
					redPowerUpImage.sprite = freeze;
					checkIfThisIsLastPower = true;
					break;
				case 5:
					redPowerUpImage.sprite = helpWall;
					checkIfThisIsLastPower = true;
					break;
				case 6:
					redPowerUpImage.sprite = slowness;
					checkIfThisIsLastPower = true;
					break;
			}
		}
		if(isBlue){
			switch(displayTypeOfPower){
				case 0:
					bluePowerUpImage.sprite = speed;
					checkIfThisIsLastPower = true;
					break;
				case 1:
					bluePowerUpImage.sprite = shrink;
					checkIfThisIsLastPower = true;
					break;
				case 2:
					bluePowerUpImage.sprite = expand;
					checkIfThisIsLastPower = true;
					break;
				case 3:
					bluePowerUpImage.sprite = shield;
					checkIfThisIsLastPower = true;
					break;
				case 4:
					bluePowerUpImage.sprite = freeze;
					checkIfThisIsLastPower = true;
					break;
				case 5:
					bluePowerUpImage.sprite = helpWall;
					checkIfThisIsLastPower = true;
					break;
				case 6:
					bluePowerUpImage.sprite = slowness;
					checkIfThisIsLastPower = true;
					break;
			}
		}
	}

	public Text powerUpText;

	public PlayerRedScript redMoveScript;
	public PlayerBlueScript blueMoveScript;

	public Transform redTransform;
	public Transform blueTransform;

	public GameObject redGoalShield;
	public GameObject blueGoalShield;

	public GameObject blueGoalAssistWall, blueGoalAssistWall2;
	public GameObject redGoalAssistWall, redGoalAssistWall2;

	float powerUpTimer;
	public bool tickThePowerUpTimer = false;
	public float powerUpDuration = 10f;

	public Image durationBar;
	public Image durationBarImage;

	bool decreaseFillAmount = false;
	// Types: 0 speed, 1 shrink, 2 expand, 3 shield, 4 freeze, 5 helpWall, 6 slowness

	public PowerUpSpawner powerUpSpawner;

	bool isSpeed = false, isShrink = false, isExpand = false, isShield = false, isFreeze = false, isHelpWall = false, isSlowness = false;
}
