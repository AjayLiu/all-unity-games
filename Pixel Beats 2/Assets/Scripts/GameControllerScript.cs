using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControllerScript : MonoBehaviour
{
    public bool autoGenerateSequence = true;

    public RawImage pixelArtImage;

    public string sequenceRaw;

    Vector2Int[] sequence;

    [HideInInspector]public AudioSource audio;
    

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();

        ProcessTexture2D();
        if (autoGenerateSequence)
            GenerateRandomSequence();
        else
            ProcessSequence();

        StartCoroutine(StartBeatmap());

        audio.Play();
    }

    // Update is called once per frame
    void Update()
    {
        DetectPlayerTaps();
    }


    public float clickDistanceRadius;

    void DetectPlayerTaps() {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            Vector2 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //see if the user clicked on a valid note that is currently listening for input
            //extract the note that was closest to the click
            Note closestNote = new Note();
            float closestDistance = 0;
            foreach (Note n in clickableNotes) {
                float distance = Vector2.Distance(clickPos, n.location);
                if (closestDistance == 0 || distance < closestDistance) {
                    closestNote = n;
                    closestDistance = distance;
                }
            }
            if(closestDistance < clickDistanceRadius) {
                OnNoteClicked(closestNote);
                clickableNotes.Remove(closestNote);
            }
        }
    }

    enum NoteResult {
        Perfect, Great, Ok, Bad, Missed
    };

    public Text indicatorText;

    void OnNoteClicked(Note note) {
        float timeDifference = Mathf.Abs(Time.time - note.spawnTime - 3*(1/(bpm / 60f)));
        if(timeDifference < 0.2f) {
            UpdateIndicatorAndStreak(NoteResult.Perfect);
        } else if(timeDifference < 0.4f){
            UpdateIndicatorAndStreak(NoteResult.Great);
        } else if (timeDifference < 0.6f) {
            UpdateIndicatorAndStreak(NoteResult.Ok);
        } else {
            UpdateIndicatorAndStreak(NoteResult.Bad);
        }

        note.frame.SetActive(false);
    }

    void OnNoteMissed() {
        UpdateIndicatorAndStreak(NoteResult.Missed);
    }


    int streak = 0;
    void UpdateIndicatorAndStreak(NoteResult result) {
        

        //kill streak
        if(result == NoteResult.Missed || result == NoteResult.Bad) {
            streak = 0;
        } else {
            streak++;
        }

        switch (result) {
            case NoteResult.Perfect:
                indicatorText.text = "Perfect! x" + streak;
                break;
            case NoteResult.Great:
                indicatorText.text = "Great! x" + streak;
                break;
            case NoteResult.Ok:
                indicatorText.text = "Ok! x" + streak;
                break;
            case NoteResult.Bad:
                indicatorText.text = "Bad!";
                break;
            case NoteResult.Missed:
                indicatorText.text = "Missed!";
                break;
        }
    }


    void ProcessTexture2D() {
        originalPixelArt = (Texture2D)pixelArtImage.texture;
        originalPixelArt.filterMode = FilterMode.Point;
        originalPixelArt.Apply();
        ClearPixelArt();
    }

    Texture2D originalPixelArt;

    void SetTileActive(Vector2Int pos, bool active) {
        Texture2D newTex = (Texture2D)pixelArtImage.texture;
        newTex.SetPixel(pos.x - 8, pos.y - 8, active ? originalPixelArt.GetPixel(pos.x + 8, pos.y + 8) : new Color(0, 0, 0, 0));
        newTex.filterMode = FilterMode.Point;
        newTex.Apply();
    }

    void ClearPixelArt() {
        Color[] transparentArr = new Color[256];
        for (int i = 0; i < transparentArr.Length; i++) {
            transparentArr[i] = new Color(0, 0, 0, 0);
        }
        Texture2D tex = new Texture2D(16, 16);
        tex.SetPixels(transparentArr);
        tex.filterMode = 0;
        tex.Apply();
        pixelArtImage.texture = tex;
    }

    void GenerateRandomSequence() {
        List<Vector2Int> tilePositions = new List<Vector2Int>(256);
        for(int i = -8; i < 8; i++) {
            for (int j = -8; j < 8; j++) {
                tilePositions.Add(new Vector2Int(i, j));
            }
        }

        //shuffle the location list
        for (int i = 0; i < tilePositions.Count; i++) {
            Vector2Int temp = tilePositions[i];
            int randomIndex = Random.Range(i, tilePositions.Count);
            tilePositions[i] = tilePositions[randomIndex];
            tilePositions[randomIndex] = temp;
        }

        //copy tilePositions to sequence
        //also initialize noteMultipliers to 1
        sequence = new Vector2Int[tilePositions.Count];
        noteMultipliers = new float[tilePositions.Count];
        for (int i = 0; i < tilePositions.Count; i++) {
            sequence[i] = tilePositions[i];
            noteMultipliers[i] = 1;
        }
    }

    float[] noteMultipliers;
    //convert string ex: 1,2;6,3;7,1 into coordinates (1,2), (6,3), (7,1)
    void ProcessSequence() {
        string[] pairTokens = sequenceRaw.Split(';');
        sequence = new Vector2Int[pairTokens.Length];
        noteMultipliers = new float[pairTokens.Length];
        for (int i = 0; i < pairTokens.Length; i++) {
            char[] delimiters = { ',', 'x' };
            string[] tokens = pairTokens[i].Split(delimiters);
            sequence[i] = new Vector2Int(int.Parse(tokens[0]), int.Parse(tokens[1]));

            //custom multiplier speed
            noteMultipliers[i] = tokens.Length > 2 ? float.Parse(tokens[2]) : 1;
        }
    }
    

    struct Note {
        public Vector2Int location;
        public float spawnTime;
        public GameObject frame;

        public Note(Vector2Int v, float t, GameObject frame) {
            location = v;
            spawnTime = t;
            this.frame = frame;
        }

        
    };

   

    List<Note> clickableNotes = new List<Note>();

    public float afterNoteThreshold; //how many seconds can the user still click after the note has already disappeared

    public int concurrentNotes = 3; //how many notes to show ahead of the current note
    public float noteTurnSpeed = 60, noteShrinkSpeed;
    public GameObject framePrefab;

    Queue<GameObject> framesQueue = new Queue<GameObject>(); //for reuse instead of destroying

    IEnumerator AnimateNote(Vector2Int location) {

        GameObject frame;
        Vector3 framePos = new Vector3(sequence[seqIndex].x + 0.5f, sequence[seqIndex].y + 0.5f, -1);

        //try to reuse from existing frames
        if(framesQueue.Count != 0 && !framesQueue.Peek().activeInHierarchy) {
            frame = framesQueue.Dequeue();
        } else {
            //make new frame
            frame = Instantiate(framePrefab, framePos, Quaternion.identity);
        }
        framesQueue.Enqueue(frame);

        //reset the frame
        frame.SetActive(true);
        frame.transform.position = framePos;        
        Transform outerFrameTransform = frame.transform.GetChild(0);
        outerFrameTransform.localScale = Vector3.one * 0.8f;

        Text noteText = frame.GetComponentInChildren<Text>();

        noteText.text = (seqIndex % 4 + 1).ToString(); // change text of note to 1, 2, 3, 4

        Vector3 targetAngles = transform.eulerAngles + 180f * Vector3.forward;
        SetTileActive(sequence[seqIndex], true);

        Note note = new Note(sequence[seqIndex], Time.time, frame);
        clickableNotes.Add(note);

        //smoothly animate the outer ring to spin to targetAngles (full 180)
        float timeElapsed = 0;
        while(timeElapsed < (60f / bpm) * concurrentNotes) {
            // outerFrameTransform.eulerAngles = Vector3.Lerp(outerFrameTransform.eulerAngles, targetAngles, noteTurnSpeed * Time.deltaTime);
            // outerFrameTransform.localScale = Vector3.Lerp(outerFrameTransform.localScale, new Vector3(0.2f, 0.2f, 1f), noteShrinkSpeed * Time.deltaTime);

            //rotate slowly
            outerFrameTransform.eulerAngles += Vector3.forward * noteTurnSpeed * Time.deltaTime;
            //shrink slowly
            outerFrameTransform.localScale += (Vector3.left + Vector3.down) * noteShrinkSpeed * Time.deltaTime;
            
            
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();           
        }
        outerFrameTransform.eulerAngles = targetAngles;
        frame.SetActive(false);

        yield return new WaitForSeconds(afterNoteThreshold);
        //if user did not click on this note
        if (clickableNotes.Contains(note)) {
            OnNoteMissed();
            clickableNotes.Remove(note);
        }       
    }


    public int bpm = 120;
    int seqIndex = 0;

    public float waitBeginningTime = 0;
    IEnumerator StartBeatmap() {
        yield return new WaitForSeconds(waitBeginningTime);
        StartCoroutine(Beats());
    }

    //Called recursively every beat
    IEnumerator Beats() {

        //skip if it is a silent beat
        if (!NoteIsSilent(sequence[seqIndex])) {
            StartCoroutine(AnimateNote(sequence[seqIndex]));
        }

       

        yield return new WaitForSeconds((60f / bpm) * noteMultipliers[seqIndex]);

        seqIndex++;

        if (seqIndex < sequence.Length)
            StartCoroutine(Beats());
    }


    public void UpdateBPM(int bpm) {
        this.bpm = bpm;
        noteShrinkSpeed = bpm / 300f;
    }




    //if the note's location is out of bounds, it is a silent note
    bool NoteIsSilent(Vector2Int v) {
        return v.x > 7 || v.x < -8 || v.y > 7 || v.y < -8;
    }
    const float roundThreshold = 0.2f;
    public static Vector3Int RoundDownVector3(Vector3 v) {        
        return new Vector3Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y), 0);
    }
    public static Vector3Int RoundClosestVector3(Vector3 v) {
        return new Vector3Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), 0);
    }
    public static Vector3Int Vector2To3Int(Vector2Int v) {
        return new Vector3Int(v.x, v.y, 0);
    }
    public static Vector2Int Vector3To2Int(Vector3Int v) {
        return new Vector2Int(v.x, v.y);
    }
}
