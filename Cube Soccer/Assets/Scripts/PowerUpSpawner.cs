using UnityEngine;
using System.Collections;

public class PowerUpSpawner : MonoBehaviour {

	public GameObject powerUpOrb;

	public PowerUp powerUpScript;

	Vector3 randomPos;

	bool chooseRandomTime = false;

	float timer;

	float timeToSpawn;

	bool tickTimer = false;

	void Update(){
		if(chooseRandomTime){
			timeToSpawn = Random.Range(5f, 20f);
			chooseRandomTime = false;
		}
		if(tickTimer){
			timer += Time.deltaTime;
		}

		if(timer >= timeToSpawn){

			randomPos = new Vector3(Random.Range(-25f, 25f), powerUpOrb.transform.position.y, Random.Range(-20f, 20f));

			powerUpOrb.transform.position = randomPos;

			powerUpScript.isFirstCollide = true;

			powerUpScript.rend.enabled = true;

			powerUpScript.checkIfThisIsLastPower = false;

			timer = 0;

			chooseRandomTime = false;

			tickTimer = false;
		}
	}

	void Start(){
		SpawnPowerOrb();
	}

	public void SpawnPowerOrb(){

		tickTimer = true;

		chooseRandomTime = true;
	}
}
