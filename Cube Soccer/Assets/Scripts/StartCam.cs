using UnityEngine;
using System.Collections;

public class StartCam : MonoBehaviour {

	[SerializeField]Transform camFocus;
	public float rotAngle = 20f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.RotateAround (camFocus.position, Vector3.up, rotAngle);
	}
}
