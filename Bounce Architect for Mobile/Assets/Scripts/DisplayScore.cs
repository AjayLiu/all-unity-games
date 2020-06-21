using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DisplayScore : MonoBehaviour
{
    Text scoreText;


    // Start is called before the first frame update
    void Start()
    {
        scoreText = GetComponent<Text>();
        scoreText.text = "Your Score: " + PlayerPrefs.GetInt("score", 0).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
