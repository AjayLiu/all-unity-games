using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileDetectionScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float missileDamage = 40;

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Obstacle") {
            other.GetComponent<ObstacleScript>().ShotDown(other.transform.position);
        }
        if(other.gameObject.tag == "Player") {
            other.GetComponent<PlayerScript>().TakeDamage(missileDamage);
        }
    }
}
