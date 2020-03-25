using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{

    [HideInInspector]public List<DamageChart> myChart = new List<DamageChart>();

    public float bulletSpeed;
    bool isDirectHit;
    bool hasHit = false;
    [HideInInspector] public bool isOutOfView;
    [HideInInspector] public bool shotFromPolice;

    Renderer rend;
    TrailRenderer trail;

    PlayerScript play;

    // Start is called before the first frame update
    void Start() {
        rend = GetComponent<Renderer>();
        play = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();

        Destroy(gameObject, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * bulletSpeed);
        isOutOfView = !rend.isVisible;
        if (isDirectHit) {
            if (Vector3.Distance(transform.position, target.position) < 1f)
                Destroy(gameObject);
            transform.LookAt(target);
        }
    }

    Transform target;

    public Gradient directHitGradient;
    public Gradient policeGradient;

    public void DrawDirectTrace(Transform target) {
        trail = GetComponent<TrailRenderer>();
        this.target = target;
        isDirectHit = true;

        trail.colorGradient = directHitGradient;
    }

    public void PoliceTrace() {
        trail = GetComponent<TrailRenderer>();
        trail.colorGradient = policeGradient;
    }

    public float policeBulletDamage;

    void OnTriggerEnter(Collider other){
        if (!hasHit && !isDirectHit) {
            if (shotFromPolice) {
                if (other.tag == "Player") {
                    play.TakeDamage(policeBulletDamage);
                    Destroy(gameObject);
                }
            } else {
                if (other.tag != "Player") {
                    HealthBarScript health = other.GetComponent<HealthBarScript>();
                    if (health != null) {
                        hasHit = true;
                        health.HitFromPlayer(myChart, false);
                    }
                    Destroy(gameObject);
                }
            }
        }        
    }
}
