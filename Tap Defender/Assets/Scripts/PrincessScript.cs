using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrincessScript : MonoBehaviour
{

    public bool isTutorial;
    GameControllerScript game;

    // Start is called before the first frame update
    void Start()
    {
        game = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Enemy") {
            if (isTutorial) {
                StartCoroutine(OnImpactWithEnemyDuringTutorial(other));
            } else {
                print(isTutorial);
                game.GameOver();
            }
        }
    }

    IEnumerator OnImpactWithEnemyDuringTutorial(Collider2D other) {
        other.GetComponent<RedGuyScript>().Die(false);
        yield return new WaitForSeconds(2f);
        TutorialScript tutorial = GameObject.FindGameObjectWithTag("GameController").GetComponent<TutorialScript>();
        game.SpawnEnemyAtBorder();
        if(tutorial.indicateNewEnemies)
            tutorial.PutPointerAboveFirstEnemy();
    }
}
