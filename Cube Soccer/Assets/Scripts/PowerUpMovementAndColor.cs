using UnityEngine;
using System.Collections;

public class PowerUpMovementAndColor : MonoBehaviour {

	public float speed = 1;
	private Renderer rend;

	float bobPeriod = 1f;
	float bobAmplitude = 1f;
	float startY;
	float bobDistance;

	void Start(){
		rend = GetComponent<Renderer>();

		startY = transform.localPosition.y;
	}

	void Update(){
		rend.material.SetColor("_Color", HSBColor.ToColor(new HSBColor( Mathf.PingPong(Time.time * speed, 1), 1, 1)));

		bobDistance = Mathf.Sin(Time.timeSinceLevelLoad / bobPeriod) * bobAmplitude;

		transform.position = new Vector3(transform.localPosition.x, bobDistance + startY, transform.localPosition.z);
	}
}
