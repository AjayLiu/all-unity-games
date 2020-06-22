using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Tile CurrentTileIn;
    public Directions direction = Directions.North;
    public GameObject CleanCupPrefab;
    public GameObject GreenSodaPrefab;
    public GameObject BlueSodaPrefab;
    public GameObject RedSodaPrefab;
    public GameObject Hand;
    public Cup Cup;
    public SodaMachine CurrentSodaMachine;
    public CleanCupTray CurrentCleanCupTray;
    public DirtyCupTray CurrentDirtyCupTray;
    public Counter AccessingCounter;
    public Animator Anim;
    public bool ToGiveSoda;
    public bool ToGetSoda;
    public bool PlayingAnimation;
    public int AnimBool_HasCup = Animator.StringToHash("HasCup");
    AudioSource audio;
    public List<AudioClip> FootStep;
    public AudioClip Mug;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = CurrentTileIn.transform.position;
        audio = GetComponent<AudioSource>();
            Anim = GetComponent<Animator>();
        if(Cup != null)
        {
            Anim.SetTrigger("HasCup");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Hand.transform.childCount == 0)
        {
            Cup = null;
        }
        if (Cup != null)
        {
            Anim.SetBool(AnimBool_HasCup, true);
        }
        else
        {
            Anim.SetBool(AnimBool_HasCup, false);
        }
        if (ToGetSoda)
        {
            GetSoda();
            ToGetSoda = false;
        }
        if (ToGiveSoda)
        {
            GiveCup();
            ToGiveSoda = false;
        }
        if (PlayingAnimation==false)
        {
            CheckInput();
     
        }

        
    }
    void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            direction = Directions.North;
            transform.eulerAngles = new Vector3(0, 0, 180);
            if (CurrentTileIn.NorthTile != null && !CurrentTileIn.NorthTile.occupied)
            {
                CurrentTileIn = CurrentTileIn.NorthTile;
                transform.position = CurrentTileIn.transform.position;
                audio.PlayOneShot(FootStep[Random.Range(0,5)]);

            }

        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            direction = Directions.South;
            transform.eulerAngles = new Vector3(0, 0, 0);
            if (CurrentTileIn.SouthTile != null && !CurrentTileIn.SouthTile.occupied)
            {
                CurrentTileIn = CurrentTileIn.SouthTile;
                transform.position = CurrentTileIn.transform.position;
                audio.PlayOneShot(FootStep[Random.Range(0, 5)]);

            }

        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            direction = Directions.West;
            transform.eulerAngles = new Vector3(0, 0, 270);
            if (CurrentTileIn.WestTile!= null && !CurrentTileIn.WestTile.occupied)
            {
                CurrentTileIn = CurrentTileIn.WestTile;
                transform.position = CurrentTileIn.transform.position;
                audio.PlayOneShot(FootStep[Random.Range(0, 5)]);

            }

        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            direction = Directions.East;
            transform.eulerAngles = new Vector3(0, 0, 90);
            if (CurrentTileIn.EastTile!= null && !CurrentTileIn.EastTile.occupied)
            {
                CurrentTileIn = CurrentTileIn.EastTile;
                transform.position = CurrentTileIn.transform.position;
                audio.PlayOneShot(FootStep[Random.Range(0, 5)]);

            }

        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            switch (direction)
            {
                case Directions.North:
                    if (CurrentTileIn.NorthTile.IsInteractive) {
                        CurrentTileIn.NorthTile.BroadcastMessage("DoInteractive",this);
                    }
                    break;
                case Directions.South:
                    if (CurrentTileIn.SouthTile.IsInteractive)
                    {
                        CurrentTileIn.SouthTile.BroadcastMessage("DoInteractive",this);
                    }
                    break;
                case Directions.East:
                    if (CurrentTileIn.EastTile.IsInteractive)
                    {
                        CurrentTileIn.EastTile.BroadcastMessage("DoInteractive",this);
                    }
                    break;
                case Directions.West:
                    if (CurrentTileIn.WestTile.IsInteractive)
                    {
                        CurrentTileIn.WestTile.BroadcastMessage("DoInteractive",this);
                    }
                    break;

            }
        }
    }
    public void GiveCup()
    {
        CurrentCleanCupTray = null;
        if (CurrentSodaMachine != null)
        {
            audio.PlayOneShot(Mug);
            CurrentSodaMachine.PutCups(Cup);
            this.Cup = null;


        }

        else if (AccessingCounter != null)
        {
            audio.PlayOneShot(Mug);
            AccessingCounter.PutCups(Cup);
            Cup = null;
        }
        else if (CurrentDirtyCupTray != null)
        {
            CurrentDirtyCupTray.PutCups(Cup);
            Cup = null;
        }
    }
    public void TakeCup(Cup cup)
    {
        audio.PlayOneShot(Mug, 0.3f);
        cup.transform.SetParent(gameObject.transform.GetChild(0).transform);
        cup.transform.localEulerAngles = Vector3.zero;
        cup.transform.localPosition = Vector3.zero;
        this.Cup = cup;
       
    }
    
    public void GetSoda()
    {
        if (CurrentCleanCupTray!=null) {
            audio.PlayOneShot(Mug, 0.3f);
            Debug.Log("Did");
            var cup = CurrentCleanCupTray.TakeCup();
            this.Cup = cup;
            cup.transform.SetParent(this.Hand.transform);
            cup.transform.localPosition = Vector3.zero;
            cup.transform.localEulerAngles = Vector3.zero;
       
            CurrentCleanCupTray = null;
            Debug.Log("SettoNull");
        }
        else if(CurrentSodaMachine != null){

            TakeCup(CurrentSodaMachine.TakeCup());
            CurrentSodaMachine = null;
        }
        else
        {
            print("else");
            TakeCup(AccessingCounter.GetDirtyCup());
            AccessingCounter = null;
        }
    }
}
    public enum Directions
    {
        North,
        South,
        East,
        West
    } 

    public enum Cups
{
    None,
    Clean,
    Dirty,
    Red,
    Green,
    Blue
    
}
