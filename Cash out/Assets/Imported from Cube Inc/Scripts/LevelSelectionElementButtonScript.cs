using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;

public class LevelSelectionElementButtonScript : MonoBehaviour
{
    public int sceneIndex;

    RawImage img;
    GameObject padlockObject;

    void Start() {
        padlockObject = transform.GetChild(1).gameObject;
        padlockObject.SetActive(false);

        img = GetComponent<RawImage>();
        Texture2D texture = Resources.Load<Texture2D>("Textures/" + sceneIndex.ToString());
        img.texture = texture;

        if (sceneIndex > PlayerPrefs.GetInt("Highest Unlocked Scene")){
            LockLevel();
        }


    }



    bool isLocked = false;

    public void OnPress(){
        if (!isLocked) {
            Time.timeScale = 1;
            SceneManager.LoadScene(sceneIndex);
        }
    }

    public Color lockColor;

    void LockLevel(){
        isLocked = true;
        img.color = lockColor;
        padlockObject.SetActive(true);
        
    }
}
