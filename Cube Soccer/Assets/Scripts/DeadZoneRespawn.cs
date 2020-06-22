using UnityEngine;
using System.Collections;

public class DeadZoneRespawn : MonoBehaviour {

	public Transform redTransform, blueTransform;

	float timer;

	bool startTimer = false;

	// Use this for initialization
	void Start () {
		timer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (startTimer) {
			timer += Time.deltaTime;
		}

		if(timer >= 1f){
			Respawn ();
			startTimer = false;
			timer = 0;
		}
	}

	bool isRedPlayer;
	bool isBluePlayer;

	void OnTriggerEnter(Collider other){
		if(other.gameObject.CompareTag("CubeRed")){
			startTimer = true;
			isRedPlayer = true;
		}
		if(other.gameObject.CompareTag("CubeBlue")){
			startTimer = true;
			isBluePlayer = true;
		}


	}

	Vector3 spawnLocationForRed;
	Vector3 spawnLocationForBlue;

	void Respawn(){
		spawnLocationForRed = new Vector3 (20f, 2f, 0.5f);
		spawnLocationForBlue = new Vector3 (-20f, 2f, -0.5f);

		if(isRedPlayer){
			redTransform.position = spawnLocationForRed;
			isRedPlayer = false;
			print ("Respawn Red");
		}
		if(isBluePlayer){
			blueTransform.position = spawnLocationForBlue;
			isBluePlayer = false;
			print ("Respawn Blue");
		}
	}
}
