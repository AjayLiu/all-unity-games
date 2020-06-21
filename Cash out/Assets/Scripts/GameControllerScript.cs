using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public enum WaveStatus {
    Intermission, Incoming, Assault, Boss
}

public class GameControllerScript : MonoBehaviour
{

    public static WaveStatus waveStatus = WaveStatus.Intermission;

    public bool isBoss = false;

    NavMeshSurface surface;

    public static int score = 0;

    PlayerScript play;

    public float chunkThreshold; //How many percent does the player travel into the current chunk until the controller generates a new one?
    [HideInInspector] public Vector3 latestChunkPosEnd = Vector3.zero; //pos of the end of the latest generated chunk
    Vector3 latestChunkPosStart = Vector3.zero; // pos of the start of the latest generated chunk
    public float chunkReuseDistance;

    public float graceChunkLength;

    void Awake()
    {
        ResetStatics();
    }

    public Sprite intermissionSprite, incomingSprite, assaultSprite, bossSprite;

    AudioSource explosionAudio;

    // Start is called before the first frame update
    void Start()
    {
        //Time.timeScale = 0;
        UpdateWaveStatusImage();
        play = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        camScript = Camera.main.GetComponent<FollowPlayer>();
        surface = GetComponent<NavMeshSurface>();
        miniGameReference.SetActive(false);
        miniGamePointer = miniGameReference.transform.GetChild(0).GetChild(1).GetComponent<Image>();
        targetArc = miniGameReference.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        midPoint = targetArc.transform.GetChild(0);
        LoadWeaponsList();
        subtitlesText.gameObject.SetActive(false);
        TranscribleSoundFXListToDict();
        explosionAudio = GetComponent<AudioSource>();
        //Spawn Roads
        for (int i = -20; i < graceChunkLength; i += 10)
            SpawnRoad(new Vector3(0, 0, i));
        latestChunkPosEnd = dirToVector() * graceChunkLength;

        lanePositionsStatic = lanePositions;
    }

    
    // Update is called once per frame
    void Update()
    {
        //If the player is halfway to the edge of the world, generate a new chunk ahead
        if(differenceAlongDirection(latestChunkPosEnd, play.transform.position) < chunkThreshold){
            GenerateChunk(obstaclesPerChunk, pedestriansPerChunk);
        }

        if (!missileFinished) {
            DuringMissileWarningUpdate();
        }

        if (Input.GetKeyDown(KeyCode.Space))
            Restart();

        UpdateScore();

        if (isInMinigame)
            DuringMiniGame();

    }

 

    public GameObject[] obstaclePrefabs;
    public GameObject policePrefab, rammerPrefab, tankerPrefab, helicopterPrefab;

    public int obstaclesPerChunk;
    public int pedestriansPerChunk;
    public float[] lanePositions;
    public static float[] lanePositionsStatic;
    public bool isTwoWay = true;

    public float gapBetweenObstacleMin, gapBetweenObstacleMax;

    
    public LinkedList<ObstacleScript> obstaclesList = new LinkedList<ObstacleScript>(); //first is newest, last is oldest
    LinkedList<GameObject> roadList = new LinkedList<GameObject>(); //first is newest, last is oldest
    LinkedList<GameObject> intersectionList = new LinkedList<GameObject>(); //first is newest, last is oldest
    

    public GameObject roadPrefab;
    public GameObject intersectionPrefab;
    public GameObject[] buildingPrefabs;

    public GameObject[] pedestrianPrefabs;


    public bool isWaveSystem = true;

    public int policePerWave;
    public int rammersPerWave;
    public int tankersPerWave;
   
    public int intermissionChunkAmount = 3;
    int intermissionChunkCounter = -3;
    public int incomingChunkAmount = 2;
    int incomingChunkCounter = 0;
    public int assaultChunkAmountMin = 2;
    int assaultChunkCounter = 0;

