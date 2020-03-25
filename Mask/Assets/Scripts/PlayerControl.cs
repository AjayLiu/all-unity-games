using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

    Rigidbody2D rb;
    public float speed;
    public static MaskScript mask;
    
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        mask = GetComponent<MaskScript>();       
    }

    // Update is called once per frame
    void Update () {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if(Time.timeScale != 0){
            Look();
            OnLeftMouseClick();
        }


    }

    void FixedUpdate(){
        Move();

    }
    Vector3 mousePos;

    [Tooltip("Radius of how far player can stun enemy")]
    public float attackRange;

    void OnLeftMouseClick(){
        if(Input.GetKeyDown(KeyCode.Mouse0)){
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector3.forward);
            if(hit.collider != null){
                //if player clicks on enemy
                if (hit.collider.gameObject.CompareTag("Enemy")){
                    //if enemy is in range                    
                    if (transform.position.x - hit.collider.gameObject.transform.position.x < attackRange && transform.position.y - hit.collider.gameObject.transform.position.y < attackRange){
                        StealMask(hit.collider.gameObject);
                    }
                }
            }
        }
    }


    public KeyCode crouchKey, runKey;
    public float crouchMultiplier, runMultiplier;

    bool isCrouch, isRun;

    void Move(){
        if (Input.GetKeyDown(crouchKey))
            isCrouch = !isCrouch;
        
        if (Input.GetKeyDown(runKey))
            isRun = !isRun;

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        
        if (isCrouch) {
            rb.velocity = input * speed * crouchMultiplier;
        } else if (isRun){
            rb.velocity = input * speed * runMultiplier;
        } else {
            rb.velocity = input * speed;
        }
        
    }


    void Look(){        
        Vector3 diff = mousePos - transform.position;
        diff.Normalize();
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        rb.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);        
    }


    void StealMask(GameObject enemy){
        print("SWAP");
        MaskScript enemyMask = enemy.GetComponentInParent<MaskScript>();
        enemyMask.GetComponentInChildren<EnemyScript>().StartCoroutine("Stun");
        mask.maskID = enemyMask.maskID;
        //enemyMask.maskID = mask.maskID;

        //Color temp = mask.color;

        mask.color = enemyMask.color;
        //enemyMask.color = temp;

        mask.SetColor();
        //enemyMask.SetColor();
    }
    
}
