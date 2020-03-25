using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum PoliceType { Regular, Rammer, Tanker }

public class PoliceScript : MonoBehaviour
{
    public PoliceType type;

    //ONLY STORES REGULAR POLICE
    public static LinkedList<PoliceScript> regPoliceList = new LinkedList<PoliceScript>();

    PlayerScript play;
    [HideInInspector]public NavMeshAgent agent;
    public float speed;
    Rigidbody rbody;
    Collider col;
    bool isAlive = true;
    [HideInInspector]public bool inPursuit = true;
    
    [HideInInspector] public Vector3 targetPos;

    float randomPerpendicularOffset = 0f;

    AudioTriggerScript audio;

    GameControllerScript game;

    public float shootFrequencyMin, shootFrequencyMax;

    [HideInInspector] public bool playAudio;

    // Start is called before the first frame update
    void Start()
    {
        game = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
        rend = GetComponentInChildren<Renderer>();

        if(type == PoliceType.Regular)
            regPoliceList.AddLast(this);

        game.allPoliceList.Add(this);

        rbody = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
 
        play = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        agent = GetComponent<NavMeshAgent>();
        targetPos = play.transform.position;

        audio = GetComponent<AudioTriggerScript>();

        RandomizeXOffset();

        InvokeRepeating("SlowUpdate", 0f, 0.1f);

        if (playAudio) {
            audio.PlayRandomFromList(0, true);
        }

        if (type == PoliceType.Regular) {
            Invoke("ShootBullet", Random.Range(shootFrequencyMin, shootFrequencyMax));
        }
        if (type == PoliceType.Rammer) {

            Invoke("StartRam", Random.Range(ramTimeFrequencyMin, ramTimeFrequencyMax));

        }
        if (type == PoliceType.Tanker) {
            Invoke("StartMissiles", Random.Range(missileFrequencyMin, missileFrequencyMax));
            game.myMissileAnim = transform.Find("missile").GetComponent<Animation>();
            game.myMissileParticle = game.myMissileAnim.transform.GetComponentInChildren<ParticleSystem>();
            game.generateObstacles = false;
        }
    }

    
    void StartRam(){
        StartCoroutine(Ram());
    }
    
    void Update(){
        if (activated) {

        } else {
            activated = GameControllerScript.differenceAlongDirection(transform.position, play.transform.position, PlayerScript.currentDirection) < 20;
        }
    }

    void SlowUpdate(){
        if (activated) {
            if (inPursuit) {
                Pursuit();
                KeepDistance();
            }
            else {
                if (play.isCrash) {
                    agent.SetDestination(play.transform.position);
                    agent.stoppingDistance = 8f;

                    if (agent.isStopped)
                        agent.enabled = false;
                }
            }
        }       
    }


    public float randomizeOffsetTimeMin, randomizeOffsetTimeMax;

    void RandomizeXOffset(){
        randomPerpendicularOffset = Random.Range(-Mathf.Abs(game.lanePositions[0] - game.lanePositions[game.lanePositions.Length-1]), Mathf.Abs(game.lanePositions[0] - game.lanePositions[game.lanePositions.Length - 1]))/2;        
    }

    float tooFarDuration = 0;

    bool activated = false; //turns on once on screen

    public float speedLimit = 1000;
    void KeepDistance(){
        //Move faster if farther from target
        float interpolation = Mathf.Max(0.01f, Time.deltaTime);

        if (isAlive){
            agent.speed = Vector3.Distance(transform.position, targetPos) * speed;
            if(game.differenceAlongDirection(targetPos, transform.position) > 0){
                agent.speed = Mathf.Lerp(agent.speed, game.differenceAlongDirection(targetPos, transform.position) * speed, interpolation);
                agent.speed = Mathf.Min(speedLimit, game.differenceAlongDirection(targetPos, transform.position) * speed);
            } else {
                agent.speed = 0;
            }
        } else {
            agent.enabled = false;
        }


        if (play.isCrash) {
            inPursuit = false;
        }

        if(type != PoliceType.Tanker) {
            //If the police car is too far for too long, respawn it behind the player in the right direction
            if (Mathf.Abs(game.differenceAlongDirection(transform.position, targetPos)) > 25) {
                tooFarDuration += Time.deltaTime;
            } else {
                tooFarDuration = 0;
            }

            if (tooFarDuration > 2f) {
                //PLACE THE POLICE CAR CLOSER
                transform.position = targetPos - GameControllerScript.dirToVector() * 20f;
                transform.eulerAngles = new Vector3(0, (int)PlayerScript.currentDirection, 0);
                agent.speed = 0;
                isRam = false;
            }
        }

        

    }

