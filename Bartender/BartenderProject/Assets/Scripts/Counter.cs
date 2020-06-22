using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour
{
    public GameObject DirtyCup;
    public Sprite[] crackedPhases;

    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    public List<CupPlacement> CupPositions;


    void Start()
    {
        spriteRenderer = GetComponentInParent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool CheckIfCupOnCounter(Cups cup)
    {
        foreach (CupPlacement placement in CupPositions)
        {
            if (placement.cup != null)
            {
                if (placement.cup.CupType == cup)
                {
                    Destroy(placement.cup.gameObject);
                    var dirtyCup = Instantiate(DirtyCup, placement.Position.transform.position, Quaternion.identity);
                    placement.cup = dirtyCup.GetComponent<Cup>();
                    dirtyCup.transform.SetParent(placement.Position.transform);
                    return true;
                }
            }
        }
        return false;
    }

    public void PutCups(Cup cup)
    {
        foreach(CupPlacement placement in CupPositions)
        {
            if(placement.cup == null)
            {
                placement.cup = cup;
                
                cup.transform.SetParent(placement.Position.transform);
                cup.transform.position = placement.Position.transform.position;
                break;
            }
        }
    }
    public Cup GetDirtyCup()
    {
        foreach (CupPlacement placement in CupPositions)
        {
            if (placement.cup != null)
            {
                if (placement.cup.CupType == Cups.Dirty)
                {
                    var dirtycup = placement.cup;
                    placement.cup = null;
                    return dirtycup;
                    
                }
            }
        }

        Debug.Log("Error with counter get dirty cup system");
        return null;
    }
    void DoInteractive(Player player)
    {
        if (player.Cup != null)
        {
            player.CurrentSodaMachine = null;
            player.AccessingCounter = this;
            player.Anim.SetTrigger("GiveCup");
        }
        else
        {
            
            player.CurrentSodaMachine = null;
            player.CurrentCleanCupTray = null;
            player.AccessingCounter = this;
            player.Anim.SetTrigger("GetCup");
            print("take cup");
        }
    }

    int counterHealth = 100;


    void ZombieAttack (int damage) {        
        counterHealth -= damage;
        print(counterHealth);
        if (counterHealth <= 0)
        {
            spriteRenderer.sprite = null;
        }
        else if (counterHealth < 20)
        {
            spriteRenderer.sprite = crackedPhases[crackedPhases.Length - 1];
        }
        else if (counterHealth < 40)
        {
            spriteRenderer.sprite = crackedPhases[crackedPhases.Length - 2];
        }
        else if (counterHealth < 60)
        {
            spriteRenderer.sprite = crackedPhases[crackedPhases.Length - 3];
        }
        else if (counterHealth < 80)
        {
            spriteRenderer.sprite = crackedPhases[crackedPhases.Length - 4];
        }



    }
}
[System.Serializable]
public class CupPlacement
{
    public GameObject Position;
    public Cup cup;
    public CupPlacement(GameObject Position)
    {


        this.Position = Position;
        
    }
}
