using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialScript : MonoBehaviour
{

    public GameObject messageObject;
    public Text messageText;
    public GameObject pointer;
    GameControllerScript game;
    

    // Start is called before the first frame update
    void Start()
    {
        game = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();

        StartCoroutine(Tutorial());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool indicateNewEnemies = true; 

    public IEnumerator Tutorial(){
        //LOAD FIRST 2 MESSAGES
        Time.timeScale = 0;
        DisplayNextMessage();
        yield return new WaitUntil(() => messageIndex == 3);
        messageObject.SetActive(false);
        Time.timeScale = 1;

        //SPAWN ENEMY
        game.SpawnEnemyAtBorder();
        //AND POINTER
        PutPointerAboveFirstEnemy();

        yield return new WaitUntil(()=> game.score > 0);
        game.allowTaps = false;
        yield return new WaitForSeconds(2f);
        indicateNewEnemies = false;
        GameObject ins = game.SpawnBomb();
        
        //Spawn 10 Red Guys
        for(int i = 0; i < 5; i++) {
            game.SpawnEnemyAtBorder();
        }
        messageObject.SetActive(true);
        Time.timeScale = 0;

        yield return new WaitUntil(() => messageIndex == 4);

        Text t = Instantiate(pointer, ins.transform.position + Vector3.up * 10, Quaternion.identity).GetComponentInChildren<Text>();
        t.text = "Press to Activate Bomb!";

        messageObject.SetActive(false);
        game.allowTaps = true;
        Time.timeScale = 1;

        yield return new WaitUntil(() => game.redguyList.Count == 0);
        game.allowTaps = false;
        yield return new WaitForSeconds(2f);

        GameObject temp = game.SpawnDouble();
        
        //Spawn 5 Red Guys
        for (int i = 0; i < 5; i++) {
            game.SpawnEnemyAtBorder();
        }
        messageObject.SetActive(true);
        Time.timeScale = 0;

        yield return new WaitUntil(() => messageIndex == 5);
        messageObject.SetActive(false);
        game.allowTaps = true;
        Time.timeScale = 1;

        Text txt = Instantiate(pointer, temp.transform.position + Vector3.up * 10, Quaternion.identity, temp.transform).GetComponentInChildren<Text>();
        txt.text = "Press to Activate Double Multiplier!";

        yield return new WaitUntil(()=> temp == null && game.redguyList.Count == 0);

        messageObject.SetActive(true);

        yield return new WaitUntil(() => messageIndex == 6);

        SceneManager.LoadScene(0);
    }

    int messageIndex = 0;
    public List<string> messages;

    public void DisplayNextMessage() {
        messageText.text = messages[messageIndex];
        messageIndex++;
    }

    public void PutPointerAboveFirstEnemy() {
        Instantiate(pointer, game.redguyList[0].transform.position + Vector3.up * 10, Quaternion.identity, game.redguyList[0].transform);
    }
}
