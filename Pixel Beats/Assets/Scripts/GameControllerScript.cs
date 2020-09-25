using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Directions{Up, Down, Left, Right};

public class GameControllerScript : MonoBehaviour
{
    public Tilemap map;
    public string sequence;
    public GameObject player;


    void Awake() {
        ProcessTilemap();

    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        DetectPlayerMove();
    }


    void DetectPlayerMove() {
        if (Input.GetKeyDown(KeyCode.W)) {
            Move(Directions.Up);
        }
        if (Input.GetKeyDown(KeyCode.A)) {
            Move(Directions.Left);
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            Move(Directions.Down);
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            Move(Directions.Right);
        }
    }

    List<KeyValuePair<TileBase, short>> bases = new List<KeyValuePair<TileBase, short>>();
    short[,] bitmap;
    void ProcessTilemap() {
        bitmap = new short[map.cellBounds.size.x, map.cellBounds.size.y];
        for(int i = map.cellBounds.x; i < map.cellBounds.xMax; i++) {
            for(int j = map.cellBounds.y; j < map.cellBounds.yMax; j++) {
                Vector3Int pos = new Vector3Int(i, j, 0);
                TileBase tile = map.GetTile(pos);
                if(tile!= null) {
                    //translate tilebase to an id (0,1,2...) based on dictionary (bases)
                    short id;
                    
                    KeyValuePair<TileBase, short> pair = GetPair(tile);
                    
                    //Add if doesn't exist
                    if (pair.Key == null) {
                        KeyValuePair<TileBase, short> newPair = new KeyValuePair<TileBase, short>(tile, (short)bases.Count);
                        bases.Add(newPair);
                        pair = newPair;
                    }

                    id = pair.Value;
                    
                    //Hide tile
                    map.SetTile(pos, null);

                    bitmap[i-map.cellBounds.x, j-map.cellBounds.y] = id;
                } else {
                    bitmap[i-map.cellBounds.x, j-map.cellBounds.y] = -1;
                }
            }
        }
    }

    

    void Move(Directions dir) {
        switch (dir) {
            case Directions.Up:
                player.transform.position += Vector3.up;
                break;
            case Directions.Down:
                player.transform.position += Vector3.down;
                break;
            case Directions.Left:
                player.transform.position += Vector3.left;
                break;
            case Directions.Right:
                player.transform.position += Vector3.right;
                break;
        }
        RevealTile(new Vector3Int(Mathf.FloorToInt(player.transform.position.x), Mathf.FloorToInt(player.transform.position.y), 0));
    }

    void RevealTile(Vector3Int pos) {
        int tileSpaceX = pos.x - map.cellBounds.x, tileSpaceY = pos.y - map.cellBounds.y;
        if(tileSpaceX >= 0 && tileSpaceY >= 0 && pos.x < map.cellBounds.xMax && pos.y < map.cellBounds.yMax) {
            short id = bitmap[tileSpaceX, tileSpaceY];
            TileBase tileToReveal = GetPair(id).Key;

            if (tileToReveal != null && bitmap != null) {
                map.SetTile(new Vector3Int(pos.x, pos.y, 0), tileToReveal);
            }
            print(pos);
        }    
    }








    KeyValuePair<TileBase, short> GetPair(short id) {
        for (int i = 0; i < bases.Count; i++) {
            if (bases[i].Value == id)
                return bases[i];
        }
        return new KeyValuePair<TileBase, short>(null, -2);
    }
    KeyValuePair<TileBase, short> GetPair(TileBase t) {
        for (int i = 0; i < bases.Count; i++) {
            if (bases[i].Key == t)
                return bases[i];
        }
        return new KeyValuePair<TileBase, short>(null, -2);
    }
}
