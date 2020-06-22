using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    public Tile currentTile;
    public float advanceDelay;
    public int damage;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Advance", 0f, advanceDelay);
    }

    void Advance()
    {
        if(!currentTile.SouthTile.IsInteractive)
        {

            currentTile = currentTile.SouthTile;

            transform.position = currentTile.transform.position;
        } else {
            currentTile.SouthTile.BroadcastMessage("ZombieAttack", damage);
        }
    }


}
