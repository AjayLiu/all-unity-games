using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    PlayerScript play;
    public float offset;
    public float speed;

    GameControllerScript game;
    // Start is called before the first frame update
    void Start()
    {
        play = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        game = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
    }

    public bool isTrailMode = false;


    // Update is called once per frame
    void Update()
    {
        float interpolation = speed * Time.deltaTime;

        Vector3 position = new Vector3();


        //transform.eulerAngles = new Vector3(transform.eulerAngles.x, play.transform.eulerAngles.y, 0);
        //transform.eulerAngles = new Vector3(transform.eulerAngles.x, (int)PlayerScript.currentDirection, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, (int)PlayerScript.currentDirection, 0), interpolation);

        if (isTrailMode) {
            position = Vector3.Lerp(transform.position, play.transform.position - GameControllerScript.dirToVector() * -offset + new Vector3(0,transform.position.y,0), interpolation);
        } else {
            switch (PlayerScript.currentDirection) {
                case Direction.North:
                    position = Vector3.Lerp(transform.position, new Vector3(game.latestChunkPosEnd.x, transform.position.y, play.transform.position.z + offset), interpolation);
                    break;
                case Direction.East:
                    position = Vector3.Lerp(transform.position, new Vector3(play.transform.position.x + offset, transform.position.y, game.latestChunkPosEnd.z), interpolation);
                    break;
                case Direction.South:
                    position = Vector3.Lerp(transform.position, new Vector3(game.latestChunkPosEnd.x, transform.position.y, play.transform.position.z - offset), interpolation);
                    break;
                case Direction.West:
                    position = Vector3.Lerp(transform.position, new Vector3(play.transform.position.x - offset, transform.position.y, game.latestChunkPosEnd.z), interpolation);
                    break;
            }
        }
        

        transform.position = position;

    }

    public void OnPlayerTurn() {
        
    }


}