    void GenerateChunk(int obstaclesToSpawn, int pedestriansToSpawn){


        bool spawnPolice = !isBoss;

        //Update wave status
        if (isWaveSystem) {
            switch (waveStatus) {
                case WaveStatus.Intermission:
                    intermissionChunkCounter++;

                    //Less obstacles
                    generateObstacles = true;
                    spawnPedestrians = true;
                    obstaclesToSpawn /= 3;
                    pedestriansToSpawn /= 3;

                    //Dont spawn police
                    spawnPolice = false;

                    if (intermissionChunkCounter > intermissionChunkAmount) {
                        waveStatus = WaveStatus.Incoming;
                        intermissionChunkCounter = 0;
                    }
                    break;
                case WaveStatus.Incoming:
                    incomingChunkCounter++;

                    //Dont spawn police
                    spawnPolice = false;

                    if (incomingChunkCounter > incomingChunkAmount) {
                        waveStatus = WaveStatus.Assault;
                        incomingChunkCounter = 0;
                    }

                    break;
                case WaveStatus.Assault:

                    if(assaultChunkCounter == 0) {
                        if(PlayerScript.currentDirection == Direction.West || PlayerScript.currentDirection == Direction.South) {
                            SpawnPolice(tankerPrefab, tankersPerWave, addToVectorAlongDirectionRemoveRest(play.transform.position, -30, PlayerScript.currentDirection) + addToVectorAlongDirectionRemoveRest(latestChunkPosEnd, 0, dirAfterTurn(PlayerScript.currentDirection, false)));
                            SpawnPolice(rammerPrefab, rammersPerWave, addToVectorAlongDirectionRemoveRest(play.transform.position, 30, PlayerScript.currentDirection) + addToVectorAlongDirectionRemoveRest(latestChunkPosEnd, 0, dirAfterTurn(PlayerScript.currentDirection, false)));
                            SpawnPolice(policePrefab, policePerWave, addToVectorAlongDirectionRemoveRest(play.transform.position, 30, PlayerScript.currentDirection) + addToVectorAlongDirectionRemoveRest(latestChunkPosEnd, 0, dirAfterTurn(PlayerScript.currentDirection, false)));
                        } else {
                            SpawnPolice(tankerPrefab, tankersPerWave, addToVectorAlongDirectionRemoveRest(play.transform.position, 30, PlayerScript.currentDirection) + addToVectorAlongDirectionRemoveRest(latestChunkPosEnd, 0, dirAfterTurn(PlayerScript.currentDirection, false)));
                            SpawnPolice(rammerPrefab, rammersPerWave, addToVectorAlongDirectionRemoveRest(play.transform.position, -30, PlayerScript.currentDirection) + addToVectorAlongDirectionRemoveRest(latestChunkPosEnd, 0, dirAfterTurn(PlayerScript.currentDirection, false)));
                            SpawnPolice(policePrefab, policePerWave, addToVectorAlongDirectionRemoveRest(play.transform.position, -30, PlayerScript.currentDirection) + addToVectorAlongDirectionRemoveRest(latestChunkPosEnd, 0, dirAfterTurn(PlayerScript.currentDirection, false)));
                        }
                    }

                    assaultChunkCounter++;
                    if (assaultChunkCounter > assaultChunkAmountMin) {
                        RetreatPoliceIfNeeded();
                    }
                    break;
                case WaveStatus.Boss:
                    generateObstacles = false;
                    spawnPedestrians = false;
                    spawnPolice = false;
                    break;
            }
            UpdateWaveStatusImage();
        }


        Vector3 spawnerPos = latestChunkPosEnd; //keeps track how far chunk has already generated

        Vector3 newestRoadPos = latestChunkPosEnd; // where was the newest road generated?

        int openLaneIndex = Random.Range(1, lanePositions.Length - 1);

        //Spawn Obstacles
        for (int i = 0; i < obstaclesToSpawn; i++){

            //Spawns road if needed             
            while (differenceAlongDirection(newestRoadPos, spawnerPos) < 10) {
                SpawnRoad(newestRoadPos);
                newestRoadPos += dirToVector() * 10;
            }

            spawnerPos += dirToVector() * Random.Range(gapBetweenObstacleMin, gapBetweenObstacleMax);


            if (generateObstacles) {
                //Picks a random spawn location
                //but leave a lane open

                int randomSpawnPosIndex = RandomRangeExcept(1, lanePositions.Length - 1, openLaneIndex);

                Vector3 spawnPos = spawnerPos + dirToVector(dirAfterTurn(PlayerScript.currentDirection, true)) * lanePositions[randomSpawnPosIndex];

                //Flip rotation if 2 way
                Direction spawnDir = PlayerScript.currentDirection;
                if (isTwoWay) {
                    if (randomSpawnPosIndex > lanePositions.Length / 2 - 1) {
                        spawnDir = dirAfterTurn(dirAfterTurn(PlayerScript.currentDirection, false), false);
                    }
                }

                //Spawns obstacle               
                SpawnObstacle(spawnPos, spawnDir);
            }

            //Spawn pedestrians
            if (spawnPedestrians) {
                for (int j = 0; j < pedestriansToSpawn / obstaclesToSpawn; j++) {
                    SpawnPedestrian(pedestrianPrefabs[Random.Range(0, pedestrianPrefabs.Length)], spawnerPos);
                }
            }
            
            
        }


        //Fill in gaps in between chunks with road (because roads are in units of 10, not scalable)        
        while (differenceAlongDirection(newestRoadPos, spawnerPos) < 10) {
            SpawnRoad(newestRoadPos);
            newestRoadPos += dirToVector() * 10;
        }


        //Spawn an intersection
        if (intersectionPercentage >= 1f) {
            newestRoadPos += dirToVector() * 5;
            SpawnIntersection(newestRoadPos);
            newestRoadPos += dirToVector() * 15;
            intersectionPercentage--;
        }

        //Spawn police 
        if (spawnPolice) {
            if (PlayerScript.currentDirection == Direction.West || PlayerScript.currentDirection == Direction.South) {
                SpawnPolice(tankerPrefab, tankersToSpawn, addToVectorAlongDirectionRemoveRest(play.transform.position, -30, PlayerScript.currentDirection) + addToVectorAlongDirectionRemoveRest(latestChunkPosEnd, 0, dirAfterTurn(PlayerScript.currentDirection, false)));
                SpawnPolice(rammerPrefab, rammersToSpawn, addToVectorAlongDirectionRemoveRest(play.transform.position, 30, PlayerScript.currentDirection) + addToVectorAlongDirectionRemoveRest(latestChunkPosEnd, 0, dirAfterTurn(PlayerScript.currentDirection, false)));
                SpawnPolice(policePrefab, policeToSpawn, addToVectorAlongDirectionRemoveRest(play.transform.position, 30, PlayerScript.currentDirection) + addToVectorAlongDirectionRemoveRest(latestChunkPosEnd, 0, dirAfterTurn(PlayerScript.currentDirection, false)));
            } else {
                SpawnPolice(tankerPrefab, tankersToSpawn, addToVectorAlongDirectionRemoveRest(play.transform.position, 30, PlayerScript.currentDirection) + addToVectorAlongDirectionRemoveRest(latestChunkPosEnd, 0, dirAfterTurn(PlayerScript.currentDirection, false)));
                SpawnPolice(rammerPrefab, rammersToSpawn, addToVectorAlongDirectionRemoveRest(play.transform.position, -30, PlayerScript.currentDirection) + addToVectorAlongDirectionRemoveRest(latestChunkPosEnd, 0, dirAfterTurn(PlayerScript.currentDirection, false)));
                SpawnPolice(policePrefab, policeToSpawn, addToVectorAlongDirectionRemoveRest(play.transform.position, -30, PlayerScript.currentDirection) + addToVectorAlongDirectionRemoveRest(latestChunkPosEnd, 0, dirAfterTurn(PlayerScript.currentDirection, false)));
            }
        }
        

        //Spawn a trigger
        Instantiate(triggerPrefab, newestRoadPos, Quaternion.identity);


        latestChunkPosStart = latestChunkPosEnd;
        latestChunkPosEnd = newestRoadPos;


        
        //Update NavMesh
        surface.BuildNavMesh();
    }


