using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HelicopterScript : MonoBehaviour
{

    public enum HeliPhase{ Appearance, Sniper, Minigun, SnipeMiniCombo, Frenzy, Death};

    public HeliPhase phase;

    GameControllerScript game;
    PlayerScript play;

    HealthBarScript health;

    LineRenderer lineRend;

    float originalDistanceFromPlayer;


    Collider col;
    ParticleSystem[] particles;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Pursuit();
        TraceSniper();
        UpdateGatling();
        CheckHealth();
        HeliLoseControl();
        
    }

    public void Initiate() {
        game = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
        play = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        lineRend = GetComponent<LineRenderer>();
        gatling = GetComponentInChildren<GatlingGun>();
        health = GetComponentInChildren<HealthBarScript>();
        originalDistanceFromPlayer = distanceFromPlayer;
        col = GetComponent<Collider>();
        particles = GetComponentsInChildren<ParticleSystem>();


        StartCoroutine(Appear());
        StartCoroutine(GenerateNewRotation());
    }

    public float flyHeight, distanceFromPlayer;
    bool isPursuit;

    Quaternion targetRot;
    Vector3 targetPos;
    void Pursuit(){
        if(isPursuit){
            if(!lostControl)
                targetPos = play.transform.position + Vector3.up * flyHeight + GameControllerScript.dirToVector() * distanceFromPlayer;
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime); 
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime);
        }
    }

    bool lowHealth = false;

    void CheckHealth(){
        if (!lowHealth) {
            if (health.health <= health.fullHealth * 0.4f) {
                lowHealth = true;
            }
        }        
    }

    IEnumerator GenerateNewRotation()
    {
        targetRot = Quaternion.Euler(0, Random.Range(-180f, 180f), 0);
        yield return new WaitForSeconds(5);
        StartCoroutine(GenerateNewRotation());
    }

    bool isSniping;
    Vector3 snipePos;
    public float snipeTraceSpeed;
    const float SNIPE_OFFSET = 6;
    bool snipeTraceHorizontal = true;
    void TraceSniper(){
        if (isSniping) {

            Vector3 before = snipePos;

            snipePos = Vector3.Lerp(snipePos, play.transform.position + GameControllerScript.dirToVector() * SNIPE_OFFSET, Time.deltaTime * snipeTraceSpeed);

            if (!snipeTraceHorizontal) {
                if (PlayerScript.currentDirection == Direction.North || PlayerScript.currentDirection == Direction.South) {
                    snipePos.x = before.x;                                 
                } else {
                    snipePos.z = before.z;
                }
            }

            lineRend.SetPositions(new Vector3[] {transform.position, snipePos});
        }
    }

    

    IEnumerator Appear(){

        phase = HeliPhase.Appearance;
        

        //GET A RANDOM ROTATION
        transform.Rotate(new Vector3(0, Random.Range(-180, 180), 0));

        transform.position = play.transform.position + Vector3.up * flyHeight + GameControllerScript.dirToVector() * distanceFromPlayer;
        isRevealed = true;

        isPursuit = true;

        yield return new WaitForSeconds(3f);

        StartCoroutine(Sniper(true, false, 1f));
    }


    public int snipeRounds = 5;

    float revealDuration = 4;
    
    IEnumerator Sniper(bool isIndependent, bool isFrenzy, float blinkDuration) {
        if(isIndependent)
            phase = HeliPhase.Sniper;

        lineRend.enabled = true;
        isSniping = true;

        for (int i = 0; i < snipeRounds; i++) {
            //Fly off screen
            if (isIndependent || isFrenzy) {
                isRevealed = false;
                lineRend.enabled = true;
                distanceFromPlayer = originalDistanceFromPlayer + 20;
            }

            //Shoot 5 bullets, each faster and faster
            for (int j = 0; j < 5; j++) {
                float takeAimDuration;
                if (isFrenzy) {
                    takeAimDuration = 1f;
                } else {
                    takeAimDuration = (6 - j) * 0.5f;
                }
                yield return new WaitForSeconds(takeAimDuration);
                StartCoroutine(WarnAndShootAndRevealSniper(isIndependent, blinkDuration));
                yield return new WaitWhile(()=>isWarningSnipe);
                lineRend.enabled = true;
                if (lowHealth && !isFrenzy)
                    break;
            }            

            //Reveal
            if (isIndependent || isFrenzy) {
                isRevealed = true;
                distanceFromPlayer = originalDistanceFromPlayer;
                lineRend.enabled = false;
                yield return new WaitForSeconds(revealDuration);
            }
        }

        isSniping = false;
        lineRend.enabled = false;


        if(isIndependent)
            StartCoroutine(Gatling(true));
    }

    bool isRevealed = true;
    bool isWarningSnipe = false;

    IEnumerator WarnAndShootAndRevealSniper(bool isIndependent, float blinkDuration){
        snipeTraceHorizontal = false;

        isWarningSnipe = true;
        //Blinking animation
        bool on = true;
        int blinkCount = 16;
        for(int i = 0; i < blinkCount; i++) {
            lineRend.enabled = on;
            on = !on;
            yield return new WaitForSeconds(blinkDuration / blinkCount);
        }


        //Shoot
        ShootAt(snipePos);

        lineRend.enabled = false;
        snipeTraceHorizontal = false;

        yield return new WaitForSeconds(0.5f);

        isWarningSnipe = false;
        snipeTraceHorizontal = true;


    }

    GatlingGun gatling;

    public int gatlingRounds = 5;
    bool isGatling = false;
    int gatlingShootLaneIndex;
    Direction dirWhenGatStart;

    IEnumerator Gatling(bool isIndepedent){
        if(isIndepedent)
            phase = HeliPhase.Minigun;
        isGatling = true;
        dirWhenGatStart = PlayerScript.currentDirection;

        for (int i = 0; i < gatlingRounds; i++) {
            game.StartCoroutine(game.LaneMissiles(false, 1, -1));
            yield return new WaitForSeconds(1.5f);
            gatling.fire = true;
            gatling.aim = false;

            gatlingShootLaneIndex = game.missileLaneIndexes[0];

            yield return new WaitUntil(() => game.missileFinished);
            gatling.fire = false;
            gatling.aim = true;            
            yield return new WaitForSeconds(1.5f);
            if (lowHealth)
                break;
        }

        isGatling = false;

        if(isIndepedent)
            StartCoroutine(SnipeAndGat());
    }

    IEnumerator SnipeAndGat()
    {        
        phase = HeliPhase.SnipeMiniCombo;

        while (!lowHealth) {

            if(!isSniping)
                StartCoroutine(Sniper(false, false, 1f));
            if(!isGatling)
                StartCoroutine(Gatling(false));

            yield return new WaitWhile(() => isSniping && isGatling);
        }

        StartCoroutine(SnipeFrenzy());
    }

    IEnumerator SnipeFrenzy (){
        phase = HeliPhase.Frenzy;
        while (isPursuit) {
            StartCoroutine(Sniper(false, true, 0.5f));
            yield return new WaitWhile(() => isSniping);
        }        
    }


    void UpdateGatling(){
        if (isGatling) {
            gatling.aim = true;
            if(game.missileIndicators.Count > 0) {
                gatling.go_target = Vector3.Lerp(gatling.go_target, game.missileIndicators[0].transform.position + GameControllerScript.dirToVector() * 5, Time.deltaTime * 5);
            }
        }
        if (gatling.fire) {
            //Check if Player is under Gatling fire
            if (play.myClosestLaneIndex == gatlingShootLaneIndex && PlayerScript.currentDirection == dirWhenGatStart) {
                play.speed = play.slowSpeed;
                play.TakeDamage(0.5f);
            }
            else
            {
                play.speed = play.originalSpeed;
            }
        }
        else
        {
            play.speed = play.originalSpeed;
        }

    }

    void ShootAt(Vector3 pos){

        int layerMask = 1 << 8;

        RaycastHit hit;

        if (Physics.Raycast(pos + Vector3.up * 10, Vector3.down, out hit, Vector3.Distance(transform.position, pos), layerMask)){

            if (hit.transform.gameObject.tag == "Player") {
                SnipedPlayer();
            }
        }
    }

    void SnipedPlayer(){
        play.GameOver();
    }

    void ShotDownFromBullet(){
        StopAllCoroutines();
        StartCoroutine(Crash());
    }

    bool lostControl = false;
    bool isFalling = false;
    void HeliLoseControl(){
        if (lostControl) {
            targetRot *= Quaternion.Euler(Vector3.up * 128 * Time.deltaTime);
            targetRot *= Quaternion.Euler(Vector3.right * 16 * Time.deltaTime);
            if(game.missileIndicators.Count != 0)
                targetPos = game.missileIndicators[1].transform.position + GameControllerScript.dirToVector() * distanceFromPlayer;
        }        
    }

    IEnumerator Crash(){
        phase = HeliPhase.Death;

        Direction dirWhenCrashStart = PlayerScript.currentDirection;

        col.enabled = false;
        particles[4].Play();
        particles[5].Play();

        int fallLaneIndex = Random.Range(0, game.lanePositions.Length-1);
        game.StartCoroutine(game.LaneMissiles(false, 1, fallLaneIndex));
        game.StartCoroutine(game.LaneMissiles(false, 1, fallLaneIndex+1));

        lostControl = true;
        col.isTrigger = true;

        yield return new WaitForSeconds(1f);

        distanceFromPlayer = originalDistanceFromPlayer;

        yield return new WaitUntil(()=>game.missileFinished);
        particles[4].Play();

        lostControl = false;
        isPursuit = false;

        if(PlayerScript.currentDirection == dirWhenCrashStart) {
            if (play.myClosestLaneIndex == fallLaneIndex || play.myClosestLaneIndex == fallLaneIndex + 1) {
                play.GameOver();
            }
        }

        GameControllerScript.waveStatus = WaveStatus.Intermission;
        game.PlayExplosionSound();
        Destroy(gameObject, 3f);
    }
}
