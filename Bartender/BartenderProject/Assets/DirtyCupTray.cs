using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtyCupTray : MonoBehaviour
{
    public CleanCupTray CleanTray;
    public List<CupPlacement> CupPositions;
    public float Timer = 0;
    public float TimeTakesToWashACup = 2;
    public GameObject CupPosition;
    public GameObject startingpos;
   
    void Start()
    {
        for(int i = 0; i < 20; i++)//instantiate the cup positions
        {
            var cupposition = Instantiate(CupPosition, transform);
            cupposition.transform.position = startingpos.transform.position + new Vector3(0, i , 0);
            CupPositions.Add(new CupPlacement(cupposition));
        }
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;
        if (Timer > TimeTakesToWashACup)
        {
            Timer = 0;
            
            foreach (CupPlacement placement in CupPositions)
            {
                if (placement.cup != null)
                {
                    Clean(placement);
                }
            }
            
        }
    }
    public void Clean(CupPlacement Placement)
    {
        CleanTray.PutCups();
        Destroy(Placement.cup.gameObject);
    }
    public void PutCups(Cup cup)
    {
        foreach (CupPlacement placement in CupPositions)
        {
            if (placement.cup == null)
            {
                placement.cup = cup;
                cup.transform.SetParent(placement.Position.transform);
                cup.transform.position = placement.Position.transform.position;
                break;
            }
        }
    }
    void DoInteractive(Player player)
    {
        player.CurrentDirtyCupTray = this;
        player.CurrentSodaMachine = null;
        player.AccessingCounter = null;
        player.CurrentCleanCupTray = null;
        player.Anim.SetTrigger("GiveCup");
       
    }
}