    public static int policeAlive, rammersAlive, tankersAlive;
    public int maxPolice, maxRammers, maxTankers;
    void SpawnPolice(GameObject prefab, float amount, Vector3 spawnPos) {
        for(int i = 0; i < (int)amount; i++) {
            bool playAudio = false;
            PoliceType type = prefab.GetComponent<PoliceScript>().type;
            switch (type) {
                case PoliceType.Regular:
                    if (policeAlive >= maxPolice)
                        return;
                    policeAlive++;
                    if(policeAlive == 1 && tankersAlive == 0) {
                        playAudio = true;
                    }
                    policeToSpawn--;
                    break;
                case PoliceType.Rammer:
                    if (rammersAlive >= maxRammers)
                        return;
                    rammersAlive++;
                    if (rammersAlive == 1 && tankersAlive == 0) {
                        playAudio = true;
                    }
                    rammersToSpawn--;
                    break;

                case PoliceType.Tanker:
                    if (tankersAlive >= maxTankers)
                        return;
                    tankersAlive++;
                    if (tankersAlive == 1) {
                        playAudio = true;
                    }
                    tankersToSpawn--;
                    break;
            }

            //randomize the spawn a little bit to avoid stacking
            spawnPos += Vector3.forward * Random.Range(-roadWidth, roadWidth) * 0.8f + Vector3.right * Random.Range(-roadWidth, roadWidth) * 0.8f;
            GameObject newPolice = Instantiate(prefab, spawnPos, Quaternion.Euler(0, (int)PlayerScript.currentDirection, 0));
            if (playAudio) {
                newPolice.GetComponent<PoliceScript>().playAudio = true;
            }
        }
    }

