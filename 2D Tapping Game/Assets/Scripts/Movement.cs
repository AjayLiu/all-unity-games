using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() //WHEN FLAG CLICKED
    {
        
    }

    public float speed;

    // Update is called once per frame
    void Update() //RUNS 60~ TIMES PER SECOND
    {
        //IF THE A KEY IS PRESSED
        if (Input.GetKey(KeyCode.A)) {
            //THEN, MOVE TO THE LEFT
            transform.Translate(Vector3.left * speed);
        }
        if (Input.GetKey(KeyCode.D)) {
            transform.Translate(Vector3.right * speed);
        }
        if (Input.GetKey(KeyCode.W)) {
            transform.Translate(Vector3.up * speed);
        }
        if (Input.GetKey(KeyCode.S)) {
            transform.Translate(Vector3.down * speed);
        }
    }
}
