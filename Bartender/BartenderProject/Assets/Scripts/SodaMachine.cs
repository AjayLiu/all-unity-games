using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SodaMachine : MonoBehaviour
{
   
    public GameObject progressBar;
    public Cups SodaType;
    private Animator animator;
    public AudioSource audio;
    public Cup Cup;
    public GameObject CupPrefab;
    private bool active = false;
    public bool cupMade;
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.enabled = false;
        active = false;
    }

    public void DoInteractive(Player player)
    {
        if (!active && player.Cup != null && !cupMade)
        {
            if (player.Cup.CupType == Cups.Clean)
            {
                player.CurrentSodaMachine = this;
                player.Anim.SetTrigger("GiveCup");
                animator.enabled = true;
                StartCoroutine(HandleInteractive());
                active = true;
            }
        }

        else if (cupMade)
        {
            player.CurrentSodaMachine = this;
            player.Anim.SetTrigger("GetCup");
            cupMade = false;
        }
    }

    IEnumerator HandleInteractive()
    {
        GameObject progressObject = Instantiate(progressBar, new Vector2(transform.position.x, transform.position.y - 10), Quaternion.identity);
        yield return new WaitForSeconds(5);
        animator.enabled = false;
        active = false;
        cupMade = true;
        Destroy(progressObject);
    }
    public Cup TakeCup()
    {
        var Soda = Instantiate(CupPrefab, transform.position, Quaternion.identity);
       
        
        return Soda.GetComponent<Cup>();
    }
    public void PutCups(Cup cup)
    {
       
          Cup = cup;

        Destroy(Cup.gameObject);

    }
}
