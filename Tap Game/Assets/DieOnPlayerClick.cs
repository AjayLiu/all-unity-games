using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieOnPlayerClick : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    public float speed;

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, Vector3.zero, speed * Time.deltaTime);
    }

    void OnMouseDown(){
        Destroy(gameObject);
    }
    
}