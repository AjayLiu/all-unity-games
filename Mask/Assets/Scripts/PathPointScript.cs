using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPointScript : MonoBehaviour {
	void OnTriggerEnter2D(Collider2D other){
        if (other.gameObject.name == "EnemyPosition"){
            EnemyScript enemy = other.GetComponent<EnemyScript>();
            EnemyBehavior b = enemy.GetComponentInParent<EnemyBehavior>();
            if (GetComponentInParent<PathScript>().id == b.pathId && b.useCustomPath){
                enemy.StartCoroutine("ChangeDirection");
            }
        }
    }
}
