using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class PlayerScript : MonoBehaviour {

    static public int player_hp = 1000;

    const int JOYSTICKS_COUNT = 2;

    public bool isPC = true;

    VirtualJoystick leftJoystick;
    VirtualJoystick rightJoystick;

    Text playerHP_Text;

    Renderer rend;
    LineRenderer line;
   AudioSource audio; 
    // Use this for initialization

    void Start() {
        line = GetComponent<LineRenderer>();
        rend = GetComponent<Renderer>();
        ReferenceJoysticks();   
        audio = GetComponent<AudioSource>(); 

        SetText_Start();
    }

    // Update is called once per frame
    void Update() {
        Move();
        Look();
        //SlowTime();
        ClampBorders();

        SetText_Update();
        SetColor();
        Shoot();
        if (player_hp <= 0)
        {
         
        }
    
    }
    public void onhit() {
    
        audio.Play ();


    }

    void ReferenceJoysticks() {
        GameObject[] joysticks = new GameObject[JOYSTICKS_COUNT];
        joysticks = GameObject.FindGameObjectsWithTag("Joystick");

        if (joysticks[0].name == "RightJoystick") {
            leftJoystick = joysticks[1].GetComponentInChildren<VirtualJoystick>();
            rightJoystick = joysticks[0].GetComponentInChildren<VirtualJoystick>();
        } else {
            leftJoystick = joysticks[0].GetComponentInChildren<VirtualJoystick>();
            rightJoystick = joysticks[1].GetComponentInChildren<VirtualJoystick>();
        }

        if (isPC) {
            leftJoystick.transform.GetChild(0).gameObject.SetActive(false);
            rightJoystick.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    void Move() {
        transform.position += new Vector3(leftJoystick.Horizontal() * leftJoystick.sensitivity, leftJoystick.Vertical() * leftJoystick.sensitivity);
    }

    Vector3 targetPos;

    void Look() {
        if (!isPC) {
            targetPos = new Vector3(rightJoystick.Horizontal() + transform.position.x, rightJoystick.Vertical() + transform.position.y);
            transform.up = targetPos - transform.position;
        } else {
            //Grab the current mouse position on the screen
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z - Camera.main.transform.position.z));

            //Rotates toward the mouse
            transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2((mousePosition.y - transform.position.y), (mousePosition.x - transform.position.x)) * Mathf.Rad2Deg - 90);
        }
    }


    GameObject HitObj;
    float Distance = 0f;

    void Shoot() {

        int playerLayer = 8;

        int playerMask = 1 << playerLayer;

        Vector3 shootPos;
        RaycastHit2D[] hits;

        if (!isPC) {
            shootPos = new Vector2(rightJoystick.Horizontal(), rightJoystick.Vertical());
            hits = Physics2D.RaycastAll((Vector2)shootPos + (Vector2)transform.position, transform.position, GameControllerScript.upBorder * 20);
            Debug.DrawRay((Vector2)shootPos + (Vector2)transform.position, transform.position);
            line.SetPositions(new Vector3[] { (Vector2)shootPos + (Vector2)transform.position, (Vector2)shootPos });
        } else {
            shootPos = Camera.main.ScreenToWorldPoint((Vector2)Input.mousePosition);
            hits = Physics2D.RaycastAll(transform.position, (Vector2)shootPos - (Vector2)transform.position, Vector2.Distance(shootPos, transform.position));

            line.SetPositions(new Vector3[] { (Vector2)transform.position, (Vector2)shootPos });
        }

        foreach(RaycastHit2D hit in hits) {
            if (hit.collider != null && hit.transform.gameObject.tag == "Enemy") {
                EnemyBlockScript enemy = hit.transform.GetComponent<EnemyBlockScript>();
                if (enemy != null)
                    enemy.block_hp--;
                else
                    hit.transform.GetComponent<BombScript>().block_hp--;
            }
        }        

        //HitObj = hit.transform.gameObject;
        // Again optional unless you want to access the hit with more detail

        //print(hit.transform.gameObject.tag);    

        //Distance = Mathf.Abs(hit.point.y - transform.position.y);
        // grabbing the distance in a 2d plane AS A FLOAT

    }

    void SlowTime() {
        if(Mathf.Abs(leftJoystick.Horizontal()) > Mathf.Abs(leftJoystick.Vertical())) {
            Time.timeScale = Mathf.Abs(leftJoystick.Horizontal());
        } else {
            Time.timeScale = Mathf.Abs(leftJoystick.Vertical());
        }
    }

    void ClampBorders() {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(this.transform.position);
        viewPos.x = Mathf.Clamp01(viewPos.x);
        viewPos.y = Mathf.Clamp01(viewPos.y);
        this.transform.position = Camera.main.ViewportToWorldPoint(viewPos);
    }


    void SetText_Start() {
        playerHP_Text = GameObject.Find("Player HP Display").GetComponent<Text>();
        playerHP_Text.fontStyle = FontStyle.Bold;
    }

    void SetText_Update() {
        playerHP_Text.text = player_hp.ToString();
    }

    void SetColor() {
        //rend.material.color = GameControllerScript.colors[player_hp / GameControllerScript.hpToColorRatio];
    }

}

