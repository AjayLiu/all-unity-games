using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombScript : MonoBehaviour
{

    Animator anim;
    GameControllerScript game;
    Collider2D col;

    // Start is called before the first frame update
    void Start()
    {
        game = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool isUsed = false;

    public void ClickedOn() {
        if (!isUsed) {
            isUsed = true;
            StartCoroutine(Boom());
        }
    }

    IEnumerator Boom() {
        anim.SetTrigger("Boom");

        //KILL ALL REDGUYS
        while(game.redguyList.Count > 0){
            game.redguyList[0].GetComponent<RedGuyScript>().Die(true);
        }

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("Finished"));

        Destroy(GameObject.FindGameObjectWithTag("Pointer"));
        Destroy(gameObject);

    }


}
