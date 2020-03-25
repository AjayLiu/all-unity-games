using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakOnImpact : MonoBehaviour
{

    public bool isTrigger = true;


    Rigidbody rbody;
    Collider[] cols;

    Vector3 startPos;
    Quaternion startRot;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.localPosition;
        startRot = transform.localRotation;
        rbody = GetComponent<Rigidbody>();
        rbody.isKinematic = true;
        cols = GetComponents<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float explosionForce;

    void Explosion(Vector3 from) {
        rbody.isKinematic = false;
        foreach(Collider c in cols)
            c.isTrigger = false;
        gameObject.layer = 11;
        rbody.AddExplosionForce(explosionForce, from, 30);

        StartCoroutine(ResetAfter(5f));
    }

    void OnTriggerEnter(Collider other) {
        if (isTrigger) {
            if(other.tag == "Player") {
                Explosion(other.transform.position);
            }
        }
    }

    IEnumerator ResetAfter(float delay) {
        yield return new WaitForSeconds(delay);
        gameObject.layer = 9;
        foreach (Collider c in cols)
            c.isTrigger = true;
        rbody.isKinematic = true;
        transform.localPosition = startPos;
        transform.localRotation= startRot;
    }
}
