using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class LeftRightButtonScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    DrawLine draw;
    public bool isLeft;

    void Start() {
        draw = GameObject.FindGameObjectWithTag("GameController").GetComponent<DrawLine>();
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (isLeft) {
            draw.leftPress = true;
        } else {
            draw.rightPress = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (isLeft) {
            draw.leftPress = false;
        } else {
            draw.rightPress = false;
        }
    }
}
