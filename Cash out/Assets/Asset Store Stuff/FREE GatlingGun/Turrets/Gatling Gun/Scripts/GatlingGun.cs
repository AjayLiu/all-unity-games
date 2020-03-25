using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatlingGun : MonoBehaviour
{
    // target the gun will aim at
    public Vector3 go_target;

    // Gameobjects need to control rotation and aiming
    public Transform go_baseRotation;
    public Transform go_GunBody;
    public Transform go_barrel;

    // Gun barrel rotation
    public float barrelRotationSpeed;
    float currentRotationSpeed;

    // Distance the turret can aim and fire from
    public float firingRange;

    // Particle system for the muzzel flash
    public ParticleSystem muzzelFlash;

    // Used to start and stop the turret firing
    public bool aim = false;

    public bool fire = false;

    void Start()
    {

    }

    void Update()
    {
        AimAndFire();
    }


    void AimAndFire()
    {
        // Gun barrel rotation
        go_barrel.transform.Rotate(0, 0, currentRotationSpeed * Time.deltaTime);

        // if can fire turret activates
        if (aim)
        {
            // start rotation
            currentRotationSpeed = barrelRotationSpeed;

            // aim at enemy
            Vector3 baseTargetPostition = new Vector3(go_target.x, this.transform.position.y, go_target.z);
            Vector3 gunBodyTargetPostition = new Vector3(go_target.x, go_target.y, go_target.z);

            go_baseRotation.transform.LookAt(baseTargetPostition);
            go_GunBody.transform.LookAt(gunBodyTargetPostition);

            
            
        }
        else
        {
            // slow down barrel rotation and stop
            currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, 0, 10 * Time.deltaTime);
            
        }

        if (fire) {
            // start particle system 
            if (!muzzelFlash.isPlaying) {
                muzzelFlash.Play();
            }
        } else {
            // stop the particle system

            if (muzzelFlash.isPlaying) {
                muzzelFlash.Stop();
            }
        }
    }
}