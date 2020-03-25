using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontInheritRotation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.identity;
        transform.position = transform.parent.position + Vector3.up * 10;
    }
}
