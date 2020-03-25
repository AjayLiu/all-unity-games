using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombScript : MonoBehaviour
{

    Animator anim;
    Collider2D coll;
    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isExploding)
        {
            CheckHP();
            SetText();
        }
          
    }

    public TextMesh textMesh;
    public int block_hp;
    void SetText()
    {
        textMesh  = GetComponentInChildren<TextMesh>();
        textMesh.text = block_hp.ToString();
        textMesh.fontStyle = FontStyle.Bold;
    }

    void CheckHP(){
        if (block_hp <= 0) {
            anim.SetTrigger("ignite");
            coll.enabled = false;
            coll.enabled = true;
            textMesh.gameObject.SetActive(false);
            isExploding = true;
            Destroy(gameObject, 0.873f);
        }
    }
    bool isExploding = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isExploding)
        {
            if (collision.tag== "Enemy")
            {
                Destroy(collision.gameObject);
            }
        }
    }
}
