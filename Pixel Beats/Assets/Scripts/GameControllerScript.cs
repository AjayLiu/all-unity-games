using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Directions{Up, Down, Left, Right};

public class GameControllerScript : MonoBehaviour
{
    public bool mappingMode = false;
    public Tilemap map;
    public string sequenceRaw;
    public GameObject player;
    public GameObject frame;

    Directions[] sequence;

    void Awake() {
        ProcessTilemap();
        if (!mappingMode) {
            ProcessSequence();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        CheckForAdvancement();
    }

    // Update is called once per frame
    void Update()
    {
        MappingModeUpdate();
        DetectPlayerMove();
    }

    string mappingResult = "";
    void MappingModeUpdate() {
        if (mappingMode) {
            if (Input.GetKeyDown(KeyCode.W)) {
                mappingResult += "U";
            }
            if (Input.GetKeyDown(KeyCode.A)) {
                mappingResult += "L";
            }
            if (Input.GetKeyDown(KeyCode.S)) {
                mappingResult += "D";
            }
            if (Input.GetKeyDown(KeyCode.D)) {
                mappingResult += "R";
            }
            if (Input.GetKeyDown(KeyCode.Space)) {
                print(mappingResult);
            }
        }
    }

    void DetectPlayerMove() {
        if (!isMoving) {
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
                    if(!mappingMode)
                        map.SetTile(pos, null);

                    bitmap[i-map.cellBounds.x, j-map.cellBounds.y] = id;
                } else {
                    bitmap[i-map.cellBounds.x, j-map.cellBounds.y] = -1;
                }
            }
        }
    }


    
    void ProcessSequence() {
        sequence = new Directions[sequenceRaw.Length];
        for(int i = 0; i < sequenceRaw.Length; i++) {
            switch (sequenceRaw[i]) {
                case 'L':
                    sequence[i] = Directions.Left;
                    break;
                case 'R':
                    sequence[i] = Directions.Right;
                    break;
                case 'U':
                    sequence[i] = Directions.Up;
                    break;
                case 'D':
                    sequence[i] = Directions.Down;
                    break;
            }
        }
    }



    
    void Move(Directions dir) {
        Vector3 desiredMoveLocation = AddDirectionToPosition(dir, player.transform.position);        
        StartCoroutine(SmoothMove(desiredMoveLocation));
    }

    float smoothDuration = 0.1f;
    bool isMoving = false;
    IEnumerator SmoothMove(Vector3 dest) {
        isMoving = true;
        float timeElapsed = 0;
        while(timeElapsed < smoothDuration) {
            player.transform.position = Vector3.Lerp(player.transform.position, dest, 20f * Time.deltaTime);
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        print("OK");
        player.transform.position = dest;
        CheckForAdvancement();
        isMoving = false;
    }

    Vector3 AddDirectionToPosition(Directions dir, Vector3 originalPos) {
        switch (dir) {
            case Directions.Up:
                return originalPos + Vector3.up;
            case Directions.Down:
                return originalPos + Vector3.down;
            case Directions.Left:
                return originalPos + Vector3.left;
            case Directions.Right:
                return originalPos + Vector3.right;
        }
        return originalPos;
    }

    int seqIndex = 0;
    Vector3Int nextPos;

    //If the player walks onto the nextPos, show the next one in the sequence
    void CheckForAdvancement() {
        if(RoundDownVector3(player.transform.position) == nextPos || seqIndex == 0) {
            if (!mappingMode) {
                nextPos = RoundDownVector3(AddDirectionToPosition(sequence[seqIndex++], RoundDownVector3(player.transform.position)));
                SetTileActive(nextPos, true);
                frame.transform.position = new Vector3(nextPos.x + 0.5f, nextPos.y + 0.5f);
            } else {
                SetTileActive(RoundDownVector3(player.transform.position), false);
            }
        }
    }

    void SetTileActive(Vector3Int pos, bool active) {
        int tileSpaceX = pos.x - map.cellBounds.x, tileSpaceY = pos.y - map.cellBounds.y;
        
        if(tileSpaceX >= 0 && tileSpaceY >= 0 && pos.x < map.cellBounds.xMax && pos.y < map.cellBounds.yMax) {
            short id = bitmap[tileSpaceX, tileSpaceY];
            TileBase tileToReveal = GetPair(id).Key;

            if (active) {
                if (tileToReveal != null && bitmap != null) {
                    map.SetTile(new Vector3Int(pos.x, pos.y, 0), tileToReveal);
                }
            } else {
                map.SetTile(new Vector3Int(pos.x, pos.y, 0), null);
            }

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


    const float roundThreshold = 0.2f;
    Vector3Int RoundDownVector3(Vector3 v) {
        
        return new Vector3Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y), 0);
    }
}
