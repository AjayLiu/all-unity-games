using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectionMenuScript : MonoBehaviour
{

    public int currentIndex = -1;

    public GameObject songItemPrefab;
    public Transform contentParent;
    public List<SongInformation> songs = new List<SongInformation>();

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        PopulateScrollList();
    }


    void PopulateScrollList() {
        for(int i = 0; i < songs.Count; i++) {
            SongInformation song = songs[i];
            SongItemScript item = Instantiate(songItemPrefab, contentParent).GetComponent<SongItemScript>();          
            item.Init(song, i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadSong(int index) {
        StartCoroutine(LoadGame(index));
    }

    IEnumerator LoadGame(int index) {
        currentIndex = index;
        //wait for scene to be loaded
        yield return SceneManager.LoadSceneAsync("Game");

        //transfer info to game scene
        songs[index].data.InitMetadata();
        songs[index].data.InitGameScene(optionalIndex: index);
    }

    public void ReturnToMenuButton() {
        SceneManager.LoadScene("Start Menu");
    }
}

[System.Serializable]
public struct SongInformation {
    public string title, description;
    public Sprite img;
    public float difficulty;

    public BeatmapData data;
}