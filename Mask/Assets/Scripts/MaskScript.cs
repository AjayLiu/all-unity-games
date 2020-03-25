using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskScript : MonoBehaviour {

    public int maskID;
    public bool hasMask = true;
    public Color color;
    SpriteRenderer rend;

    // Use this for initialization
    void Start () {
        rend = GetComponentInChildren<SpriteRenderer>();
        SetColor();
    }

    public void SetColor() {
        rend.color = new Color(color.r, color.g, color.b, 1f);
    }
}
