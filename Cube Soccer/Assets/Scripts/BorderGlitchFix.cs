using UnityEngine;
using System.Collections;

public class BorderGlitchFix : MonoBehaviour {

	public Rigidbody redRbody;
	public Rigidbody blueRbody;

	enum RedKeyPress {isPressingNone, isPressingW, isPressingA, isPressingS, isPressingD, isPressingWA, isPressingWD, isPressingSA, isPressingSD};
	enum BlueKeyPress {isPressingNone, isPressingUpArrow,isPressingLeftArrow, isPressingDownArrow, isPressingRightArrow, isPressingUpLeft, isPressingUpRight, isPressingDownLeft, isPressingDownRight};

	RedKeyPress redKeyPress;
	BlueKeyPress blueKeyPress;

	void Update(){


		//Red Movement
		if (Input.GetKey (KeyCode.W) && Input.GetKey (KeyCode.A)) {
			redKeyPress = RedKeyPress.isPressingWA;
		}

		if (Input.GetKey (KeyCode.W) && Input.GetKey (KeyCode.D)) {
			redKeyPress = RedKeyPress.isPressingWD;
		} 

		if (Input.GetKey (KeyCode.S) && Input.GetKey (KeyCode.A)) {
			redKeyPress = RedKeyPress.isPressingSA;
		} 

		if (Input.GetKey (KeyCode.S) && Input.GetKey (KeyCode.D)) {
			redKeyPress = RedKeyPress.isPressingSD;
		}

		// Single Movement	
		if (Input.GetKey (KeyCode.W) && Input.GetKey(KeyCode.S) == false && Input.GetKey(KeyCode.A) == false && Input.GetKey(KeyCode.D) == false) {
			redKeyPress = RedKeyPress.isPressingW;
		}

		if (Input.GetKey (KeyCode.S) && Input.GetKey(KeyCode.W) == false && Input.GetKey(KeyCode.A) == false && Input.GetKey(KeyCode.D) == false) {
			redKeyPress = RedKeyPress.isPressingS;
		} 

		if (Input.GetKey (KeyCode.A) && Input.GetKey(KeyCode.S) == false && Input.GetKey(KeyCode.W) == false && Input.GetKey(KeyCode.D) == false) {
			redKeyPress = RedKeyPress.isPressingA;
		}

		if (Input.GetKey (KeyCode.D) && Input.GetKey(KeyCode.S) == false && Input.GetKey(KeyCode.A) == false && Input.GetKey(KeyCode.W) == false) {
			redKeyPress = RedKeyPress.isPressingD;
		}



		//Blue Movement
		if (Input.GetKey (KeyCode.UpArrow) && Input.GetKey (KeyCode.LeftArrow)) {
			blueKeyPress = BlueKeyPress.isPressingUpLeft;
		} 

		if (Input.GetKey (KeyCode.UpArrow) && Input.GetKey (KeyCode.RightArrow)) {
			blueKeyPress = BlueKeyPress.isPressingUpRight;
		} 

		if (Input.GetKey (KeyCode.DownArrow) && Input.GetKey (KeyCode.LeftArrow)) {
			blueKeyPress = BlueKeyPress.isPressingDownLeft;
		} 

		if (Input.GetKey (KeyCode.DownArrow) && Input.GetKey (KeyCode.RightArrow)) {
			blueKeyPress = BlueKeyPress.isPressingDownRight;
		}

		// Single Movement
		if (Input.GetKey (KeyCode.UpArrow) && Input.GetKey (KeyCode.DownArrow) == false && Input.GetKey (KeyCode.LeftArrow) == false && Input.GetKey (KeyCode.RightArrow) == false) {
			blueKeyPress = BlueKeyPress.isPressingUpArrow;
		}

		if (Input.GetKey (KeyCode.DownArrow) && Input.GetKey (KeyCode.UpArrow) == false && Input.GetKey (KeyCode.LeftArrow) == false && Input.GetKey (KeyCode.RightArrow) == false) {
			blueKeyPress = BlueKeyPress.isPressingDownArrow;
		} 

		if (Input.GetKey (KeyCode.LeftArrow) && Input.GetKey (KeyCode.DownArrow) == false && Input.GetKey (KeyCode.UpArrow) == false && Input.GetKey (KeyCode.RightArrow) == false) {
			blueKeyPress = BlueKeyPress.isPressingLeftArrow;
		}

		if (Input.GetKey (KeyCode.RightArrow) && Input.GetKey (KeyCode.DownArrow) == false && Input.GetKey (KeyCode.LeftArrow) == false && Input.GetKey (KeyCode.UpArrow) == false) {
			blueKeyPress = BlueKeyPress.isPressingRightArrow;
		}




		if((redIsOnTopBorder && (redKeyPress == RedKeyPress.isPressingW || redKeyPress == RedKeyPress.isPressingWA || redKeyPress == RedKeyPress.isPressingWD))){
			redIsStuckTop = true;
		}

		if((blueIsOnTopBorder && (blueKeyPress == BlueKeyPress.isPressingUpArrow || blueKeyPress == BlueKeyPress.isPressingUpLeft || blueKeyPress == BlueKeyPress.isPressingUpRight))){
			blueIsStuckTop = true;
		}

		if((redIsOnBottomBorder && (redKeyPress == RedKeyPress.isPressingS || redKeyPress == RedKeyPress.isPressingSA || redKeyPress == RedKeyPress.isPressingSD))){
			redIsStuckBottom = true;
		}

		if((blueIsOnBottomBorder && (blueKeyPress == BlueKeyPress.isPressingDownArrow || blueKeyPress == BlueKeyPress.isPressingDownLeft || blueKeyPress == BlueKeyPress.isPressingDownRight))){
			blueIsStuckBottom = true;	
		}




		if((redIsOnTopBorder && (redKeyPress == RedKeyPress.isPressingS || redKeyPress == RedKeyPress.isPressingSA || redKeyPress == RedKeyPress.isPressingSD)   ||   (redIsOnBottomBorder && (redKeyPress == RedKeyPress.isPressingW || redKeyPress == RedKeyPress.isPressingWA || redKeyPress == RedKeyPress.isPressingWD)))){
			redIsStuckTop = false;
			redIsStuckBottom = false;
			redIsOnTopBorder = false;
			redIsOnBottomBorder = false;
		}

		if((blueIsOnTopBorder && (blueKeyPress == BlueKeyPress.isPressingDownArrow || blueKeyPress == BlueKeyPress.isPressingDownLeft || blueKeyPress == BlueKeyPress.isPressingDownRight)   ||   (blueIsOnBottomBorder && (blueKeyPress == BlueKeyPress.isPressingUpArrow || blueKeyPress == BlueKeyPress.isPressingUpLeft || blueKeyPress == BlueKeyPress.isPressingUpRight)))){
			blueIsStuckTop = false;
			blueIsStuckBottom = false;
			blueIsOnTopBorder = false;
			blueIsOnBottomBorder = false;
			blueMoveScript.stopDiagMove = false;
		}




		if(redIsStuckTop && (redKeyPress == RedKeyPress.isPressingA || redKeyPress == RedKeyPress.isPressingWA)){
			redRbody.velocity = (Vector3.right) * 2800f * Time.deltaTime;
		}
		if(redIsStuckTop && (redKeyPress == RedKeyPress.isPressingD || redKeyPress == RedKeyPress.isPressingWD)){
			redRbody.velocity = (Vector3.left) * 2800f * Time.deltaTime;
		}
		if(redIsStuckBottom && (redKeyPress == RedKeyPress.isPressingD || redKeyPress == RedKeyPress.isPressingSD)){
			redRbody.velocity = (Vector3.left) * 2800f * Time.deltaTime;
		}
		if(redIsStuckBottom && (redKeyPress == RedKeyPress.isPressingA || redKeyPress == RedKeyPress.isPressingSA)){
			redRbody.velocity = (Vector3.right) * 2800f * Time.deltaTime;
		}


		if(blueIsStuckTop && (blueKeyPress == BlueKeyPress.isPressingLeftArrow || blueKeyPress == BlueKeyPress.isPressingUpLeft)){
			blueRbody.velocity = (Vector3.right) * 2800f * Time.deltaTime;
		}
		if(blueIsStuckTop && (blueKeyPress == BlueKeyPress.isPressingRightArrow || blueKeyPress == BlueKeyPress.isPressingUpRight)){
			blueRbody.velocity = (Vector3.left) * 2800f * Time.deltaTime;
		}
		if(blueIsStuckBottom && (blueKeyPress == BlueKeyPress.isPressingRightArrow || blueKeyPress == BlueKeyPress.isPressingDownRight)){
			blueRbody.velocity = (Vector3.left) * 2800f * Time.deltaTime;
			blueMoveScript.stopDiagMove = true;
		}
		if(blueIsStuckBottom && (blueKeyPress == BlueKeyPress.isPressingLeftArrow || blueKeyPress == BlueKeyPress.isPressingDownLeft)){
			blueRbody.velocity = (Vector3.right) * 2800f * Time.deltaTime;
			blueMoveScript.stopDiagMove = true;
		}

		if(Input.GetKey(KeyCode.W) == false && Input.GetKey(KeyCode.A) == false && Input.GetKey(KeyCode.S) == false && Input.GetKey(KeyCode.D) == false){
			redKeyPress = RedKeyPress.isPressingNone;
		}

		if(Input.GetKey(KeyCode.UpArrow) == false && Input.GetKey(KeyCode.LeftArrow) == false && Input.GetKey(KeyCode.DownArrow) == false && Input.GetKey(KeyCode.RightArrow) == false){
			blueKeyPress = BlueKeyPress.isPressingNone;
		}
	}
	public PlayerBlueScript blueMoveScript;

	bool redIsStuckTop = false;
	bool redIsStuckBottom = false;

	bool blueIsStuckTop = false;
	bool blueIsStuckBottom = false;

	bool redIsOnTopBorder = false;
	bool blueIsOnTopBorder = false;

	bool redIsOnBottomBorder = false;
	bool blueIsOnBottomBorder = false;

	void OnCollisionEnter(Collision other){
		if (this.gameObject.name == "InvisBorderTop") {	
			if (other.gameObject.CompareTag ("CubeRed")) {
				redIsOnTopBorder = true;
			}

			if (other.gameObject.CompareTag ("CubeBlue")) {
				blueIsOnTopBorder = true;
			}
	
		}

		if(this.gameObject.name == "InvisBorderBottom"){
			if (other.gameObject.CompareTag ("CubeRed")) {
				redIsOnBottomBorder = true;
			}

			if (other.gameObject.CompareTag ("CubeBlue")) {
				blueIsOnBottomBorder = true;
			}
		}
	}
}