    public bool generateObstacles = true;

    void SpawnObstacle(Vector3 pos, Direction dir){
        if (obstaclesList.Count > 0) {
            LinkedListNode<ObstacleScript> oldest = obstaclesList.Last;
            do {
                ObstacleScript oldestObstacle = oldest.Value;
                if (!oldestObstacle.isCrash && (Mathf.Abs(differenceAlongDirection(play.transform.position, oldestObstacle.transform.position, dirAfterTurn(PlayerScript.currentDirection, true))) > chunkReuseDistance || differenceAlongDirection(play.transform.position, oldestObstacle.transform.position) > chunkReuseDistance)) {
                    HealthBarScript oldHealth = oldestObstacle.GetComponent<HealthBarScript>();
                    oldHealth.health = 100f;
                    oldHealth.healthBar.parent.gameObject.SetActive(false);
                    oldHealth.UpdateHealthBar();
                    oldHealth.transform.position = pos;
                    oldestObstacle.transform.eulerAngles = new Vector3(0, (int)dir, 0);
                    oldestObstacle.ResetDirection();
                    obstaclesList.Remove(oldest);
                    obstaclesList.AddFirst(oldestObstacle);
                    return;
                }
                oldest = oldest.Previous;
            } while (oldest != null);
        }
        
        //or else, just spawn a new obstacle
        int obstacleType = Random.Range(0, obstaclePrefabs.Length);
        GameObject obstacle = Instantiate(obstaclePrefabs[obstacleType], pos, Quaternion.identity);
        obstacle.transform.eulerAngles = new Vector3(0, (int)dir, 0);
        ObstacleScript ob = obstacle.GetComponent<ObstacleScript>();

        obstaclesList.AddFirst(ob);
    }

    void SpawnRoad(Vector3 pos){
        //Try to reuse old roads
        if (roadList.Count > 0){
            LinkedListNode<GameObject> oldest = roadList.Last;
            do {
                GameObject oldestRoad = oldest.Value;
                if (Mathf.Abs(differenceAlongDirection(play.transform.position, oldestRoad.transform.position, dirAfterTurn(PlayerScript.currentDirection,true))) > chunkReuseDistance || differenceAlongDirection(play.transform.position, oldestRoad.transform.position) > chunkReuseDistance) {
                    oldestRoad.transform.position = pos;
                    oldestRoad.transform.eulerAngles = new Vector3(0, (int)PlayerScript.currentDirection, 0);
                    roadList.Remove(oldest);
                    roadList.AddFirst(oldestRoad);
                    return;
                }
                oldest = oldest.Previous;                
            } while (oldest != null);
        }
            
            
        

        //If not, just make a new one
        GameObject road = Instantiate(roadPrefab, pos, Quaternion.identity);


        //Add Buildings to the sides

        //Right side
        Instantiate(buildingPrefabs[Random.Range(0, buildingPrefabs.Length)], road.transform);

        //Left side
        GameObject building = Instantiate(buildingPrefabs[Random.Range(0, buildingPrefabs.Length)],road.transform);
        building.transform.eulerAngles += new Vector3(0, 180, 0);
        building.transform.localPosition = new Vector3(-building.transform.localPosition.x, 0f, -building.transform.localPosition.z);

        road.transform.eulerAngles = new Vector3(0, (int)PlayerScript.currentDirection, 0);

        roadList.AddFirst(road);
        

    }

    void SpawnIntersection(Vector3 pos){
        //Try to reuse old intersections
        if (intersectionList.Count > 0) {
            LinkedListNode<GameObject> oldest = intersectionList.Last;
            do {
                GameObject oldestIntersection = oldest.Value;
                if (Mathf.Abs(differenceAlongDirection(play.transform.position, oldestIntersection.transform.position, dirAfterTurn(PlayerScript.currentDirection, true))) > chunkReuseDistance || differenceAlongDirection(play.transform.position, oldestIntersection.transform.position) > chunkReuseDistance) {
                    oldestIntersection.transform.position = pos;
                    oldestIntersection.transform.eulerAngles = new Vector3(0, (int)PlayerScript.currentDirection, 0);
                    intersectionList.Remove(oldest);
                    intersectionList.AddFirst(oldestIntersection);
                    return;
                }
                oldest = oldest.Previous;
            } while (oldest != null);
        }       

        //If not, just make a new one
        GameObject intersection = Instantiate(intersectionPrefab, pos, Quaternion.identity);
        intersection.transform.eulerAngles = new Vector3(0, (int)PlayerScript.currentDirection, 0);
        intersectionList.AddFirst(intersection);
    }

