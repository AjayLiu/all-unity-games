using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public Text contentText;
    public TextAsset howToPlayAsset, creditsAsset;
    public GameObject panel;

    // Start is called before the first frame update
    void Start()
    {
        panel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlay() {
        SceneManager.LoadScene(1);
    }

    public void OnHowTo() {
        panel.SetActive(true);
        contentText.text = howToPlayAsset.text;
    }

    public void OnCredits() {
        panel.SetActive(true);
        contentText.text = creditsAsset.text;
    }


    public void OnX() {
        panel.SetActive(false);
    }
}
