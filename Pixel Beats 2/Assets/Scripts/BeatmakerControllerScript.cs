using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SFB;
using System.Runtime.InteropServices;

public class BeatmakerControllerScript : MonoBehaviour
{
    int index = 1;
    List<SequenceElement> sequence = new List<SequenceElement>();
    public GameObject framePrefab;
    public GameObject framesParent;

    public GameObject dontDestroyParent;


    static BeatmakerControllerScript selfInstance;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(dontDestroyParent);
        //ensure only one copy of dont destroy exists
        if (selfInstance == null)
            selfInstance = this;
        else
            Destroy(dontDestroyParent);
    }

    // Update is called once per frame
    void Update()
    {
        if(!isPreviewing)
            CheckForClicks();
    }

    void CheckForClicks() {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            if (!PointerOverUI.IsPointerOverUIElement()) {
                Vector2Int pos = GameControllerScript.Vector3To2Int(GameControllerScript.RoundDownVector3(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
                SpawnFrame(pos);
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            if (!PointerOverUI.IsPointerOverUIElement()) {
                Vector2Int pos = GameControllerScript.Vector3To2Int(GameControllerScript.RoundDownVector3(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
                RemoveFrame(pos);
            }
        }
    }
    
    void SpawnFrame(Vector2Int pos) {
        //check if there already is one there
        int ind = IndexOfElementAtPos(pos);
        if (ind == -1) {                        
            AddToHistory(new HistoryElement(CreateSequenceElement(pos, index), true));
        }
    }

    void RemoveFrame(Vector2Int pos) {
        //check if there already is one there
        int ind = IndexOfElementAtPos(pos);
        if (ind != -1) {
            //remove the element
            SequenceElement elem = sequence[ind];
            RemoveSequenceElement(elem, ind);
            AddToHistory(new HistoryElement(elem, false));
        }
    }

    SequenceElement CreateSequenceElement(Vector2Int pos, int ind) {

        GameObject frame = Instantiate(framePrefab, new Vector3(pos.x + 0.5f, pos.y + 0.5f, -5f), Quaternion.identity, framesParent.transform);
        SequenceElement elem = new SequenceElement(pos, frame, ind);
        
        //if the ind does not match index, it means it should be inserted and every element in the sequence after must +1 to their index
        if(ind != index) {
            //shift all the elements after up by 1
            for (int i = ind-1; i < sequence.Count; i++) {
                SequenceElement e = sequence[i];
                e.index++;
                e.frameText.text = e.index.ToString();
            }
        }
        sequence.Insert(ind-1, elem);


        index++;
        return elem;
    }
    void RemoveSequenceElement(SequenceElement elem, int ind) {
        sequence.Remove(elem);
        Destroy(elem.frame);
        index--;

        //shift all the elements after down by 1
        for (int i = ind; i < sequence.Count; i++) {
            SequenceElement e = sequence[i];
            e.index--;
            e.frameText.text = e.index.ToString();
        }
    }

    int IndexOfElementAtPos(Vector2Int pos) {
        for(int i = 0; i < sequence.Count; i++) {
            if(sequence[i].pos == pos) {
                return i;
            }
        }
        return -1;
    }


    //results follow the format pos1.x,pos1.y;pos2.x,pos2.y;pos3.x,pos3.y   ...etc
    string SequenceToString() {
        string results = "";
        foreach (SequenceElement e in sequence) {
            results += e.pos.x + "," + e.pos.y;
            if (e != sequence[sequence.Count - 1])
                results += ";";
        }
        return results;
    }

    public void Export() {
        string results = SequenceToString();
        print(results);
        print("Total Notes: " + sequence.Count);
        //copy results to clipboard
        TextEditor te = new TextEditor();
        te.text = results;
        te.SelectAll();
        te.Copy();
        
    }

    public struct HistoryElement {
        public SequenceElement seq;
        public bool wasAdded;
        public HistoryElement(SequenceElement s, bool added) {
            seq = s;
            wasAdded = added;
        }
    }

    int historyIndex = 0;
    public List<HistoryElement> history = new List<HistoryElement>();
    void AddToHistory(HistoryElement h) {
        //clear rest of history if a new branch has started
        history.RemoveRange(historyIndex, history.Count - historyIndex);
        history.Add(h);
        historyIndex++;
    }

    void ReverseActionInHistory(HistoryElement h, bool isRedo) {        

        //Redo does the reverse of undo
        if (!isRedo) {
            if (h.wasAdded) {
                int ind = IndexOfElementAtPos(h.seq.pos);
                RemoveSequenceElement(sequence[ind], ind);
            } else {
                CreateSequenceElement(h.seq.pos, h.seq.index);
            }
        } else {
            if (h.wasAdded) {
                CreateSequenceElement(h.seq.pos, h.seq.index);
            } else {
                int ind = IndexOfElementAtPos(h.seq.pos);
                RemoveSequenceElement(sequence[ind], ind);
            }
        }
        
    }

    public void Undo() {
        if(historyIndex > 0) {
            //reverse what was done in the history
            ReverseActionInHistory(history[historyIndex-1], false);
            historyIndex--;
        }
    }
    public void Redo() {
        if(historyIndex < history.Count) {
            ReverseActionInHistory(history[historyIndex], true);
            historyIndex++;
        }
    }

    bool isPreviewing = false;
    public Text previewButtonText;
    public void Preview() {
        StartCoroutine(LoadInPreviewScene());
    }

    public GameObject exitPreviewButton;
    IEnumerator LoadInPreviewScene() {
        isPreviewing = true;
        framesParent.SetActive(false);

        //wait for scene to be loaded
        yield return SceneManager.LoadSceneAsync("Game");
        GameControllerScript game = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();

        //import preview information
        game.pixelArtImage.texture = pixelArtImage.texture;
        ((Texture2D)game.pixelArtImage.texture).Apply();

        game.sequenceRaw = SequenceToString();
        game.autoGenerateSequence = false;

        //hide this scene's pixelart
        pixelArtImage.gameObject.SetActive(false);

        //enable ExitPreview button
        exitPreviewButton.SetActive(true);
    }

    public void ExitPreview() {
        SceneManager.LoadScene("Beatmap Maker");
        framesParent.SetActive(true);
        exitPreviewButton.SetActive(false);
        pixelArtImage.gameObject.SetActive(true);
        isPreviewing = false;
    }

    public void ImportPixelArt() {
        //file explorer thanks to https://github.com/gkngkc/UnityStandaloneFileBrowser/blob/5b9318f207569331587fa321de47497c8113040e/Assets/StandaloneFileBrowser/Sample/CanvasSampleOpenFileImage.cs
#if UNITY_WEBGL && !UNITY_EDITOR
        //
        // WebGL
        //
        [DllImport("__Internal")]
        private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

        UploadFile(gameObject.name, "OnFileUpload", ".png, .jpg", false);

        // Called from browser
        public void OnFileUpload(string url) {
            StartCoroutine(OutputRoutine(url));
        }
#else
        //
        // Standalone platforms & editor
        //

        var paths = StandaloneFileBrowser.OpenFilePanel("Title", "", "png", false);
        if (paths.Length > 0) {
            StartCoroutine(OutputRoutine(new System.Uri(paths[0]).AbsoluteUri));
        }  
#endif
    }

    public RawImage pixelArtImage;

    private IEnumerator OutputRoutine(string url) {
        var loader = new WWW(url);
        yield return loader;
        pixelArtImage.texture = loader.texture;
        pixelArtImage.texture.filterMode = FilterMode.Point;
        ((Texture2D)pixelArtImage.texture).Apply();
    }

}

public class SequenceElement {
    public Vector2Int pos;
    public GameObject frame;
    public int index;
    public Text frameText;
    public SequenceElement(Vector2Int p, GameObject f, int index) {
        pos = p;
        frame = f;
        this.index = index;
        frameText = f.GetComponentInChildren<Text>();
        frameText.text = index.ToString();
    }
}