    FollowPlayer camScript;

    public void IntersectionTurn(bool isLeft, Vector3 pos) {
        play.OnIntersectionExit(isLeft);
        latestChunkPosEnd = new Vector3(pos.x, 0, pos.z) + dirToVector() * 18;
        camScript.OnPlayerTurn();
        foreach (PoliceScript police in allPoliceList) {
            police.OnPlayerTurn();
        }
        StartCoroutine(RestrictTurn());


    }

    public float roadWidth = 5.5f;

    public bool spawnPedestrians = true;

    LinkedList<PedestrianScript> pedestrianList = new LinkedList<PedestrianScript>();
    void SpawnPedestrian(GameObject prefab, Vector3 spawnPos) {


        spawnPos = addToVectorAlongDirection(spawnPos, Random.Range(0, 2) == 0 ? roadWidth : -roadWidth, dirAfterTurn(PlayerScript.currentDirection, false));
        Direction dir = Random.Range(0, 2) == 0 ? PlayerScript.currentDirection : dirAfterTurn(dirAfterTurn(PlayerScript.currentDirection, true), true);

        //Try to reuse old pedestrians
        if (pedestrianList.Count > 0) {
            LinkedListNode<PedestrianScript> oldest = pedestrianList.Last;
            do {
                PedestrianScript oldestPed = oldest.Value;
                if (oldestPed.isAlive){
                    if (Mathf.Abs(differenceAlongDirection(play.transform.position, oldestPed.transform.position, dirAfterTurn(PlayerScript.currentDirection, true))) > chunkReuseDistance || differenceAlongDirection(play.transform.position, oldestPed.transform.position) > chunkReuseDistance) {
                        oldestPed.transform.position = spawnPos;
                        oldestPed.transform.eulerAngles = new Vector3(0, (int)dir, 0);
                        pedestrianList.Remove(oldest);
                        pedestrianList.AddFirst(oldestPed);
                        return;
                    }
                } 
                oldest = oldest.Previous;
            } while (oldest != null);
        }

        //If not, just make a new one
        PedestrianScript newPed = Instantiate(prefab, spawnPos, Quaternion.Euler(0, (int)dir, 0)).GetComponent<PedestrianScript>();
        pedestrianList.AddFirst(newPed);
    }

    void RetreatPoliceIfNeeded() {
        if(policeAlive <= 1 && rammersAlive <= 1 && tankersAlive == 0) {
            waveStatus = WaveStatus.Intermission;
            assaultChunkCounter = 0;
            DisableAllPolice();                
        }
    }

    void DisableAllPolice(){
        while (allPoliceList.Count > 0) {
            allPoliceList[0].inPursuit = false;
            allPoliceList[0].StopAllCoroutines();
            allPoliceList[0].DestroyPolice();
        }
    }


    public void SpawnHelicopter() {
        HelicopterScript heli = Instantiate(helicopterPrefab, Vector3.zero, Quaternion.identity).GetComponent<HelicopterScript>();

        waveStatus = WaveStatus.Boss;
        UpdateWaveStatusImage();
        
        //DESPAWN ALL OBSTACLES AND POLICE
        isBoss = true;
        spawnPedestrians = false;
        generateObstacles = false;

        DisableAllPolice();

        heli.Initiate();
    }

    public static Vector3 dirToVector() {
        switch (PlayerScript.currentDirection) {
            case Direction.North:
                return Vector3.forward;

            case Direction.East:
                return Vector3.right;

            case Direction.South:
                return Vector3.back;

            case Direction.West:
                return Vector3.left;

        }
        return Vector3.zero;
    }
    public static Vector3 dirToVector(Direction dir) {
        switch (dir) {
            case Direction.North:
                return Vector3.forward;

            case Direction.East:
                return Vector3.right;

            case Direction.South:
                return Vector3.back;

            case Direction.West:
                return Vector3.left;               
        }
        return Vector3.zero;
    }
    public float differenceAlongDirection(Vector3 a, Vector3 b) {
        switch (PlayerScript.currentDirection) {
            case Direction.North:
                return a.z - b.z;

            case Direction.East:
                return a.x - b.x;

            case Direction.South:
                return b.z - a.z;

            case Direction.West:
                return b.x - a.x;
        }
        return 0;
    }

