using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectionMenuScript : MonoBehaviour
{
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
        foreach(SongInformation song in songs) {
            SongItemScript item = Instantiate(songItemPrefab, contentParent).GetComponent<SongItemScript>();
            item.Init(song);
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
        //wait for scene to be loaded
        yield return SceneManager.LoadSceneAsync("Game");

        //transfer info to game scene
        songs[index].data.InitGameScene();
    }

}

[System.Serializable]
public struct SongInformation {
    public string title, description;
    public Sprite img;
    public float difficulty;

    public BeatmapData data;
}