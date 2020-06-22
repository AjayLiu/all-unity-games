using UnityEngine;
using System.Collections;

public class KnockDetectRed : MonoBehaviour {

	[SerializeField]Collider bluLeft, bluRight, bluUp, bluDown;
	[SerializeField]GameObject RedCube;


	// Use this for initialization
	void Start () {
		timer = 0;
	}

	public float knockRange;
	public float knockTime;
	Vector3 knockToLocation;
	float timer;

	void Update () {

		if (fromRight) {

			knockToLocation = new Vector3 (RedCube.transform.position.x + knockRange, transform.position.y , RedCube.transform.position.z);

			RedCube.transform.position = Vector3.Lerp(RedCube.transform.position, knockToLocation, 0.2f);

			timer += Time.deltaTime;

			if(timer >= knockTime){
				timer = 0;
				fromRight = false;
			}


		}

		if (fromLeft) {

			knockToLocation = new Vector3 (RedCube.transform.position.x - knockRange, transform.position.y, RedCube.transform.position.z);

			RedCube.transform.position = Vector3.Lerp(RedCube.transform.position, knockToLocation, 0.2f);

			timer += Time.deltaTime;

			if(timer >= knockTime){
				timer = 0;
				fromLeft = false;
			}
		}

		if (fromUp) {

			knockToLocation = new Vector3 (RedCube.transform.position.x, transform.position.y, RedCube.transform.position.z + knockRange);

			RedCube.transform.position = Vector3.Lerp(RedCube.transform.position, knockToLocation, 0.2f);

			timer += Time.deltaTime;

			if(timer >= knockTime){
				timer = 0;
				fromUp = false;
			}
		}

		if (fromDown) {

			knockToLocation = new Vector3 (RedCube.transform.position.x,transform.position.y, RedCube.transform.position.z - knockRange);

			RedCube.transform.position = Vector3.Lerp(RedCube.transform.position, knockToLocation, 0.2f);

			timer += Time.deltaTime;

			if(timer >= knockTime){
				timer = 0;
				fromDown = false;
			}
		}

		//Combined Knockback

		if(fromUp && fromRight){
			knockToLocation = new Vector3 (RedCube.transform.position.x + knockRange,transform.position.y, RedCube.transform.position.z + knockRange);
			RedCube.transform.position = Vector3.Lerp(RedCube.transform.position, knockToLocation, 0.2f);

			timer += Time.deltaTime;

			if(timer >= knockTime){
				timer = 0;
				fromUp = false;
				fromRight = false;
			}
		}

		if(fromUp && fromLeft){
			knockToLocation = new Vector3 (RedCube.transform.position.x - knockRange, transform.position.y, RedCube.transform.position.z + knockRange);
			RedCube.transform.position = Vector3.Lerp(RedCube.transform.position, knockToLocation, 0.2f);

			timer += Time.deltaTime;

			if(timer >= knockTime){
				timer = 0;
				fromUp = false;
				fromLeft = false;
			}
		}

		if(fromDown && fromRight){
			knockToLocation = new Vector3 (RedCube.transform.position.x + knockRange, transform.position.y, RedCube.transform.position.z - knockRange);
			RedCube.transform.position = Vector3.Lerp(RedCube.transform.position, knockToLocation, 0.2f);

			timer += Time.deltaTime;

			if(timer >= knockTime){
				timer = 0;
				fromDown = false;
				fromRight = false;
			}
		}

		if(fromDown && fromLeft){
			knockToLocation = new Vector3 (RedCube.transform.position.x - knockRange, transform.position.y, RedCube.transform.position.z + knockRange);
			RedCube.transform.position = Vector3.Lerp(RedCube.transform.position, knockToLocation, 0.2f);

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

	void OnTriggerEnter (Collider other) {
		if (other.CompareTag ("CubeBlue")) {

			//Which direction did the hit come from?


			if (other == bluLeft) {
				fromRight = true;
			}

			if (other == bluRight) {
				fromLeft = true;
			}

			if (other == bluUp) {
				fromDown = true;
			}

			if (other == bluDown) {
				fromUp = true;
			}
		}
	}
}