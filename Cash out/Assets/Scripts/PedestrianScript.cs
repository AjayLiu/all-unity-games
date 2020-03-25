using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianScript : MonoBehaviour
{
    public static bool allowPedestrianWarning;

    [HideInInspector] public bool isAlive = true;

    Animator anim;
    AudioTriggerScript audio;
    // Start is called before the first frame update
    void Start()
    {
        
        anim = GetComponent<Animator>();
        audio = GetComponent<AudioTriggerScript>();

    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter(Collider other){
        if(other.tag == "Player") {
            StartCoroutine(OnHit());
           
        }
    }

    IEnumerator OnHit(){
        anim.SetTrigger("Die");
        GameControllerScript.PedestrianKilled();
        isAlive = false;
        if (allowPedestrianWarning) {
            audio.PlayRandomFromList(0, true);
            allowPedestrianWarning = false;
            yield return new WaitUntil(()=> audio.finishedQueue);
            allowPedestrianWarning = true;
        }
        Destroy(this.gameObject, 30);
    }
}
