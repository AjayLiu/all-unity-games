using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    GameObject player;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        PanCamera();
	}

    public float panSmoothness;

    void PanCamera(){
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, player.transform.position.x, panSmoothness), Mathf.Lerp(transform.position.y, player.transform.position.y, panSmoothness), transform.position.z); 
    }
}
