using UnityEngine;
using System.Collections;

public class PlayerBlueScript : MonoBehaviour {

	public float speed;
	Rigidbody rbody;

	// Use this for initialization
	void Start () {
		rbody = GetComponent<Rigidbody> ();
	}

	// Update is called once per frame
	public bool stopDiagMove = false;

	public StartOfGame startScript;

	void Update () {
		

		if(startScript.gameStart){
			//Multiple Movement
			if (Input.GetKey (KeyCode.UpArrow) && Input.GetKey (KeyCode.LeftArrow)) {
				rbody.velocity = (Vector3.back + Vector3.right) * speed * Time.deltaTime / 1.5f;
			} 

			if (Input.GetKey (KeyCode.UpArrow) && Input.GetKey (KeyCode.RightArrow)) {
				rbody.velocity = (Vector3.back + Vector3.left) * speed * Time.deltaTime / 1.5f;
			} 
			if(!stopDiagMove){
				if (Input.GetKey (KeyCode.DownArrow) && Input.GetKey (KeyCode.LeftArrow)) {
					rbody.velocity = (Vector3.forward + Vector3.right) * speed * Time.deltaTime / 1.5f; 
				} 

				if (Input.GetKey (KeyCode.DownArrow) && Input.GetKey (KeyCode.RightArrow)) {
					rbody.velocity = (Vector3.forward + Vector3.left) * speed * Time.deltaTime / 1.5f;
				}
			}
			// Single Movement
			if (Input.GetKey (KeyCode.UpArrow) && Input.GetKey (KeyCode.DownArrow) == false && Input.GetKey (KeyCode.LeftArrow) == false && Input.GetKey (KeyCode.RightArrow) == false) {
				rbody.velocity = (Vector3.back) * speed * Time.deltaTime;
			}

			if (Input.GetKey (KeyCode.DownArrow) && Input.GetKey (KeyCode.UpArrow) == false && Input.GetKey (KeyCode.LeftArrow) == false && Input.GetKey (KeyCode.RightArrow) == false) {
				rbody.velocity = (Vector3.forward) * speed * Time.deltaTime;
			} 

			if (Input.GetKey (KeyCode.LeftArrow) && Input.GetKey (KeyCode.DownArrow) == false && Input.GetKey (KeyCode.UpArrow) == false && Input.GetKey (KeyCode.RightArrow) == false) {
				rbody.velocity = (Vector3.right) * speed * Time.deltaTime;
			}

			if (Input.GetKey (KeyCode.RightArrow) && Input.GetKey (KeyCode.DownArrow) == false && Input.GetKey (KeyCode.LeftArrow) == false && Input.GetKey (KeyCode.UpArrow) == false) {
				rbody.velocity = (Vector3.left) * speed * Time.deltaTime;
			}


			if (Input.GetKey (KeyCode.UpArrow) == false && Input.GetKey (KeyCode.LeftArrow) == false && Input.GetKey (KeyCode.RightArrow) == false && Input.GetKey (KeyCode.DownArrow) == false) {
				rbody.velocity = Vector3.zero;
			}
		}
	}
}