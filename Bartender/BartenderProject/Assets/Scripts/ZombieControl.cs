using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieControl : MonoBehaviour
{
    public float zombieAdvanceDelay;
    public int damagePerHit;

    public Tile[] spawnLocations;
    public GameObject zombiePrefab;

    // Start is called before the first frame update
    void Start()
    {
        SpawnZombie();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SpawnZombie() {
        Tile spawnLocation = spawnLocations[Random.Range(0, 5)];

        GameObject zombie = Instantiate(zombiePrefab, spawnLocation.transform.position, Quaternion.identity);
        Zombie zombieScript = zombie.GetComponent<Zombie>();
        zombieScript.currentTile = spawnLocation;
        zombieScript.advanceDelay = zombieAdvanceDelay;
        zombieScript.damage = damagePerHit;
    }
}
