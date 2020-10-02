using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridScript : MonoBehaviour
{

    SpriteRenderer rend;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClick() {
        rend.color = rend.color == Color.red ? Color.white : Color.red;
        print("COLOR");
    }
}
