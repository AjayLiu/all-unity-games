using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ObstacleScript : MonoBehaviour
{
    public Direction currentDirection;


    public bool isCrash = false;
    Rigidbody rbody;
    NavMeshObstacle obstacle;
    bool detectCollisions = true;

    public float drivingSpeedMin, drivingSpeedMax;

    Renderer rend;
    Collider collider;

    ParticleSystem[] particles;
    GameControllerScript game;

    // Start is called before the first frame update
    void Start()
    {

        game = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();


        particles = GetComponentsInChildren<ParticleSystem>();    

        rbody = GetComponent<Rigidbody>();
        rend = GetComponentInChildren<Renderer>();


        collider = GetComponent<Collider>();
        obstacle = GetComponent<NavMeshObstacle>();

        ResetDirection();

        
    }

    public void ResetDirection() {
        rbody.velocity = transform.forward * Random.Range(drivingSpeedMin, drivingSpeedMax);
        collider.isTrigger = false;        
    }

    // Update is called once per frame
    void Update()
    {
        if(!rend.isVisible) {
            if (isCrash) {
                game.obstaclesList.Remove(this);
                Destroy(this.gameObject);
            } 
        }

        collider.isTrigger = !rend.isVisible;

    }

    public float damageToPlayer = 30f;


    void OnCollisionEnter(Collision col){
        if(detectCollisions){
            if (col.gameObject.tag == "Player") {
                PlayerScript play = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
                play.TakeDamage(damageToPlayer);
                ShotDown(col.GetContact(0).point);
            }

            

            

        } else {
            if (col.gameObject.tag == "Building") {
                BlowUpWithEffects(explosionForceWhenShot/5, col.GetContact(0).point);
            }
        }
        Crash();
    }

    void OnTriggerEnter(Collider other) {
        if (detectCollisions) {
            if (other.gameObject.tag == "Police") {
                ShotDown(other.transform.position);
            }
        }
    }

    public float explosionForceWhenShot = 1000f;

    void ShotDownFromBullet(){
        ShotDown(transform.position);
    }

    public void ShotDown(Vector3 collisionPos){
        rbody.useGravity = true;
        rbody.drag = 1;
        rbody.angularDrag = 1;
        isCrash = true;
        detectCollisions = false;

        obstacle.enabled = false;

        gameObject.layer = 11;

        BlowUpWithEffects(explosionForceWhenShot, collisionPos);
    }

    void BlowUpWithEffects(float intensity, Vector3 pos){
        rbody.AddExplosionForce(intensity, pos, 3f);
        rbody.AddTorque(Vector3.one * Random.Range(-360, 360));
        particles[0].transform.position = pos;
        particles[0].Play();
        particles[1].Play();
        game.PlayExplosionSound();
    }

    void Crash(){
        rbody.mass = 1f;
        rbody.useGravity = true;
        rbody.drag = 1;
        rbody.angularDrag = 1;
        isCrash = true;
        //detectCollisions = false;
    }

    
}
