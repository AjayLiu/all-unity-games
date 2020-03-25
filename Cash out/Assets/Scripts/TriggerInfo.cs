using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerInfo : MonoBehaviour
{
    public float intersectionPercentage;
    public float policeToSpawn;
    public float rammersToSpawn;
    public float tankersToSpawn;

    void Start() {
        Debug.DrawLine(transform.position, transform.position + Vector3.up * 30f, Color.cyan, 10);
    }

    void OnTriggerEnter(Collider other){
        if(other.tag == "Player") {
            GameControllerScript.TriggerHit(this);
            Destroy(this.gameObject);
        }
    }
}
