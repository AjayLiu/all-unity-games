using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text timer;

    public int seconds;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Runtime", 0, 1f);
    }

    // Update is called once per frame
    void Runtime()
    {
        
        seconds -= 1;
        int minutes = seconds / 60;
        int displaySeconds = seconds - (minutes * 60);
        timer.text = $"{minutes.ToString()}:{displaySeconds.ToString()}";
    }
}
