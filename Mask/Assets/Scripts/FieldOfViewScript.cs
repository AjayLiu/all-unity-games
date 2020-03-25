using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewScript : MonoBehaviour {

    public float widenessAngle = 90f;
    EnemyScript enemy;

	// Use this for initialization
	void Start () {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, 90f - widenessAngle, transform.eulerAngles.z);        
        enemy = transform.parent.GetComponentInParent<EnemyScript>();
    }

    void OnTriggerEnter2D(Collider2D other){
        if (other.CompareTag("Player"))
            enemy.OnPlayerSeen();
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player"))
            enemy.OnEscapeView();
    }
}
