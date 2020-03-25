using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectorScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter2D(Collision2D col) {
        col.otherCollider.GetComponent<BarScript>().isInZone = true;
    }
    void OnCollisionExit2D(Collision2D col) {
        col.otherCollider.GetComponent<BarScript>().isInZone = false;
    }
}
