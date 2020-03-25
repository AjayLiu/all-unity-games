using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour {
    GameScript game;
    [HideInInspector]public Rigidbody rb;
    Collider c;
    public static bool isMovingUp = false;
    public float jumpAmount;

    bool isStart;

    public static Color playerColor;
    public int colorIndex = 0;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        game = GameObject.Find("GAME").GetComponent<GameScript>();
        c = GetComponent<Collider>();

        playerColor = game.colors[0];
        GetComponentInChildren<SpriteRenderer>().color = new Color(playerColor.r, playerColor.g, playerColor.b, 1f);

        customSensitivity = sensitivitySlider.GetComponent<Slider>();
        customSensitivity.value = customSensitivity.maxValue / 2;
    }

    public float sensitivity;
    public Slider customSensitivity;

	// Update is called once per frame
	void Update () {
        TiltMove(sensitivity);
        TapDetection();
        
        
        
        PanCamera();
    }

    public bool allowTouch = true;

    public void ResetColorFromStar() {
        playerColor = game.colors[colorIndex];
        GetComponentInChildren<SpriteRenderer>().color = new Color(playerColor.r, playerColor.g, playerColor.b, 1f);
    }

    void TapDetection() {
		if(Input.touchCount >= 1 && allowTouch) {
			Touch t = Input.GetTouch(0);
			if(t.phase == TouchPhase.Began) {

				if(!EventSystem.current.IsPointerOverGameObject(t.fingerId)) {
					if(colorIndex == game.colors.Length - 1)
						colorIndex = 0;
					else
						colorIndex++;

					playerColor = game.colors[colorIndex];
					GetComponentInChildren<SpriteRenderer>().color = new Color(playerColor.r,playerColor.g,playerColor.b,1f);

				}
			}
		} 

		//For Keyboard
		if(Input.GetKeyDown(KeyCode.Space)) {
			if(colorIndex == game.colors.Length - 1)
				colorIndex = 0;
			else
				colorIndex++;
			
			playerColor = game.colors[colorIndex];
			GetComponentInChildren<SpriteRenderer>().color = new Color(playerColor.r,playerColor.g,playerColor.b,1f);
		}

    }

    public bool allowTilt = false;

    public GameObject sensitivitySlider;

    public void ChangeTiltSensitivity() {
        sensitivity = 32 * (customSensitivity.value);
    }



    bool isKeyboard;
    public Canvas inputSelectCanvas;
    public void SetMobileInput() {
        isKeyboard = false;
        inputSelectCanvas.gameObject.SetActive(false);
    }
    public void SetKeyboardInput() {
        isKeyboard = true;
        inputSelectCanvas.gameObject.SetActive(false);
    }

    void TiltMove(float tiltSensitivity) {
        if (allowTilt) {
            if (isKeyboard) {
                rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * tiltSensitivity * 0.2f, rb.velocity.y);
            } else {
                rb.velocity = new Vector2(-Input.acceleration.x * tiltSensitivity * Time.deltaTime * 3, rb.velocity.y);
            }
        }

        


        if(Camera.main.WorldToViewportPoint(transform.position).x > 1) {
            transform.position = new Vector3(Camera.main.ViewportToWorldPoint(Vector3.zero).x, transform.position.y);
        }
        if (Camera.main.WorldToViewportPoint(transform.position).x < 0) {
            transform.position = new Vector3(Camera.main.ViewportToWorldPoint(Vector3.one).x, transform.position.y);
        }

        //TURN OFF PLATFORM COLLIDERS WHEN MOVING UP
        if (rb.velocity.y > 0) {
            game.DisablePlatformColliders();
        }
        if (rb.velocity.y < 0){
            game.EnablePlatformColliders();
        }
    }

    void Jump(float power) {
        rb.AddForce(Vector2.up * power);
    }

    public float screenPanThresholdNormalized;
    public float panAmount;
    public float panSpeed;
    void PanCamera() {
        if (Camera.main.WorldToViewportPoint(transform.position).y > screenPanThresholdNormalized) {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, 
                new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + (Camera.main.WorldToViewportPoint(transform.position).y - screenPanThresholdNormalized) * panAmount, Camera.main.transform.position.z), panSpeed);
        }
    }

    [SerializeField]
    float[] glitchThresholds;
    public float glitchAdjustmentAmount;

    void OnCollisionEnter(Collision col) {
        if (col.gameObject.CompareTag("Platform")) {

            PlatformProperties pro;
            pro = col.gameObject.GetComponent<PlatformProperties>();

            rb.velocity = new Vector2(0, rb.velocity.y);
            StartCoroutine("FallThrough", pro);
            Jump(jumpAmount);
            
            if(pro.isCracked) {
                pro.BreakPlatform();
            }            
        }
    }

    IEnumerator FallThrough(PlatformProperties p) {        
        c.enabled = false;
        p.DisableCollider();
        yield return new WaitForSeconds(0.2f);
        c.enabled = true;
        p.EnableCollider();
    }
}
    