using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Customer : MonoBehaviour
{
    public Tile currentTile;
    public bool Ordering;

    public Cups order;
    public int Newness = 5;
    public int temper = 5;
    SpriteRenderer rend;

    public Text score;
    public AudioSource audio;
    public List<AudioClip> FootSteps;
    public List<AudioClip> Frustrations;
    public List<Texture> FrustratedFaces;
    public int FrustratedFacesIndex = -1;
    public AudioClip Chug;
    public GameObject orderBubble;
    public Sprite wantRedSprite, wantGreenSprite, wantBlueSprite;
    public Vector2 EmojiSize = new Vector2(20, 20);
    public SpriteRenderer ProgressBar;
    public List<Sprite> ProgressTexture;
    public float waitTimePerBarLoss;

    void OnGUI()
    {

        if (FrustratedFacesIndex !=-1)
        {
            var UnityScreenPos = Camera.main.WorldToScreenPoint(transform.GetChild(0).transform.position);
            var ScreenPos = new Vector2(UnityScreenPos.x, Screen.height - UnityScreenPos.y);
            GUI.DrawTexture(new Rect(ScreenPos + new Vector2(0, 0) + new Vector2(-EmojiSize.x/2,EmojiSize.y/2), new Vector2(EmojiSize.x,EmojiSize.y)), FrustratedFaces[FrustratedFacesIndex],ScaleMode.ScaleToFit);
        }
    }
    // Update is called once per frame
    private void Start()
    {
        
        order = (Cups)Random.Range((int)Cups.Red, (int)Cups.Blue + 1);
        orderBubble = transform.GetChild(0).GetChild(0).gameObject;
        orderBubble.transform.parent.gameObject.SetActive(false);

        rend = orderBubble.GetComponent<SpriteRenderer>();
        InvokeRepeating("MoveUp", 0, 2f);

        switch (order) {
            case Cups.Red:
                rend.sprite = wantRedSprite;
                break;
            case Cups.Green:
                rend.sprite = wantGreenSprite;
                break;
            case Cups.Blue:
                rend.sprite = wantBlueSprite;
                break;
        }

    }

    void Leave()
    {
        orderBubble.SetActive(false);
        ProgressBar.gameObject.SetActive(false);
        if (currentTile.NorthTile != null)
        {
            currentTile = currentTile.NorthTile;
            transform.position = currentTile.transform.position;
            


        }
        else
        {
            Destroy(gameObject);
        }
    }
    void MoveUp()
    {
        if (currentTile.SouthTile != null)
        {
            if (!currentTile.SouthTile.occupied)
            {
                audio.PlayOneShot(FootSteps[Random.Range(0, 5)],0.3f);
                currentTile = currentTile.SouthTile;
                transform.position = currentTile.transform.position;
                currentTile.occupied = true;
                currentTile.NorthTile.occupied = false;
            }
             if (currentTile.SouthTile.IsInteractive)
            {
                Ordering = true;
                DisplayOrder();
             
                CancelInvoke("MoveUp");
                InvokeRepeating("TakeCup",0,0.1f);
                InvokeRepeating("WaitingPatiently",0,waitTimePerBarLoss);
                ProgressBar.gameObject.SetActive(true);
            }


        }
    }

    void DisplayOrder() {
  
        orderBubble.transform.parent.gameObject.SetActive(true);
    }

    void TakeCup()
    {

        if (currentTile.SouthTile.transform.GetChild(0).GetComponent<Counter>().CheckIfCupOnCounter(order))
        {
            audio.PlayOneShot(Chug);
            transform.rotation = Quaternion.Euler(0, 0, 180);
            score.text = (Int32.Parse(score.text) + 1).ToString();
            currentTile.occupied = false;
            CancelInvoke("WaitingPatiently");
            CancelInvoke("WaitingAnnoyedly");
            CancelInvoke("TakeCup");
            InvokeRepeating("Leave", 0, 2);
        }

    }
    void WaitingPatiently()
    {
        
        if (Newness < 0)
        {
            FrustratedFacesIndex = Random.Range(0, FrustratedFaces.Count - 1);
            InvokeRepeating("WaitingAnnoyedly", 0, 2f);
            ProgressBar.gameObject.SetActive(false);
        }
        ProgressBar.sprite = ProgressTexture[Newness];
        Newness -= 1;
        
      
    }
    void WaitingAnnoyedly()
    {
        if (UnityEngine.Random.Range(0, 5) > temper)
        {
            FrustratedFacesIndex = -1;
            transform.rotation = Quaternion.Euler(0, 0, 180);
            InvokeRepeating("Leave", 0, 2);
            CancelInvoke("WaitingAnnoyedly");
        }
        audio.PlayOneShot(Frustrations[Random.Range(0, Frustrations.Count - 1)]);

        temper--;
    }
    private void Update()
    {
        
        if (Ordering)
        {
            
            DisplayOrder();
        }
    }
}
