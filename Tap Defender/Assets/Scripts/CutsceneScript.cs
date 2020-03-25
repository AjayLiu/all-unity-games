using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class CutsceneScript : MonoBehaviour
{

    public Sprite[] images;
    public TextAsset[] texts;
    int currentSlide = 0;


    // Start is called before the first frame update
    void Start()
    {
        UpdateSlide();
    }

    public Image img;
    public Text text;
    public Text skipText;

    public void PlayNext() {
        currentSlide++;
        UpdateSlide();
    }

    void UpdateSlide() {
        if (currentSlide == 4) {
            SceneManager.LoadScene(0);
            return;
        }

        img.sprite = images[currentSlide];
        text.text = texts[currentSlide].ToString();
        if (currentSlide == 3) {
            skipText.text = "Start";
        }
        
    }
}