    public static float differenceAlongDirection(Vector3 a, Vector3 b, Direction dir) {
        switch (dir) {
            case Direction.North:
                return a.z - b.z;

            case Direction.East:
                return a.x - b.x;

            case Direction.South:
                return b.z - a.z;

            case Direction.West:
                return b.x - a.x;
        }
        return 0;
    }

    
    public static Vector3 addToVectorAlongDirection(Vector3 v, float f, Direction dir) {
        switch (dir) {
            case Direction.North:
                return v + Vector3.forward * f;

            case Direction.East:
                return v + Vector3.right * f;

            case Direction.South:
                return v + Vector3.forward * f;

            case Direction.West:
                return v + Vector3.right * f;

        }
        return Vector3.zero;
    }

    public static Vector3 addToVectorAlongDirectionRemoveRest(Vector3 v, float f, Direction dir) {
        switch (dir) {
            case Direction.North:
                return new Vector3(0,0,v.z + f);

            case Direction.East:
                return new Vector3(v.x + f, 0, 0);

            case Direction.South:
                return new Vector3(0, 0, v.z + f);

            case Direction.West:
                return new Vector3(v.x + f, 0, 0);

        }
        return Vector3.zero;
    }

    //    North = 0, East = 90, South = 180, West = 270

    public static Direction dirAfterTurn(Direction dir, bool isLeft){
        if (isLeft) {
            //TURN LEFT
            if ((int)dir - 90 < 0)
                return Direction.West;
            else
                return (Direction)((int)dir - 90);
        } else {
            //TURN RIGHT
            if ((int)dir + 90 > 270)
                return Direction.North;
            else
                return (Direction)((int)dir + 90);
        }
    }

    

    public Text scoreText;

    public int scoreMultiplier = 1000;
    void UpdateScore(){
        score = Mathf.Max(score, Mathf.RoundToInt(Time.timeSinceLevelLoad) * scoreMultiplier);
        scoreText.text = score.ToString();
    }

    public Image waveStatusImage;
    public void UpdateWaveStatusImage(){
        switch (waveStatus) {
            case WaveStatus.Intermission:
                waveStatusImage.sprite = intermissionSprite;
                break;
            case WaveStatus.Incoming:
                waveStatusImage.sprite = incomingSprite;
                break;
            case WaveStatus.Assault:
                waveStatusImage.sprite = assaultSprite;
                break;
            case WaveStatus.Boss:
                waveStatusImage.sprite = bossSprite;
                break;
        }
    }

    public void Restart(){
        ResetStatics();
        SceneManager.LoadScene(0);
    }

    [HideInInspector]public List<PoliceScript> allPoliceList = new List<PoliceScript>();

    void ResetStatics(){
        score = 0;
        waveStatus = WaveStatus.Intermission;
        triggerCount = 0;
        policeAlive = rammersAlive = tankersAlive = 0;
        intersectionPercentage = policeToSpawn = tankersToSpawn = rammersToSpawn = 0;
        PlayerScript.currentDirection = Direction.North;
        allPoliceList.Clear();
        PoliceScript.regPoliceList.Clear();
        AudioTriggerScript.playOrder.Clear();
        PedestrianScript.allowPedestrianWarning = true;
        StopAllCoroutines();
}

    public static float ClosestLanePos(float n){
        float closest = 100f;
        float result = 0;
        foreach (float lane in lanePositionsStatic) {
            if (Mathf.Abs(n - lane) < closest) {
                result = lane;
                closest = Mathf.Abs(n - lane);
            }
        }
        return result;
    }

    int RandomRangeExcept(int min, int max, int except) {
        int r = Random.Range(min, max - 1);
        if (r >= except)
            r ++;
        return r;
        
    }

    public GameObject gunBackgroundReference;
    public List<GameObject> weaponPrefabList  = new List<GameObject>();
    [HideInInspector] public List<WeaponInfoScript> weaponList = new List<WeaponInfoScript>();
    [HideInInspector]public WeaponInfoScript currentWeap;

    public int currentWeaponIndex = 0;

    void LoadWeaponsList() {
        foreach(GameObject g in weaponPrefabList) {
            GameObject weapon = Instantiate(g, gunBackgroundReference.transform);
            weapon.transform.SetSiblingIndex(0);
            weaponList.Add(weapon.GetComponent<WeaponInfoScript>());
            weapon.SetActive(false);
        }

        currentWeap = weaponList[currentWeaponIndex];

        ResetWeaponsList();
    }

    public void ResetWeaponsList(){
        //save bullets remaining in previous weapon
        currentWeap.bulletsInMyChamber = play.bulletsLeft;

        currentWeap = weaponList[currentWeaponIndex];


        play.bulletMaxCapacity = currentWeap.clipSize;
        play.bulletsLeft = currentWeap.bulletsInMyChamber;
        RefreshWeaponDisplay();
    }