    Vector3 ramOffset;

    float ramAmount = 8;

    void Pursuit() {
        if(type == PoliceType.Regular) {
            //Leader chases player, other police trail each other
            if(regPoliceList.First != null){
                if (regPoliceList.First.Value == this)
                    targetPos = play.transform.position;
                else {                    
                    if(regPoliceList.Find(this)!= null)
                        targetPos = regPoliceList.Find(this).Previous.Value.transform.position;
                }
            }
            
        } else if(type == PoliceType.Rammer) {
            if (isRam) {
                
                switch (PlayerScript.currentDirection) {
                    case Direction.North:
                        targetPos = new Vector3(ramOffset.x, 0, play.transform.position.z + ramAmount);
                        break;
                    case Direction.East:
                        targetPos = new Vector3(play.transform.position.x + ramAmount, 0, ramOffset.z);
                        break;
                    case Direction.South:
                        targetPos = new Vector3(ramOffset.x, 0, play.transform.position.z - ramAmount);
                        break;
                    case Direction.West:
                        targetPos = new Vector3(play.transform.position.x - ramAmount, 0, ramOffset.z);
                        break;
                }
                /*
                targetPos = GameControllerScript.addToVectorAlongDirectionRemoveRest(
                    play.transform.position,
                    ramAmount,
                    PlayerScript.currentDirection);
                targetPos += GameControllerScript.addToVectorAlongDirectionRemoveRest(
                    game.latestChunkPosEnd,
                    ramOffset,
                    GameControllerScript.dirAfterTurn(PlayerScript.currentDirection, false)
                    );
                */
            } else {
                targetPos = play.transform.position;
            }
        } else if(type == PoliceType.Tanker) {
            targetPos = play.transform.position + GameControllerScript.dirToVector() * 30;   
        }



        //Give a little bit of offset
        Vector3 location = targetPos;

        if(!isRam)
            location = GameControllerScript.addToVectorAlongDirection(location, randomPerpendicularOffset, GameControllerScript.dirAfterTurn(PlayerScript.currentDirection, false));

        if (agent.isOnNavMesh)
            agent.SetDestination(location);
    }



    #region Rammer
    public float ramTimeFrequencyMin, ramTimeFrequencyMax;
    bool isRam;
    Renderer rend;
    IEnumerator Ram() {

        yield return new WaitUntil(() => rend.isVisible);
        yield return new WaitForSeconds(Random.Range(ramTimeFrequencyMin, ramTimeFrequencyMax));

        if (activated) {
            isRam = true;

            ramOffset = (play.transform.position - transform.position).normalized * 5f;
            switch (PlayerScript.currentDirection) {
                case Direction.North:
                    ramOffset += new Vector3(play.transform.position.x, 0, 0);
                    break;
                case Direction.East:
                    ramOffset += new Vector3(0, 0, play.transform.position.z);
                    break;
                case Direction.South:
                    ramOffset += new Vector3(play.transform.position.x, 0, 0);
                    break;
                case Direction.West:
                    ramOffset += new Vector3(0, 0, play.transform.position.z);
                    break;
            }

            /*
            ramOffset = GameControllerScript.differenceAlongDirection(game.latestChunkPosEnd, play.transform.position - transform.position, 
                GameControllerScript.dirAfterTurn(PlayerScript.currentDirection, false));

            print(ramOffset);
            Debug.Break();
            */
            checkIfSucceed = true;
            StartCoroutine(RamSuccessCountdown());

            yield return new WaitWhile(() => isRam);


        }

        StartCoroutine(Ram());
    }

    bool checkIfSucceed = false;

    IEnumerator RamSuccessCountdown() {        
        yield return new WaitForSeconds(Vector3.Distance(transform.position, targetPos) / 4);
        if (checkIfSucceed){
            OnRamFail();
        }
    }

