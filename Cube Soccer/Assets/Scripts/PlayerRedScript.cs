using UnityEngine;
using System.Collections;

public class PlayerRedScript : MonoBehaviour {

	public float speed;
	Rigidbody rbody;

	// Use this for initialization
	void Start () {
		rbody = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	public StartOfGame startScript;

	void Update (){

		if(startScript.gameStart){
			//Combine Movement
			if (Input.GetKey (KeyCode.W) && Input.GetKey (KeyCode.A)) {
				rbody.velocity = (Vector3.back + Vector3.right) * speed * Time.deltaTime / 1.5f;
			}

			if (Input.GetKey (KeyCode.W) && Input.GetKey (KeyCode.D)) {
				rbody.velocity = (Vector3.back + Vector3.left) * speed * Time.deltaTime / 1.5f;
			} 

			if (Input.GetKey (KeyCode.S) && Input.GetKey (KeyCode.A)) {
				rbody.velocity = (Vector3.forward + Vector3.right) * speed * Time.deltaTime / 1.5f; 
			} 

			if (Input.GetKey (KeyCode.S) && Input.GetKey (KeyCode.D)) {
				rbody.velocity = (Vector3.forward + Vector3.left) * speed * Time.deltaTime / 1.5f;
			}
				
			// Single Movement	
			if (Input.GetKey (KeyCode.W) && Input.GetKey(KeyCode.S) == false && Input.GetKey(KeyCode.A) == false && Input.GetKey(KeyCode.D) == false) {
				rbody.velocity = (Vector3.back) * speed * Time.deltaTime;
			}

			if (Input.GetKey (KeyCode.S) && Input.GetKey(KeyCode.W) == false && Input.GetKey(KeyCode.A) == false && Input.GetKey(KeyCode.D) == false) {
				rbody.velocity = (Vector3.forward) * speed * Time.deltaTime;
			} 

			if (Input.GetKey (KeyCode.A) && Input.GetKey(KeyCode.S) == false && Input.GetKey(KeyCode.W) == false && Input.GetKey(KeyCode.D) == false) {
				rbody.velocity = (Vector3.right) * speed * Time.deltaTime;
			}

			if (Input.GetKey (KeyCode.D) && Input.GetKey(KeyCode.S) == false && Input.GetKey(KeyCode.A) == false && Input.GetKey(KeyCode.W) == false) {
				rbody.velocity = (Vector3.left) * speed * Time.deltaTime;
			}

			if (Input.GetKey (KeyCode.W) == false && Input.GetKey (KeyCode.A) == false && Input.GetKey (KeyCode.D) == false && Input.GetKey (KeyCode.S) == false) {
				rbody.velocity = Vector3.zero;
			}
		}
	}
}
