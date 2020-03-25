using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPrimaryColor : MonoBehaviour {

    Renderer rend;
    Text txt;

    void Start() {
        rend = GetComponent<Renderer>();
        if (rend != null) {
            rend.material.color = GameCon.material1.color;
        } else {
            txt = GetComponent<Text>();
            txt.color = GameCon.material1.color;
        }
    }
}
