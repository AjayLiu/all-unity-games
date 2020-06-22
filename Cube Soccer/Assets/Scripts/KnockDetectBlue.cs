using UnityEngine;
using System.Collections;

public class KnockDetectBlue : MonoBehaviour {

	[SerializeField]Collider redLeft, redRight, redUp, redDown;
	[SerializeField]GameObject BlueCube;

	// Use this for initialization
	void Start () {
		timer = 0;
	}
		
	public float knockRange;
	public float knockTime;
	Vector3 knockToLocation;
	float timer;

	void Update () {

		if (fromRight && !fromDown && !fromLeft && !fromUp) {
	
			knockToLocation = new Vector3 (BlueCube.transform.position.x + knockRange, transform.position.y, BlueCube.transform.position.z);

			BlueCube.transform.position = Vector3.Lerp(BlueCube.transform.position, knockToLocation, 0.2f);

			timer += Time.deltaTime;

			if(timer >= knockTime){
				timer = 0;
				fromRight = false;
			}
		}

		if (fromLeft && !fromDown && !fromRight && !fromUp) {

			knockToLocation = new Vector3 (BlueCube.transform.position.x - knockRange,transform.position.y, BlueCube.transform.position.z);

			BlueCube.transform.position = Vector3.Lerp(BlueCube.transform.position, knockToLocation, 0.2f);

			timer += Time.deltaTime;

			if(timer >= knockTime){
				timer = 0;
				fromLeft = false;
			}

		}

		if (fromUp && !fromDown && !fromLeft && !fromRight) {
			
			knockToLocation = new Vector3 (BlueCube.transform.position.x, transform.position.y, BlueCube.transform.position.z + knockRange);

			BlueCube.transform.position = Vector3.Lerp(BlueCube.transform.position, knockToLocation, 0.2f);

			timer += Time.deltaTime;

			if(timer >= knockTime){
				timer = 0;
				fromUp = false;
			}
		}

		if (fromDown && !fromRight && !fromLeft && !fromUp) {

			knockToLocation = new Vector3 (BlueCube.transform.position.x, transform.position.y, BlueCube.transform.position.z - knockRange);

			BlueCube.transform.position = Vector3.Lerp(BlueCube.transform.position, knockToLocation, 0.2f);

			timer += Time.deltaTime;

			if(timer >= knockTime){
				timer = 0;
				fromDown = false;
			}
		}

		//Combined Knockback

		if(fromUp && fromRight){
			knockToLocation = new Vector3 (BlueCube.transform.position.x + knockRange, transform.position.y, BlueCube.transform.position.z + knockRange);
			BlueCube.transform.position = Vector3.Lerp(BlueCube.transform.position, knockToLocation, 0.2f);

			timer += Time.deltaTime;

			if(timer >= knockTime){
				timer = 0;
				fromUp = false;
				fromRight = false;
			}
		}

		if(fromUp && fromLeft){
			knockToLocation = new Vector3 (BlueCube.transform.position.x - knockRange,transform.position.y, BlueCube.transform.position.z + knockRange);
			BlueCube.transform.position = Vector3.Lerp(BlueCube.transform.position, knockToLocation, 0.2f);

			timer += Time.deltaTime;

			if(timer >= knockTime){
				timer = 0;
				fromUp = false;
				fromLeft = false;
			}
		}

		if(fromDown && fromRight){
			knockToLocation = new Vector3 (BlueCube.transform.position.x + knockRange, transform.position.y, BlueCube.transform.position.z - knockRange);
			BlueCube.transform.position = Vector3.Lerp(BlueCube.transform.position, knockToLocation, 0.2f);

			timer += Time.deltaTime;

			if(timer >= knockTime){
				timer = 0;
				fromDown = false;
				fromRight = false;
			}
		}

		if(fromDown && fromLeft){
			knockToLocation = new Vector3 (BlueCube.transform.position.x - knockRange, transform.position.y, BlueCube.transform.position.z + knockRange);
			BlueCube.transform.position = Vector3.Lerp(BlueCube.transform.position, knockToLocation, 0.2f);

			timer += Time.deltaTime;

			if(timer >= knockTime){
				timer = 0;
				fromDown = false;
				fromLeft = false;
			}
		}

	}

	bool fromLeft;
	bool fromRight;
	bool fromUp;
	bool fromDown;

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag ("CubeRed")) {

		//Which direction did the hit come from?


			if (other == redLeft) {
				fromRight = true;
			}

			if(other == redRight){
				fromLeft = true;
			}

			if(other == redUp){
				fromDown = true;
			}

			if(other == redDown){
				fromUp = true;
			}
		}
	}
}