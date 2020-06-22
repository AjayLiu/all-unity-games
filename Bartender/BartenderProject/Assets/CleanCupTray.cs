using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanCupTray : MonoBehaviour
{
    
    public int CupsCount = 10;
   
    public List<CupPlacement> CupPositions;
    public GameObject CleanCupPrefab;
    public GameObject CupPosition;
    public GameObject startingpos;
    public void Start()
    {
        for (int i = 0; i < 5; i++)//instantiate the cup positions
        {
            var cupposition = Instantiate(CupPosition, transform);
            cupposition.transform.position = startingpos.transform.position + new Vector3(0, i , 0);
            CupPositions.Add(new CupPlacement(cupposition));
        }
        foreach (CupPlacement placement in CupPositions)
        {
            if (placement.Position != null)
            {
                var cup = Instantiate(CleanCupPrefab, placement.Position.transform.position, Quaternion.identity);
                cup.gameObject.transform.SetParent(placement.Position.transform);
                placement.cup = cup.GetComponent<Cup>();
               
            }
        }
    }
    public void PutCups()
    {
        foreach (CupPlacement placement in CupPositions)
        {
            if (placement.cup == null)
            {
                var cup = Instantiate(CleanCupPrefab, placement.Position.transform.position, Quaternion.identity);
                cup.gameObject.transform.SetParent(placement.Position.transform);
                placement.cup = cup.GetComponent<Cup>();


            }
        }
        CupsCount++;
    }

    public Cup TakeCup()
    {
        foreach (CupPlacement placement in CupPositions)
        {
            if (placement.cup != null)
            {
                var Cup= placement.cup;//setting cup to null while giving it
                placement.cup = null;
                return Cup;

            }
        }
        return null;
        

    }
    public void DoInteractive(Player player)
    {
        if (CupsCount != 0)
        {
            player.CurrentCleanCupTray = this;
            player.Anim.SetTrigger("GetCup");
            
        }
    }
}
