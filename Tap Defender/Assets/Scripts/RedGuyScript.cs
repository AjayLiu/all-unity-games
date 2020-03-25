using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedGuyScript : MonoBehaviour
{

    public float walkDistance = 10;
    public float walkStepTime = 1;

    SpriteRenderer rend;

    GameControllerScript game;

    // Start is called before the first frame update
    void Start()
    {
        game = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
        Walk();       
    }

    // Update is called once per frame
    void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("finished"))
            Destroy(this.gameObject);
    }

    bool isAlive = true;

    void Walk() {
        if (isAlive) {
            transform.right = -transform.position;
            transform.position += transform.right * walkDistance;
            if(transform.position.x > 0) {
                rend.flipY = true;
            }
        }
        
        if(isAlive)
            Invoke("Walk", walkStepTime);
    }

    Animator anim;

    public void Die(bool countsTowardsPoints) {
        if (isAlive) {
            game.redguyList.Remove(gameObject);
            isAlive = false;
            PlayDeathAnim();
            if(countsTowardsPoints)
                game.OnEnemyKilled();
            if(transform.childCount > 0)
                transform.GetChild(0).gameObject.SetActive(false);
        }        
    }

    public void PlayDeathAnim() {
        anim.SetTrigger("Death");        
    }
}
