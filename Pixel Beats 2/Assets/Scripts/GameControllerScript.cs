using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameControllerScript : MonoBehaviour
{
    public bool autoGenerateSequence = true;

    public RawImage pixelArtImage;

    public string sequenceRaw;

    Vector2Int[] sequence;

    [HideInInspector]public AudioSource audio;

    [HideInInspector] public int currentSongIndex;
    SongInformation currentSongInfo;

    int score = 0, notesHit = 0, notesTotal = 0, highestCombo = 0;

    bool allowInput = true;

    SelectionMenuScript selectionMenuScript;

    // Start is called before the first frame update
    void Start()
    {
        

        Time.timeScale = 1f;

        audio = GetComponent<AudioSource>();        
        
        ProcessTexture2D();
        if (autoGenerateSequence)
            GenerateRandomSequence();
        else
            ProcessSequence();


        pausePanel.SetActive(false);
        warningPanel.SetActive(false);
        settingsWindow.SetActive(false);

        if (!isPreviewMode)
            selectionMenuScript = GameObject.FindGameObjectWithTag("SelectionGameController").GetComponent<SelectionMenuScript>();

        if (currentSongIndex > -1)
            currentSongInfo = selectionMenuScript.songs[currentSongIndex];

        multiplierText = indicatorText.transform.GetChild(0).GetComponent<Text>();
        multiplierText.gameObject.SetActive(false); //hide it first, then show it on the first hit
        UpdateNewMultiplerTier(multiplierTiers[0]);


        //Load in binds for click
        LoadSavedSettings();
        


        //pool in some frames
        int poolFrameCount = 8;
        for(int i = 0; i < poolFrameCount; i++) {
            GameObject frame = Instantiate(framePrefab, new Vector3(1000,1000), Quaternion.identity);
            framesQueue.Enqueue(frame);
        }


        StartCoroutine(StartBeatmap());


    }

    void LoadSavedSettings() {
        clickKeys = new KeyCode[] { KeyCode.Mouse0, (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Click0", "X")), (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Click1", "Z")) };
        audio.volume = PlayerPrefs.GetFloat("Volume", 0.5f);

        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 0.5f);
        
        for (int i = 0; i < bindButtonTexts.Length; i++) {
            if (PlayerPrefs.HasKey("Click" + i))
                bindButtonTexts[i].text = PlayerPrefs.GetString("Click" + i);
        }

    }

    // Update is called once per frame
    float deltaTime = 0;
    void Update()
    {
        //deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        //float fps = 1.0f / deltaTime;
        //print(Mathf.Ceil(fps).ToString());


        if (allowInput && !isPreviewMode)
            DetectPlayerTaps();

    }

    public float clickDistanceRadius;
    KeyCode[] clickKeys;

    void DetectPlayerTaps() {

        foreach(KeyCode keycode in clickKeys) {
            if (Input.GetKeyDown(keycode)) {
                Vector2 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                //see if the user clicked on a valid note that is currently listening for input
                //extract the note that was closest to the click
                //Note closestNote = new Note();
                //float closestDistance = -1f;
                //foreach (Note n in clickableNotes) {
                //    float distance = Vector2.Distance(clickPos, n.frame.transform.position);
                //    if (closestDistance == -1f || distance < closestDistance) {
                //        closestNote = n;
                //        closestDistance = distance;
                //    }
                //}
                //if (closestDistance < clickDistanceRadius) {
                //    OnNoteClicked(closestNote);
                //    clickableNotes.Remove(closestNote);
                //}

                if(clickableNotes.Count > 0 && Vector2.Distance(clickPos, clickableNotes[0].frame.transform.position) < clickDistanceRadius) {
                    OnNoteClicked(clickableNotes[0]);
                    clickableNotes.RemoveAt(0);
                }
            }
        }
    }

    enum NoteResult {
        Perfect, Great, Ok, Bad, Missed
    };

    public Text indicatorText, scoreText;

    int scoreMultiplier = 1;

    [System.Serializable]
    public struct MultiplierTier {
        public int multiplier;
        public int streaksNeeded;
        public Color streakColor;
    };

    public MultiplierTier[] multiplierTiers;
    int multiplierTierIndex = 0;

    void OnNoteClicked(Note note) {
        float timeDifference = Mathf.Abs(Time.time - note.spawnTime - 3*(1/(bpm / 60f)));

        //Check if able to advance to next streak tier
        if(multiplierTierIndex != multiplierTiers.Length - 1) {
            if (streak > multiplierTiers[multiplierTierIndex + 1].streaksNeeded) {
                multiplierTierIndex++;
                UpdateNewMultiplerTier(multiplierTiers[multiplierTierIndex]);
            }
        }
        
        

        if (timeDifference < 0.05f) {
            UpdateIndicatorAndStreak(NoteResult.Perfect);
            score += 100 * scoreMultiplier;
        } else if(timeDifference < 0.1f){
            UpdateIndicatorAndStreak(NoteResult.Great);
            score += 75 * scoreMultiplier;
        } else if (timeDifference < 0.2f) {
            UpdateIndicatorAndStreak(NoteResult.Ok);
            score += 50 * scoreMultiplier;
        } else {
            score += 25 * scoreMultiplier;
            UpdateIndicatorAndStreak(NoteResult.Bad);
        }
        UpdateScoreUI();
        note.frame.SetActive(false);
        notesHit++;
    }

    Text multiplierText;

    void UpdateNewMultiplerTier(MultiplierTier tier) {
        scoreMultiplier = tier.multiplier;
        multiplierText.text = "x" + tier.multiplier;
        multiplierText.color = tier.streakColor;
    }

    void OnNoteMissed() {
        UpdateIndicatorAndStreak(NoteResult.Missed);
        
    }

    int streak = 0;
    public Color[] noteResultColors;
    void UpdateIndicatorAndStreak(NoteResult result) {
        //kill streak
        if(result == NoteResult.Missed || result == NoteResult.Bad) {
            streak = 0;
            multiplierTierIndex = 0;
            if(!isPreviewMode)
                UpdateNewMultiplerTier(multiplierTiers[0]);
        } else {
            streak++;
            highestCombo = Mathf.Max(streak, highestCombo);
        }



        switch (result) {
            case NoteResult.Perfect:
                indicatorText.text = "Perfect! x" + streak;
                indicatorText.color = noteResultColors[0];
                break;
            case NoteResult.Great:
                indicatorText.text = "Great! x" + streak;
                indicatorText.color = noteResultColors[1];
                break;
            case NoteResult.Ok:
                indicatorText.text = "Ok! x" + streak;
                indicatorText.color = noteResultColors[2];
                break;
            case NoteResult.Bad:
                indicatorText.text = "Bad!";
                indicatorText.color = noteResultColors[3];
                break;
            case NoteResult.Missed:
                indicatorText.text = "Missed!";
                indicatorText.color = noteResultColors[4];
                break;
        }
    }


    void UpdateScoreUI() {
        scoreText.text = score.ToString();
        multiplierText.gameObject.SetActive(true);
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
        if (framesQueue.Count != 0 && !framesQueue.Peek().activeInHierarchy) {
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

        float timeBeforeAnim = audio.time;

        //smoothly animate the outer ring to spin to targetAngles (full 180)
        float timeElapsed = 0;
        float audioTimeLastFrame = audio.time;
        while (timeElapsed < (60f / bpm) * 3) {
            // outerFrameTransform.eulerAngles = Vector3.Lerp(outerFrameTransform.eulerAngles, targetAngles, noteTurnSpeed * Time.deltaTime);
            // outerFrameTransform.localScale = Vector3.Lerp(outerFrameTransform.localScale, new Vector3(0.2f, 0.2f, 1f), noteShrinkSpeed * Time.deltaTime);

            float audioTimeDelta = audio.time - audioTimeLastFrame;

            //rotate slowly
            outerFrameTransform.eulerAngles += Vector3.forward * noteTurnSpeed * audioTimeDelta;
            //shrink slowly
            outerFrameTransform.localScale += (Vector3.left + Vector3.down) * noteShrinkSpeed * audioTimeDelta;


            timeElapsed += audioTimeDelta;
            audioTimeLastFrame = audio.time;
            yield return null;
        }
        outerFrameTransform.eulerAngles = targetAngles;
        frame.SetActive(false);

        if (enableMetronome)
            metronomeAudio.Play();

        yield return new WaitForSeconds(afterNoteThreshold * 60f * Time.deltaTime);
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
        yield return new WaitForSeconds(1f);

        audio.Play();


        float secondsToWait = waitBeginningTime;
        //float temp = Time.realtimeSinceStartup;

        //float timer = 0;
        //while (timer < secondsToWait) {
        //    timer += audio.time - timeLastFrame;
        //    timeLastFrame = audio.time;
        //    yield return null;
        //}
        //print(Time.realtimeSinceStartup - temp);

        //double timeStart = audio.time;
        //yield return new WaitUntil(() => audio.time >= timeStart + secondsToWait);

        while (audio.time < secondsToWait) {
            yield return null;
        }

        expectedAudioPosition = audio.time;
        StartCoroutine(Beats());
    }

    public bool enableMetronome = false;
    public AudioSource metronomeAudio;

    [HideInInspector]public float expectedAudioPosition = 0;
    //Called recursively every beat
    IEnumerator Beats() {

        //skip if it is a silent beat
        if (!NoteIsSilent(sequence[seqIndex])) {
            notesTotal++;
            StartCoroutine(AnimateNote(sequence[seqIndex]));
        }
        

        float beatDuration = (60f / bpm) * noteMultipliers[seqIndex];
        expectedAudioPosition += beatDuration;
        //float startTime = audio.time;
        double startDsp = AudioSettings.dspTime;
        while (audio.time < expectedAudioPosition) {
            yield return null;
        }
        //double timeStart = audio.time;
        //yield return new WaitUntil(() => audio.time >= timeStart + secondsToWait);

        seqIndex++;
        if(seqIndex == 30) {
            print(audio.time);
        }

        if (seqIndex < sequence.Length)
            StartCoroutine(Beats());
        else {
            StartCoroutine(GameOver());
        }
    }

    static GameData gameData;
    [HideInInspector] public bool isPreviewMode = false;

    IEnumerator GameOver() {
        //wait for all the current active beats to die out first before ending the game
        yield return new WaitUntil(()=>clickableNotes.Count==0);        
        yield return new WaitForSeconds(2);


        if (!isPreviewMode) {
            
            gameData = new GameData();

            GameData.score = score;

            GameData.highScore = Mathf.Max(score, PlayerPrefs.GetInt(currentSongInfo.title, 0));
            PlayerPrefs.SetInt(currentSongInfo.title, GameData.highScore);

            GameData.highestCombo = highestCombo;

            GameData.numNotesHit = notesHit;

            GameData.numNotesTotal = notesTotal;

            SceneManager.LoadScene("GameOver");
        }
        
    }

    public void UpdateBPM(int bpm) {
        this.bpm = bpm;
        noteShrinkSpeed = bpm / 300f;
    }



    #region PauseMenu

    public GameObject pausePanel, warningPanel;
    public Text warningText;

    public void PauseButton() {
        audio.Pause();
        pausePanel.SetActive(true);
        allowInput = false;
        Time.timeScale = 0;        
    }

    public void ResumeButton() {
        audio.UnPause();
        pausePanel.SetActive(false);
        allowInput = true;
        Time.timeScale = 1f;
    }


    /* SETTINGS */
    public GameObject settingsWindow;
    public void SettingsButton() {
        settingsWindow.SetActive(true);
    }

    public void XButton() {
        LoadSavedSettings();
        settingsWindow.SetActive(false);
    }

    public Slider volumeSlider;
    public void OnVolumeChanged() {
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
    }


    public void ChangeBind(int bindID) {
        StartCoroutine(BindProcess(bindID));
    }

    KeyCode userPressedKey;
    public Text[] bindDescriptions, bindButtonTexts;

    IEnumerator BindProcess(int bindID) {
        bindButtonTexts[bindID].text = "...";
        string originalDescription = bindDescriptions[bindID].text;
        bindDescriptions[bindID].text = "Press key to bind";

        yield return new WaitUntil(() => Input.anyKeyDown);

        foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode))) {
            if (Input.GetKeyDown(kcode)) {
                bindButtonTexts[bindID].text = kcode.ToString();
                bindDescriptions[bindID].text = originalDescription;
                PlayerPrefs.SetString("Click" + bindID, kcode.ToString());
            }
        }
    }
    /* END SETTINGS */


    enum RedirectYesButtonTo { Restart, Home };
    RedirectYesButtonTo redir;

    public void OnYesButton() {
        switch (redir) {
            case RedirectYesButtonTo.Home:
                SceneManager.LoadScene("Start Menu");
                break;
            case RedirectYesButtonTo.Restart:
                Restart();
                break;
        }
    }

    public void OnNoButton() {
        warningPanel.SetActive(false);
    }

    public void RestartButtonPress() {
        warningPanel.SetActive(true);
        redir = RedirectYesButtonTo.Restart;
        warningText.text = "You will lose all your progress for this song. \n\nAre you sure you want to restart?";
    }

    public void Restart() {
        selectionMenuScript.LoadSong(selectionMenuScript.currentIndex);
    }

    public void HomeButton() {
        warningPanel.SetActive(true);
        redir = RedirectYesButtonTo.Home;
        warningText.text = "You will lose all your progress for this song. \n\nAre you sure you want to return to the main menu?";
    }

    #endregion



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
