using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplierScript : MonoBehaviour
{

    BeatmakerControllerScript controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.FindGameObjectWithTag("BeatmapGameController").GetComponent<BeatmakerControllerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSubmit() {
        controller.OnMultiplierChange(int.Parse(transform.parent.GetComponentInChildren<Text>().text), float.Parse(transform.GetChild(2).GetComponentInChildren<Text>().text));
    }
}
