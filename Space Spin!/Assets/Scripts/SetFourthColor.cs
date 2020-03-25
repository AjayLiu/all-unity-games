using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SetFourthColor : MonoBehaviour {
    Renderer rend;
    Text txt;

    void Start() {
        rend = GetComponent<Renderer>();
        if (rend != null) {
            rend.material.color = GameCon.material4.color;
        } else {
            txt = GetComponent<Text>();
            txt.color = GameCon.material4.color;
        }
    }
}