    Coroutine currentWeapDisplayCoroutine;

    public Text weaponText;

    public void RefreshWeaponDisplay(){
        if (currentWeapDisplayCoroutine != null)
            StopCoroutine(currentWeapDisplayCoroutine);
        currentWeapDisplayCoroutine = StartCoroutine(DisplayCurrentWeapon());
    }

    float weaponDisplayTime = 2;
    IEnumerator DisplayCurrentWeapon() {
        foreach(WeaponInfoScript w in weaponList) {
            w.gameObject.SetActive(false);
        }
        play.ammoImage.sprite = currentWeap.ammoSprite;
        currentWeap.gameObject.SetActive(true);

        play.ammoCounterText.gameObject.SetActive(true);
        weaponText.gameObject.SetActive(true);
        weaponText.text = currentWeap.weapName;

        yield return new WaitForSeconds(weaponDisplayTime);

        play.ammoCounterText.gameObject.SetActive(false);
        weaponText.gameObject.SetActive(false);


    }

    public GameObject triggerPrefab;
    static int triggerCount;
    public static float intersectionPercentage = 0;
    public static float policeToSpawn = 2;
    public static float rammersToSpawn = 1;
    public static float tankersToSpawn = 1;


    public static void TriggerHit(TriggerInfo trigger){
        triggerCount++;
        intersectionPercentage += trigger.intersectionPercentage;
        if(waveStatus == WaveStatus.Assault) {
            rammersToSpawn += trigger.rammersToSpawn;
            policeToSpawn += trigger.policeToSpawn;
            tankersToSpawn += trigger.tankersToSpawn;
        }
        
    }


    public static void PedestrianKilled() {

    }


    [HideInInspector] public Animation myMissileAnim;
    [HideInInspector] public ParticleSystem myMissileParticle;
    public GameObject missileIndicatorPrefab;
    bool isWarning = false;
    [HideInInspector] public List<GameObject> missileIndicators = new List<GameObject>();
    [HideInInspector] public List<int> missileLaneIndexes = new List<int>();
    Direction directionWhenMissileInitiated;
    [HideInInspector] public bool allowMissiles = true;

    //pass in -1 for laneIndex for random
    public IEnumerator LaneMissiles(bool isMissile, int numMissiles, int laneIndex){
        if(myMissileAnim != null) {
            myMissileAnim.Play();
            myMissileParticle.Play();
            myMissileAnim.transform.position = transform.position;
        }
        directionWhenMissileInitiated = PlayerScript.currentDirection;
        for (int i = 0; i < numMissiles; i++) {
            //Choose lane index to spawn
            int lane = laneIndex;
            if (laneIndex == -1) {
                if (missileIndicators.Count >= lanePositions.Length) {
                    Debug.LogError("TOO MANY MISSILES");
                    break;
                }
                do {
                    lane = Random.Range(0, lanePositions.Length);
                } while (missileLaneIndexes.Contains(lane));
            }
            missileLaneIndexes.Add(lane);

            Vector3 spawnPos = addToVectorAlongDirectionRemoveRest(play.transform.position, 0, PlayerScript.currentDirection);

            spawnPos = addToVectorAlongDirection(spawnPos, lanePositions[lane],dirAfterTurn(PlayerScript.currentDirection, false)) + Vector3.up * 0.2f;
            missileIndicators.Add(Instantiate(missileIndicatorPrefab, spawnPos, Quaternion.Euler(0, (int)PlayerScript.currentDirection, 0)));
            StartCoroutine(MissileWarningCountdown(isMissile));
        }


        yield return new WaitWhile(() => isWarning);

        missileFinished = true;
    }

    [HideInInspector] public bool missileFinished = true;

    IEnumerator MissileWarningCountdown(bool isMissile) {
        isWarning = true;
        missileFinished = false;
        yield return new WaitForSeconds(4f);
        //isWarning = false;
        StartCoroutine(ShootMissiles(isMissile));
    }

    void DuringMissileWarningUpdate() {
        if (allowMissiles) {
            for (int i = 0; i < missileLaneIndexes.Count; i++) {
                Vector3 spawnPos = addToVectorAlongDirectionRemoveRest(play.transform.position, 0, PlayerScript.currentDirection); ;
                spawnPos += addToVectorAlongDirectionRemoveRest(latestChunkPosEnd, lanePositions[missileLaneIndexes[i]], dirAfterTurn(PlayerScript.currentDirection, false)) + Vector3.up * 0.2f;
                missileIndicators[i].transform.position = spawnPos;
            }
        }
    }

