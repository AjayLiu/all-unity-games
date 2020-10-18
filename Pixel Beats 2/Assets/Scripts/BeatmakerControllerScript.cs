using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class BeatmakerControllerScript : MonoBehaviour
{
    uint index = 1;
    Queue<Vector2Int> positions = new Queue<Vector2Int>();
    public GameObject framePrefab;
    public Tilemap map;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckForClicks();
    }

    void CheckForClicks() {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            Vector2Int pos = GameControllerScript.Vector3To2Int(GameControllerScript.RoundDownVector3(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
            SpawnFrame(pos, index);
        }
    }    
    void SpawnFrame(Vector2Int pos, uint num) {
        GameObject frame = Instantiate(framePrefab, new Vector3(pos.x + 0.5f, pos.y + 0.5f, -5f), Quaternion.identity);
        Text frameText = frame.GetComponentInChildren<Text>();
        frameText.text = num.ToString();
        index++;
    }
}
