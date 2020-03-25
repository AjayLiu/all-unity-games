using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    GameCon game;

    SpinnerScript spin;
    public Rigidbody2D rd;
    HingeJoint2D hinge;

	// Use this for initialization
	void Start () {

        game = GameObject.Find("GAMECON").GetComponent<GameCon>();

        spin = GameObject.FindGameObjectWithTag("Spinner").GetComponent<SpinnerScript>();
        rd = GetComponent<Rigidbody2D>();

        hinge = GetComponent<HingeJoint2D>();       

        defaultDrag = rd.angularDrag;

        currentSpinDetector = Instantiate(spinDetectorPrefab, transform.position, Quaternion.identity);

        prevSpinDrag = defaultDrag;

    }
	
	// Update is called once per frame
	void Update () {
        GetInput();
        CheckClockwise();
        SlowPlayer();
        SlowSpinning();

        LookAtMoveDirection();

        currentSpinSpeed = defaultDrag - rd.angularDrag;
        currentFlySpeed = DRAG_MAX - rd.drag;

    }


   

    void GetInput() {
        if(!game.lockInput) { 
            if (game.status == Status.Spinning && (Input.GetKey(KeyCode.Space) || Input.touchCount > 0)) {
                SpeedUp();            
             }

            if(!game.isKeyboard) {
                if (game.status == Status.Spinning) {
                    bool blast;
                    if (Input.touchCount == 0)
                        blast = false;
                    else
                        blast = true;

                    foreach (Touch touch in Input.touches) {
                        if(touch.phase != TouchPhase.Ended) {
                            blast = false;
                            break;
                        }
                    }
                    if(blast)
                        BlastOff();
                }                
            } else {
                if (Input.GetKeyUp(KeyCode.Space)) {
                    BlastOff();
                }
            }
        }
    }

    [HideInInspector]
    public bool isMaxSpeed = false;

    public float speed;
    public float speed2;

    bool isClockwise = false;

    public float currentSpinSpeed;
    public float currentFlySpeed;

    void SpeedUp() {

        slowSpin = false;

        if (isClockwise)
            rd.AddTorque(-1f, ForceMode2D.Force);
        else
            rd.AddTorque(1f, ForceMode2D.Force);

        if(rd.angularDrag > defaultDrag * 0.5f) {
            rd.angularDrag -= speed * Time.deltaTime;
            print("SPEED 1");      
        } else {
            rd.angularDrag -= speed2 * Time.deltaTime;
            print("SPEED 2");
        }

        if(rd.angularDrag == 0) {
            print("MAX SPEED");
            isMaxSpeed = true;
        } else {
            isMaxSpeed = false;
        }
    }

    bool slowDown = false;
    bool slowSpin = false;

    float prevSpinDrag;

    [SerializeField] float dragAmount;
    [SerializeField]
    float angularDragAmount;

    float DRAG_MAX = 1.5f;

    void SlowPlayer() {
        if(slowDown) {            
            rd.drag += dragAmount * Time.deltaTime;
        }
    }

    void SlowSpinning() {
        if (slowSpin && rd.angularDrag < defaultDrag) {
            rd.angularDrag += angularDragAmount * Time.deltaTime;
        }
    }

    [SerializeField]Transform directionDummy;
    bool trackMoveDirection = false;

    void BlastOff() {
        game.status = Status.Flying;

        game.everything.Remove(hinge.connectedBody.gameObject);
        Destroy(hinge.connectedBody.gameObject);
        Destroy(hinge);

        Destroy(currentSpinDetector);

        slowDown = true;
        rd.angularDrag = prevSpinDrag;

        game.isGameSetup = false;

        trackMoveDirection = true;

        game.lockSteer = false;
        game.leftSteerButton.gameObject.SetActive(true);
        game.rightSteerButton.gameObject.SetActive(true);
        game.upSteerButton.gameObject.SetActive(true);

        game.lockInput = true;
    }

    float defaultDrag;

    bool checkClockwise = false;

    Vector3 pointA, pointB; // for comparing clockwise or not

    public GameObject currentSpinner;

    [SerializeField]
    GameObject spinDetectorPrefab;

    GameObject currentSpinDetector;


    public void SpinnerHit (GameObject spinner, Collision2D col) {
        rd.drag = 0;
        slowDown = false;
        slowSpin = true;
        prevSpinDrag = rd.angularDrag;
        trackMoveDirection = false;
        game.lockSteer = true;
        game.leftSteerButton.gameObject.SetActive(false);
        game.rightSteerButton.gameObject.SetActive(false);
        game.upSteerButton.gameObject.SetActive(false);

        game.lockInput = false;

        game.status = Status.Spinning;

        spin = spinner.GetComponent<SpinnerScript>();
        hinge = gameObject.AddComponent<HingeJoint2D>();
        hinge.connectedBody = spinner.GetComponent<Rigidbody2D>();
        currentSpinner = spinner;
        hinge.anchor = transform.InverseTransformPoint(spinner.transform.position);

        //Determine Clockwise or not
        pointA = col.contacts[0].point;
        checkClockwise = true;

        //Default Angular Drag
        rd.angularDrag = defaultDrag;

        //Create Spin Detector
        currentSpinDetector = Instantiate(spinDetectorPrefab, col.contacts[0].point, Quaternion.identity);       
        //Add to the Spinners Hit var
        if(!game.isGameSetup) {
            game.spinnersHit++;
        }
    }

    
    int frameCount = 0;
    void CheckClockwise() {
        if(checkClockwise) {
            if(frameCount > 6) {

                //pointB = transform.InverseTransformPoint(hinge.connectedBody.gameObject.transform.position);
                pointB = transform.position;

                //Instantiate(game.REDMARKER, pointA, Quaternion.identity);
                //Instantiate(game.BLUEMARKER, pointB, Quaternion.identity);

                //RIGHT SIDE OF SPINNER
                if (pointA.x > hinge.connectedBody.transform.position.x) {
                    //going up
                    if(pointB.y > pointA.y) {
                        isClockwise = false;                        
                    } else {
                        isClockwise = true;                        
                    }
                } else {
                    //LEFT SIDE OF SPINNER
                    if (pointB.y > pointA.y) {
                        isClockwise = true;
                    } else {
                        isClockwise = false;
                    }
                }

                frameCount = 0;
                checkClockwise = false;
            }
            frameCount++;
        }
    }

    void LookAtMoveDirection() {
        if(trackMoveDirection) {
            transform.up = rd.velocity.normalized;
        }
    }
}