    IEnumerator ShootMissiles(bool isMissile) {

        if (isMissile) {
            if (allowMissiles) {

                foreach (GameObject indicator in missileIndicators) {
                    Animation[] clips = indicator.GetComponentsInChildren<Animation>();
                    foreach (Animation a in clips) {
                        a.Play();
                    }
                    ParticleSystem[] effects = indicator.GetComponentsInChildren<ParticleSystem>();
                    foreach (ParticleSystem p in effects) {
                        if (p.name == "fire")
                            p.Play();
                    }
                }

                yield return new WaitForSeconds(0.5f);


                foreach (GameObject indicator in missileIndicators) {
                    ParticleSystem[] effects = indicator.GetComponentsInChildren<ParticleSystem>();
                    foreach (ParticleSystem p in effects) {
                        if (p.name != "fire")
                            p.Play();
                    }
                }
            }
        }
        

        OnMissileFinish();

    }

    void OnMissileFinish() {
        foreach (GameObject indicator in missileIndicators) {
            Destroy(indicator, 0.2f);
        }
        isWarning = false;
        missileIndicators.Clear();
        missileLaneIndexes.Clear();
        missileFinished = true;
    }

    public IEnumerator RestrictTurn(){
        allowMissiles = false;
        yield return new WaitWhile(() => isWarning);
        yield return new WaitUntil(() => missileFinished);
        allowMissiles = true;
    }

    public GameObject miniGameReference;
    Image miniGamePointer, targetArc;
    [HideInInspector] public bool isInMinigame = false;
    public float miniGameRotationSpeed;

    public void PlayMiniGame(){
        isInMinigame = true;
        Time.timeScale = 0.08f;
        play.tintImage.gameObject.SetActive(true);
        play.tintImage.color = play.damageTintColor;
        miniGameReference.SetActive(true);

        //Randomize rotation of target zone
        targetArc.transform.Rotate(Vector3.forward * Random.Range(-180, 180));
    }

    
    public GameObject[] toHideObjs;

    public void HideUglyButtons() {
        foreach(GameObject g in toHideObjs) {
            g.SetActive(false);
        }
    }
    
    public void ToggleMobile (){
        play.isMobile = !play.isMobile;
        play.isKeyboard = !play.isKeyboard;        
    }

    void DuringMiniGame() {
        miniGamePointer.transform.Rotate(Vector3.forward * miniGameRotationSpeed * Time.deltaTime);
    }

    Transform midPoint;
    public Text miniGameText;
    public void OnMiniGameStopButtonPress(){
        midPoint.rotation = targetArc.transform.rotation;
        midPoint.Rotate(Vector3.forward, -targetArc.fillAmount * 180);
        if (Quaternion.Angle(miniGamePointer.transform.rotation, midPoint.rotation) < targetArc.fillAmount * 180) {
            Revive();
        } else {
            MiniGameFail();
        }
        isInMinigame = false;

        StartCoroutine(ResetMiniGameDelayed());
    }

    IEnumerator ResetMiniGameDelayed(){
        yield return new WaitForSecondsRealtime(2f);
        Time.timeScale = 1f;
        play.tintImage.gameObject.SetActive(false);
        miniGameReference.SetActive(false);
        miniGameText.text = "STOP";
    }

    void Revive(){
        miniGameText.text = "SUCCESS";
        play.health = 50;
        play.shield = 100;
        play.UpdateShieldAndHealth();
    }

    void MiniGameFail(){
        miniGameText.text = "FAIL";
        play.GameOver();
    }


    public Text subtitlesText;

    public IEnumerator DisplaySubtitles(string txt, float duration) {
        subtitlesText.gameObject.SetActive(true);
        subtitlesText.text = txt;

        yield return new WaitForSeconds(duration);
        subtitlesText.gameObject.SetActive(false);
    }

    [System.Serializable]
    public struct SoundEffectPair{
        public string name;
        public AudioClip clip;
    };

    public List<SoundEffectPair> soundEffectsList = new List<SoundEffectPair>();
    Dictionary<string, AudioClip> soundEffectsDictionary = new Dictionary<string, AudioClip>();

    void TranscribleSoundFXListToDict(){
        foreach(SoundEffectPair sound in soundEffectsList) {
            soundEffectsDictionary.Add(sound.name, sound.clip);
        }
    }

    public void PlayExplosionSound() {
        explosionAudio.Play();
    }
}
