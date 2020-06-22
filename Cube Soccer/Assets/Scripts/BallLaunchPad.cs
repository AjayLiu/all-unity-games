using UnityEngine;
using System.Collections;

public class BallLaunchPad : MonoBehaviour {

	[SerializeField] Rigidbody ballRbody;
	public float speed;

	void OnCollisionEnter(Collision other){
		if(other.gameObject.CompareTag("SoccerBall")){
			if(this.gameObject.name == "TopLeft"){
				ballRbody.velocity = (Vector3.forward + Vector3.left) * speed * Time.deltaTime;
			}
			if(this.gameObject.name == "TopRight"){
				ballRbody.velocity = (Vector3.forward + Vector3.right) * speed * Time.deltaTime;
			}
			if(this.gameObject.name == "BottomLeft"){
				ballRbody.velocity = (Vector3.back + Vector3.left) * speed * Time.deltaTime;
			}
			if(this.gameObject.name == "BottomRight"){
				ballRbody.velocity = (Vector3.back + Vector3.right) * speed * Time.deltaTime;
			}
			if(this.gameObject.name == "RedGoalkeeper"){
				ballRbody.velocity = Vector3.left * speed * Time.deltaTime;
			}
			if(this.gameObject.name == "BlueGoalkeeper"){
				ballRbody.velocity = Vector3.right * speed * Time.deltaTime;
			}
            if (this.gameObject.name == "TopEdge") {
                ballRbody.velocity = Vector3.forward * speed * Time.deltaTime;
            }
            if (this.gameObject.name == "BottomEdge") {
                ballRbody.velocity = Vector3.back * speed * Time.deltaTime;
            }

        }
	}
}
