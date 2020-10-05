using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameControllerScript : MonoBehaviour
{
    public bool autoGenerateSequence = true;
    public Tilemap map;
    public string sequenceRaw;
    public GameObject frame;

    Vector3Int[] sequence;

    void Awake() {
        ProcessTilemap();
        if (autoGenerateSequence)
            GenerateRandomSequence();
        else
            ProcessSequence();        
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Beats());
    }

    // Update is called once per frame
    void Update()
    {
    }

    void DetectPlayerTaps() {
        
    }

    List<KeyValuePair<TileBase, short>> bases = new List<KeyValuePair<TileBase, short>>();
    short[,] bitmap;
    List<Vector3Int> tilePositions = new List<Vector3Int>();
    void ProcessTilemap() {
        bitmap = new short[map.cellBounds.size.x, map.cellBounds.size.y];
        for(int i = map.cellBounds.x; i < map.cellBounds.xMax; i++) {
            for(int j = map.cellBounds.y; j < map.cellBounds.yMax; j++) {
                Vector3Int pos = new Vector3Int(i, j, 0);
                TileBase tile = map.GetTile(pos);
                if(tile!= null) {
                    tilePositions.Add(pos);
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

                    bitmap[i-map.cellBounds.x, j-map.cellBounds.y] = id;

                    //hide the tile
                    SetTileActive(pos, false);
                } else {
                    bitmap[i-map.cellBounds.x, j-map.cellBounds.y] = -1;
                }
            }
        }
    }

    void GenerateRandomSequence() {
        //randomize the location list
        for (int i = 0; i < tilePositions.Count; i++) {
            Vector3Int temp = tilePositions[i];
            int randomIndex = Random.Range(i, tilePositions.Count);
            tilePositions[i] = tilePositions[randomIndex];
            tilePositions[randomIndex] = temp;
        }

        //copy tilePositions to sequence
        sequence = new Vector3Int[tilePositions.Count];
        for (int i = 0; i < tilePositions.Count; i++) {
            sequence[i] = tilePositions[i];
        }
    }


    //convert string ex: 1,2;6,3;7,1 into coordinates (1,2), (6,3), (7,1)
    void ProcessSequence() {
        string[] pairTokens = sequenceRaw.Split(';');
        sequence = new Vector3Int[pairTokens.Length];
        for(int i = 0; i < pairTokens.Length; i++) {
            string[] coordTokens = pairTokens[i].Split(',');
            sequence[i] = new Vector3Int(int.Parse(coordTokens[0]), int.Parse(coordTokens[1]), 0);
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


    int seqIndex = 0;
    void NextNote() {
        frame.transform.position = new Vector3(sequence[seqIndex].x + 0.5f, sequence[seqIndex].y + 0.5f, -1);
        SetTileActive(sequence[seqIndex], true);
        print(sequence[seqIndex]);
        seqIndex++;        
    }


    public float beatDuration = 0.8f;
    //Called recursively every beat
    IEnumerator Beats() {
        NextNote();
        yield return new WaitForSeconds(beatDuration);
        if (seqIndex < sequence.Length)
            StartCoroutine(Beats());
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