    IEnumerator PullAwayAfterRam(){
        
        if(game.differenceAlongDirection(play.transform.position, transform.position) < 4) {            
            inPursuit = false;
        }
        agent.updateRotation = false;

        while (game.differenceAlongDirection(play.transform.position, transform.position) < 4) {
            agent.speed = 0;
            yield return new WaitForSeconds(0.1f);
        }
        agent.updateRotation = true;
        inPursuit = true;
    }

    public float ramDamage = 30;
    void OnRamSuccess(){
        isRam = false;
        checkIfSucceed = false;
        StopCoroutine(RamSuccessCountdown());
        play.TakeDamage(ramDamage);
        StartCoroutine(PullAwayAfterRam());
    }

    
    void OnRamFail(){
        isRam = false;
        StartCoroutine(PullAwayAfterRam());
    }

    #endregion

    #region Tanker

    public float missileFrequencyMin = 6, missileFrequencyMax = 10;

    void StartMissiles(){
        if(inPursuit)
            StartCoroutine(FireMissiles());
    }

    public int missilesPerFire = 2;

    IEnumerator FireMissiles() {
        if (activated && inPursuit) {
            game.StartCoroutine(game.LaneMissiles(true, missilesPerFire, -1));        
        }
        yield return new WaitUntil(()=>game.missileFinished);
        yield return new WaitForSeconds(Random.Range(missileFrequencyMin, missileFrequencyMax));
        StartCoroutine(FireMissiles());
    }

    

    public void OnPlayerTurn() {
        if(type == PoliceType.Tanker) {
            SetPosInFrontOfPlayer();
            StartCoroutine(RestrictMissilesDuringTurn());
        }
    }

    void SetPosInFrontOfPlayer() {

        agent.enabled = false;


        transform.position = GameControllerScript.addToVectorAlongDirectionRemoveRest(game.latestChunkPosEnd, 0,  GameControllerScript.dirAfterTurn(PlayerScript.currentDirection, PlayerScript.currentDirection == Direction.East || PlayerScript.currentDirection == Direction.North));
        transform.position += GameControllerScript.addToVectorAlongDirectionRemoveRest(play.transform.position, PlayerScript.currentDirection == Direction.South || PlayerScript.currentDirection == Direction.West ? -75f : 75f, PlayerScript.currentDirection);
        agent.enabled = true;

    }

    IEnumerator RestrictMissilesDuringTurn() {
        game.StartCoroutine(game.RestrictTurn());
        yield return new WaitUntil(()=>game.allowMissiles);
        yield return new WaitForSeconds(Random.Range(missileFrequencyMin, missileFrequencyMax));
        FireMissiles();
    }

    #endregion

    public float explosionForceWhenShot = 1000f;

    public GameObject bulletPrefab;

    public float aimInaccuracy;

    void ShootBullet() {
        if (activated) {
            if (inPursuit) {
                //Spawn Bullet
                BulletScript bullet = Instantiate(bulletPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity).GetComponent<BulletScript>();
                bullet.transform.LookAt(play.transform.position + Vector3.up * 0.5f);
                //give bullet some random random trajectory
                bullet.transform.eulerAngles += Vector3.up * Random.Range(-aimInaccuracy, aimInaccuracy);
                bullet.bulletSpeed *= 1.5f;
                bullet.PoliceTrace();
                bullet.shotFromPolice = true;
            }
        }        
        Invoke("ShootBullet", Random.Range(shootFrequencyMin, shootFrequencyMax));

    }

    void ShotDownFromBullet() {
        CancelInvoke("SlowerUpdate");

        rbody.useGravity = true;
        rbody.drag = 1;
        rbody.angularDrag = 1;
        isAlive = false;

        rbody.AddExplosionForce(explosionForceWhenShot, transform.position + new Vector3(Random.Range(-2.9f, 2.9f), -0.3f, Random.Range(-2.9f, 2.9f)), 3f);

        col.enabled = false;

        DestroyPolice();
    }

    public void DestroyPolice(){
        game.allPoliceList.Remove(this);

        if (type == PoliceType.Regular) {
            regPoliceList.Remove(this);
            GameControllerScript.policeAlive--;
        } else if (type == PoliceType.Rammer) {
            GameControllerScript.rammersAlive--;
        } else if (type == PoliceType.Tanker) {
            game.generateObstacles = true;
            GameControllerScript.tankersAlive--;
        }


        Destroy(gameObject, 3f);
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player"){
            if(isRam)
                OnRamSuccess();
        }
    }
}
