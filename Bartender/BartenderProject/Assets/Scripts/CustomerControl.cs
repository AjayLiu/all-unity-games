using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomerControl : MonoBehaviour
{
    // Start is called before the first frame update
    public Tile[] spawnPositions;
    public GameObject customerPrefab;
    public Text score;
    void Start()
    {
        StartCoroutine(SpawnCustomers(10));
    }

    IEnumerator SpawnCustomers(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Tile spawnLocation = spawnPositions[Random.Range(0, 5)];
            if (!spawnLocation.occupied)
            {
                spawnLocation.occupied = true;
                GameObject customer = Instantiate(customerPrefab, spawnLocation.transform.position, Quaternion.identity);
                Customer customerScript = customer.GetComponent<Customer>();
                customerScript.currentTile = spawnLocation;
                customerScript.score = score;
            }
            yield return new WaitForSeconds(3f);
        }       
    }
}
