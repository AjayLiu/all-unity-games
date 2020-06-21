using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public enum Direction {
    North = 0, East = 90, South = 180, West = 270
}

public class PlayerScript : MonoBehaviour
{
    public static Direction currentDirection;

    public bool isKeyboard, isMobile, isLaneInput;

    int totalLanes;
    public int currentLane = 2;

    [HideInInspector]public float shield = 100f, health = 100f;


    public float speed, turnSpeed;
    [HideInInspector]public float originalSpeed, slowSpeed;
    public float[] autoTurnLeftSpeedsForEachLane;

    public GameObject bulletPrefab;

    Rigidbody rbody;

    [HideInInspector] public bool isCrash = false;
    bool allowShoot = true;

    public GraphicRaycaster gRaycaster;
    public PointerEventData pData;

    GameControllerScript game;

    bool lockIntoLanes;

    FollowPlayer camScript;

    AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        originalSpeed = speed;
        slowSpeed = speed / 2;
        defaultRotationClamp = rotationClamp;
        game = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
        camScript = Camera.main.GetComponent<FollowPlayer>();

        SlowdownEnd();


        lockIntoLanes = isLaneInput;
        totalLanes = game.lanePositions.Length;

        rbody = GetComponent<Rigidbody>();
        bulletsLeft = bulletMaxCapacity;
        ammoCounterText.text = bulletsLeft.ToString();
        reloadProgressImgOnCar.enabled = false;
        reloadProgressImgOnButton.enabled = false;
        damageIndicator.enabled = false;
        reloadGrayOut.enabled = false;

        audio = GetComponent<AudioSource>();


        StartCoroutine(DisplayHealthAndShield());


        //SetMobileInput();

    }


    public Canvas inputSelectCanvas;
    public void SetMobileInput() {
        isKeyboard = false;
        isMobile = true;
        isLaneInput = true;
        inputSelectCanvas.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
    public void SetKeyboardInput() {
        isKeyboard = true;
        isMobile = false;
        isLaneInput = false;
        inputSelectCanvas.gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    void Update()
    {
        if (!isCrash) {

            TrackTouches();
            if (allowShoot) {
                Shoot();
            }

            DetectTurningUpdate();
            
            if (isSlowdown) {
                DuringSlowdown();
            }

            if (isRepairing)
            {
                Repair();
            }

            CheckForWallGrind();

            RegenerateShield();
        }

        if (!isReloading){        
            if (Input.GetKeyDown(KeyCode.R)) {
                OnReloadButtonPress();
            }
        }

        if (Input.GetKey(KeyCode.E)) {
            Repair();
        }

        if (Input.GetKeyDown(KeyCode.F)) {
            StartCoroutine(SwitchWeapon(true));
        }

    }

    void FixedUpdate(){
        if (!isCrash) {
            Move();
            DetectTurningFixed();
            RoundRotation();            
        }
    }

    void RoundRotation(){
        transform.eulerAngles = new Vector3(0, currentRotation, 0);
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        //if is lane input mode, round to the nearest lanes when idle
        if (isLaneInput && !isChangingLane && lockIntoLanes) {
            SetPosToClosestLane();
        }

        if (!autoTurn) {
            float angleFromForward = Mathf.DeltaAngle((int)currentDirection, transform.eulerAngles.y);
            if (Mathf.Abs(angleFromForward) > rotationClamp) {
                Turn(angleFromForward > 0 ? -turnSpeed : turnSpeed, true);
            }
        }

        myClosestLaneIndex = GetClosestLaneIndex();
    }

    bool consistentForwardSpeed = true;

    bool laneChangeIsLeft = false;

    void Move(){
        Vector3 velocity = transform.forward * speed;
        if (consistentForwardSpeed) {
            velocity = GameControllerScript.addToVectorAlongDirectionRemoveRest(velocity, 0, GameControllerScript.dirAfterTurn(currentDirection, false));
            if (currentDirection == Direction.South || currentDirection == Direction.West)
                velocity = GameControllerScript.addToVectorAlongDirection(velocity, -speed, currentDirection);
            else {
                velocity = GameControllerScript.addToVectorAlongDirection(velocity, speed, currentDirection);
            }
        }
        

        rbody.velocity = velocity;

        if (isChangingLane) {
            DuringLaneChange(laneChangeIsLeft);
        }
    }

    public float swipeSensitivity = 0.1f, mobileTiltSensitivity = 0.5f;

    void DetectTurningUpdate() {
        if (isKeyboard) {
            if (isLaneInput) {
                if (Input.GetKeyDown(KeyCode.A)) {
                    if (isSlowdown) {
                        autoTurnDecided = true;
                        autoTurnIsLeft = true;
                        autoTurnDesiredDirection = GameControllerScript.dirAfterTurn(currentDirection, autoTurnIsLeft);
                        SlowdownEnd();
                    } else {
                        ChangeLaneIfValid(true);
                    }
                }
                if (Input.GetKeyDown(KeyCode.D)) {
                    if (isSlowdown) {
                        autoTurnDecided = true;
                        autoTurnIsLeft = false;
                        autoTurnDesiredDirection = GameControllerScript.dirAfterTurn(currentDirection, autoTurnIsLeft);
                        SlowdownEnd();
                    } else {
                        ChangeLaneIfValid(false);
                    }
                }

            }

        }
    }
    void DetectTurningFixed(){
        

        if (isMobile) {
            if (!isLaneInput) {
                Turn(Input.acceleration.x * mobileTiltSensitivity);
            }

            // laneinput handled in TrackTouches();
        }

        if (isKeyboard && !isLaneInput) {
            if (Input.GetKey(KeyCode.A)) {
                Turn(-turnSpeed);
            }
            if (Input.GetKey(KeyCode.D)) {
                Turn(turnSpeed);
            }
        }
        

        if (autoTurn)
            AutoTurnUpdate();
    }

    public float rotationClamp;
    [HideInInspector] public float defaultRotationClamp;

    [HideInInspector] public float currentRotation;

    void Turn(float turnAmount) {
        transform.eulerAngles = new Vector3(0, currentRotation, 0);
        transform.Rotate(Vector3.up, turnAmount);
        //Clamp Rotation

        float angleFromForward = Mathf.DeltaAngle((int)currentDirection, transform.eulerAngles.y);
        if (Mathf.Abs(angleFromForward) > rotationClamp) {
            transform.Rotate(Vector3.up, -turnAmount);            
        }        
        currentRotation = transform.eulerAngles.y;
        
    }

    void Turn(float turnAmount, bool isStuck) {
        if (isStuck) {
            transform.eulerAngles = new Vector3(0, currentRotation, 0);
            transform.Rotate(Vector3.up, turnAmount);
            currentRotation = transform.eulerAngles.y;
        }
    }

    bool isChangingLane = false;


    void ChangeLaneIfValid(bool toLeft) {
        if (!autoTurn && !autoTurnDecided) {
            if (!isChangingLane) {
                if (toLeft) {
                    if (currentLane == 0)
                        return;

                    currentLane--;
                } else {
                    if (currentLane == totalLanes - 1)
                        return;

                    currentLane++;
                }
                StartCoroutine(ChangeLaneAnimation(toLeft));
            }            
        }        
    }

    
    public Vector3 laneChangeOffset;

    [HideInInspector] public Vector3 laneChangeTarget;

    [HideInInspector] public int myClosestLaneIndex;

    IEnumerator ChangeLaneAnimation(bool toLeft){
        isChangingLane = true;
        laneChangeIsLeft = toLeft;
        //IF NORTH (0,0,1)
        //IF WEST (-1,0,0)
        laneChangeTarget = transform.position + 
            GameControllerScript.dirToVector(GameControllerScript.dirAfterTurn(currentDirection, toLeft)) * laneChangeOffset.x //SIDE OFFSET
            + GameControllerScript.dirToVector() * laneChangeOffset.z; //FORWARD OFFSET

        laneChangeDifference = GameControllerScript.differenceAlongDirection(transform.position, laneChangeTarget, GameControllerScript.dirAfterTurn(currentDirection, false));


        yield return new WaitWhile(()=>isChangingLane);
        
        currentRotation = (int)currentDirection;
        SetPosToClosestLane();
    }

    void SetPosToClosestLane(){
        switch (currentDirection) {
            case Direction.North:
                transform.position = new Vector3(game.latestChunkPosEnd.x + game.lanePositions[currentLane], 0, transform.position.z);
                break;
            case Direction.East:
                transform.position = new Vector3(transform.position.x, 0, game.latestChunkPosEnd.z - game.lanePositions[currentLane]);
                break;
            case Direction.South:
                transform.position = new Vector3(game.latestChunkPosEnd.x - game.lanePositions[currentLane], 0, transform.position.z);
                break;
            case Direction.West:
                transform.position = new Vector3(transform.position.x, 0, game.latestChunkPosEnd.z + game.lanePositions[currentLane]);
                break;
        }
    }

    int GetClosestLaneIndex(){
        switch (currentDirection) {
            case Direction.North:
                return GetIndexOfClosestInArray(game.lanePositions, transform.position.x - game.latestChunkPosEnd.x);                
            case Direction.East:
                return GetIndexOfClosestInArray(game.lanePositions, transform.position.z - game.latestChunkPosEnd.z);
            case Direction.South:
                return GetIndexOfClosestInArray(game.lanePositions, transform.position.x - game.latestChunkPosEnd.x);
            case Direction.West:
                return GetIndexOfClosestInArray(game.lanePositions, transform.position.z - game.latestChunkPosEnd.z);
        }
        return -1;
    }

    int GetIndexOfClosestInArray(float[] arr, float val) {
        int index = 0;
        for(int i = 0; i < arr.Length; i++) {
            if(Mathf.Abs(arr[i] - val) < Mathf.Abs(arr[index] - val)) {
                index = i;
            }
        }
        return index;
    }

    float laneChangeDifference;

    void DuringLaneChange(bool isLeft){
        /*
        if(Vector3.Distance(transform.position, laneChangeTarget) > 0.5f){
            transform.LookAt(laneChangeTarget);
            currentRotation = transform.eulerAngles.y;
        } else {
            //LANE CHANGE FINISHED
            isChangingLane = false;
        }
        */

        float difference = Mathf.Abs(GameControllerScript.differenceAlongDirection(transform.position, laneChangeTarget, GameControllerScript.dirAfterTurn(currentDirection, false)));
        //  difference, 6 laneChangeDifference

        if (difference > 0.1f) {
            float steerSpeed = turnSpeed * turnSpeed * 0.7f;
            if (Mathf.Abs(laneChangeDifference) - difference < Mathf.Abs(laneChangeDifference) * 0.45f) {
                Turn(isLeft ? -steerSpeed : steerSpeed);
            } else {
                Turn(isLeft ? steerSpeed : -steerSpeed);
            }
            currentRotation = transform.eulerAngles.y;
        } else {
            //LANE CHANGE FINISHED
            isChangingLane = false;
        }
    }

    bool isSlowdown = false;
    //At intersection
    public void Slowdown(){
        isSlowdown = true;
        tintImage.gameObject.SetActive(true);
        tintImage.color = slowDownTintColor;
    }

    void SlowdownEnd(){
        isSlowdown = false;
        if(!game.isInMinigame)
            Time.timeScale = 1f;
        tintImage.gameObject.SetActive(false);
    }

    public float slowdownMultiplier = 0.5f;
    void DuringSlowdown(){
        if(!game.isInMinigame)
            Time.timeScale = slowdownMultiplier;
    }

    Direction autoTurnDesiredDirection;
    public void AutoTurn(){
        SlowdownEnd();
        if (autoTurnDecided) {
            consistentForwardSpeed = false;
            rotationClamp = 90;
            if (isLaneInput && !isChangingLane) {
                //autoTurnDesiredDirection = GameControllerScript.dirAfterTurn(currentDirection, isLeft);
                //autoTurnIsLeft = isLeft;
                autoTurn = true;
                lockIntoLanes = false;
            }
        }
        
        
    }

    [HideInInspector]public bool autoTurn;
    bool autoTurnDecided = false;
    bool autoTurnIsLeft;
    void AutoTurnUpdate(){       
        if (autoTurnIsLeft) {
            Turn(-autoTurnLeftSpeedsForEachLane[currentLane]);
        } else {
            Turn(autoTurnLeftSpeedsForEachLane[totalLanes - currentLane - 1]);
        }
    }

    public void OnIntersectionExit(bool isLeft) {
        if(autoTurn)
            AutoTurnFinish();
        else
            currentDirection = GameControllerScript.dirAfterTurn(PlayerScript.currentDirection, isLeft);

    }

    void AutoTurnFinish() {
        consistentForwardSpeed = true;
        autoTurn = false;
        autoTurnDecided = false;
        lockIntoLanes = true;
        currentRotation = (int)autoTurnDesiredDirection;
        currentDirection = autoTurnDesiredDirection;
        rotationClamp = defaultRotationClamp;
    }

    public LinkedList<BulletScript> bulletList = new LinkedList<BulletScript>();

    float timeOfLastKeyboardShoot = 0;
    bool isHold;
    void Shoot(){
        //FOR KEYBOARD
        if (isKeyboard){
            if(Input.GetMouseButtonDown(0)){
                if(Time.timeSinceLevelLoad - timeOfLastKeyboardShoot > game.currentWeap.cooldownBetweenTriggers) {
                    TapAt(Input.mousePosition);
                    timeOfLastKeyboardShoot = Time.timeSinceLevelLoad;
                }

            } else if (Input.GetMouseButton(0)) {
                if(Time.timeSinceLevelLoad - timeOfLastKeyboardShoot > tapToHoldTimeThreshold) {
                    isHold = true;
                    if (game.currentWeap.isAutomatic && allowHold) {
                        StartCoroutine(OnHold(Input.mousePosition));
                    }
                }                
            }

            if (Input.GetMouseButtonUp(0)) {
                isHold = false;
            }
        }

        //FOR MOBILE
        //HANDLED IN TrackTouches();

        if (bulletsLeft <= 0) {
            allowShoot = false;

            //Auto reload
            if (bulletsLeft == 0 && !isReloading) {
                currentReloadCoroutine = StartCoroutine(Reload());
            }
        } else {
            allowShoot = true;
        }
    }

    RaycastHit ValidHit(Vector2 screenPos) {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        RaycastHit hit;

        // If the player clicked on something
        if (Physics.Raycast(ray, out hit)) {
            //IGNORE ALL UI            
            List<RaycastResult> results = getUIRaycastResults(screenPos);

            if (results.Count > 0) {
                foreach(RaycastResult r in results) {
                    if(r.gameObject.layer != 2)
                        return new RaycastHit();
                }
            }
                      
        }

        return hit;
    }

    public List<RaycastResult> getUIRaycastResults (Vector2 screenPos) {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        PointerEventData ped = new PointerEventData(null);
        ped.position = screenPos;
        List<RaycastResult> results = new List<RaycastResult>();
        gRaycaster.Raycast(ped, results);

        return results;       
    }

    void TapAt(Vector3 screenPos) {
        RaycastHit hit = ValidHit(screenPos);

        //IF IT HIT SOMETHING VALID
        if(!hit.Equals(new RaycastHit())) {
            if (hit.transform.gameObject.layer != 5) {
                //IF SUCCESSFULLY CLICKED ON TARGET (DIRECT HIT)
                if (hit.transform.gameObject.tag == "Obstacle" || hit.transform.gameObject.tag == "Police") {
                    if (!game.currentWeap.isShotgun) {
                        hit.transform.gameObject.GetComponent<HealthBarScript>().HitFromPlayer(game.currentWeap.chart, true);
                    }
                    ShootBullet(hit.point, true, hit.transform);
                } else if (hit.transform.gameObject.tag == "Player"){
                    currentReloadCoroutine = StartCoroutine(Reload());
                } else {
                    ShootBullet(hit.point, false, null);
                }
            }
        } else {
            //SEE IF PLAYER IS TRYING TO RELOAD
            if (isMobile) {
                List<RaycastResult> results = getUIRaycastResults(screenPos);
                foreach (RaycastResult r in results) {
                    if (r.gameObject.tag == "Gun Icon") {
                        OnReloadButtonPress();
                        break;
                    }
                }
            }
            
        }

        
    }

    struct TouchInfo{
        public float startTime;
        public Vector2 startPosition;

        public TouchInfo(float startTime, Vector2 startPosition) {
            this.startTime = startTime;
            this.startPosition = startPosition;
        }
    };

    //PARALLEL LISTS
    List<int> touchIDList = new List<int>();
    List<TouchInfo> touchStartInfos = new List<TouchInfo>();

    public float tapToSwipeDistanceThreshold = 0.1f;
    public float tapToHoldTimeThreshold = 0.2f;

    bool allowHold = true;

    float timeOfLatestShot = 0;

    void TrackTouches() {
        if(Input.touchCount == 0) {
            isHold = false;
        }
        foreach(Touch t in Input.touches) {
            if(t.phase == TouchPhase.Began) {                
                touchIDList.Add(t.fingerId);
                touchStartInfos.Add(new TouchInfo(Time.timeSinceLevelLoad, t.position));
            }
            if(t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary) {
                int index = touchIDList.IndexOf(t.fingerId);
                TouchInfo info = touchStartInfos[index];
                if(Time.timeSinceLevelLoad - info.startTime> tapToHoldTimeThreshold) {
                    isHold = true;
                    if (game.currentWeap.isAutomatic && allowHold)
                        StartCoroutine(OnHold(t.position));
                }
            }
            if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled) {
                int index = touchIDList.IndexOf(t.fingerId);
                TouchInfo info = touchStartInfos[index];
                if(Vector2.Distance(t.position, info.startPosition) > tapToSwipeDistanceThreshold) {
                    // SWIPE //
                    //HORIZONTAL
                    if (Mathf.Abs(t.position.x - info.startPosition.x) > Mathf.Abs(t.position.y - info.startPosition.y)) {
                        OnSwipe(info.startPosition, t.position);
                    } else {
                        //VERTICAL
                        if (isSlowdown) {
                            //cancel slowdown
                            SlowdownEnd();
                        }
                    }
                    
                } else {
                    // TAP //
                    if(allowShoot && Time.timeSinceLevelLoad - timeOfLatestShot > game.currentWeap.cooldownBetweenTriggers) {
                        timeOfLatestShot = Time.timeSinceLevelLoad;
                        TapAt(t.position);
                    }
                }
                
                touchIDList.Remove(t.fingerId);
                touchStartInfos.RemoveAt(index);
            }
        }
    }

    void OnSwipe(Vector2 from, Vector2 to) {


        //SEE IF PLAYER IS TRYING TO SWITCH WEAPONS
        List<RaycastResult> results = getUIRaycastResults(from);
        bool isGunSwipe = false;
        foreach(RaycastResult r in results) {
            if(r.gameObject.tag == "Gun Icon") {
                isGunSwipe = true;
                break;
            }
        }
        if (isGunSwipe) {
            //SWITCH WEAPON
            StartCoroutine(SwitchWeapon(to.x - from.x < 0));
        } else {
            //DETERMINE AUTOTURN DIRECTION
            if (isSlowdown) {
                autoTurnDecided = true;
                autoTurnIsLeft = to.x - from.x < 0;
                autoTurnDesiredDirection = GameControllerScript.dirAfterTurn(currentDirection, autoTurnIsLeft);
                SlowdownEnd();
            } else {
                ChangeLaneIfValid(to.x - from.x < 0);
            }
        }

        
    }

    IEnumerator OnHold(Vector2 screenPos){
        allowHold = false;
        if(allowShoot)
            TapAt(screenPos);
        yield return new WaitForSeconds(game.currentWeap.automaticIntervalTime);
        isHold = false;
        allowHold = true;
    }



    float weaponSwitchCooldown = 1;
    bool allowWeapSwitch = true;

    IEnumerator SwitchWeapon(bool isLeft) {
        CancelReload();

        reloadProgressImgOnCar.enabled = true;
        reloadProgressImgOnButton.enabled = true;
        reloadProgressImgOnCar.color = Color.red;
        reloadProgressImgOnButton.color = Color.red;

        PlaySwitchAudio();

        reloadGrayOut.enabled = true;
        allowShoot = false;

        //Update the weapon switch progress meter
        int steps = 60;
        for (int i = 0; i <= steps; i++) {
            reloadProgressImgOnCar.fillAmount = (i * (weaponSwitchCooldown / steps)) / weaponSwitchCooldown;
            reloadProgressImgOnButton.fillAmount = reloadProgressImgOnCar.fillAmount;

            yield return new WaitForSeconds(weaponSwitchCooldown / steps);
        }

        //Change weapon
        
        //Wrapping weapon index
        if (isLeft) {
            if (game.currentWeaponIndex != 0) {
                game.currentWeaponIndex--;
            } else {
                game.currentWeaponIndex = game.weaponList.Count - 1;
            }
        } else {
            if (game.currentWeaponIndex != game.weaponList.Count - 1) {
                game.currentWeaponIndex++;
            } else {
                game.currentWeaponIndex = 0;
            }
        }
        
        game.ResetWeaponsList();
        ammoCounterText.text = bulletsLeft.ToString();



        reloadProgressImgOnCar.enabled = false;
        reloadProgressImgOnButton.enabled = false;

        reloadProgressImgOnCar.color = Color.white;
        reloadProgressImgOnButton.color = Color.white;
        reloadGrayOut.enabled = false;
        allowShoot = true;
    }

    void CancelReload(){
        if(isReloading)
            StopCoroutine(currentReloadCoroutine);
        isReloading = false;
    }

    [HideInInspector]public int bulletMaxCapacity = 7;
    [HideInInspector]public int bulletsLeft;
    public Text ammoCounterText;
    public Image ammoImage;

    void ShootBullet(Vector3 to, bool onTarget, Transform hitTransform){
        if(bulletsLeft > 0) {

            //deduct one bullet from magazine
            bulletsLeft--;
            ammoCounterText.text = bulletsLeft.ToString();

            //See if it is Shotgun
            if (game.currentWeap.isShotgun) {
                for(int i = 0; i < game.currentWeap.shotgunBulletsPerShot; i++) {                    
                    BulletScript bullet = Instantiate(bulletPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity).GetComponent<BulletScript>();
                    bullet.transform.LookAt(new Vector3(to.x, 0.5f, to.z));
                    bullet.transform.Rotate(Vector3.up * game.currentWeap.bulletSpreadDegrees * (game.currentWeap.shotgunBulletsPerShot / 2f - i));
                    bullet.myChart = game.currentWeap.chart;
                    bulletList.AddFirst(bullet);                               
                }
            } else {
                //Spawn Bullet
                BulletScript bullet = Instantiate(bulletPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity).GetComponent<BulletScript>();
                bullet.transform.LookAt(new Vector3(to.x, 0.5f, to.z));
                bullet.myChart = game.currentWeap.chart;
                if (onTarget) {
                    bullet.DrawDirectTrace(hitTransform);
                }
                bulletList.AddFirst(bullet);
            }

            PlayShootAudio();
            game.RefreshWeaponDisplay();
        }
    }


    bool isReloading = false;

    public void OnReloadButtonPress() {
        if(!isReloading)
            currentReloadCoroutine = StartCoroutine(Reload());
    }

    public Image reloadProgressImgOnCar, reloadProgressImgOnButton, reloadGrayOut;

    Coroutine currentReloadCoroutine;

    IEnumerator Reload() {
        reloadProgressImgOnCar.enabled = true;
        
        reloadProgressImgOnButton.enabled = true;

        reloadGrayOut.enabled = true;

        isReloading = true;
        allowShoot = false;

        PlayReloadAudio();

        //Update the reload progress meter
        int steps = 60;
        for(int i = 0; i <= steps; i++) {
            reloadProgressImgOnCar.fillAmount = (i * (game.currentWeap.reloadTime / steps)) / game.currentWeap.reloadTime;
            reloadProgressImgOnButton.fillAmount = reloadProgressImgOnCar.fillAmount;

            yield return new WaitForSeconds(game.currentWeap.reloadTime / steps);
        }

        bulletsLeft = bulletMaxCapacity;
        ammoCounterText.text = bulletsLeft.ToString();

        allowShoot = true;
        isReloading = false;
        reloadProgressImgOnCar.enabled = false;
        reloadProgressImgOnButton.enabled = false;
        reloadGrayOut.enabled = false;        
    }


    public Image shieldImg, healthImg;
    public Text damageIndicator;
    public Image tintImage;
    public Color damageTintColor, slowDownTintColor;

    Coroutine currentDamageCoroutine;
    Coroutine currentDamageTintCoroutine;

    public void TakeDamage(float damage) {
        if(currentDamageCoroutine != null)
            StopCoroutine(currentDamageCoroutine);
        currentDamageCoroutine = StartCoroutine(DisplayHealthAndShield());

        undamagedDuration = 0;
        float shieldAndHealthAfterDamage = shield + health - damage;

        if (shieldAndHealthAfterDamage < 0) {
            NoHP();
        }

        float damageRemaining = damage - shield;
        damageRemaining = Mathf.Clamp(damageRemaining, 0f, 100f);

        shield -= damage;
        shield = Mathf.Clamp(shield, 0f, 100f);

        health -= damageRemaining;
        health = Mathf.Clamp(health, 0f, 100f);

        UpdateShieldAndHealth();

        StartCoroutine(DisplayDamageTaken(damage));

        if (currentDamageTintCoroutine != null)
            StopCoroutine(currentDamageTintCoroutine);
        currentDamageTintCoroutine = StartCoroutine(DisplayDamageTint());
    }

    float displayHealthDuration = 2;
    IEnumerator DisplayHealthAndShield() {
        healthImg.transform.parent.gameObject.SetActive(true);
        yield return new WaitForSeconds(displayHealthDuration);
        healthImg.transform.parent.gameObject.SetActive(false);
    }

    public void UpdateShieldAndHealth(){
        shieldImg.fillAmount = shield / 100f;
        healthImg.fillAmount = health / 100f;
    }

    IEnumerator DisplayDamageTint() {
        tintImage.gameObject.SetActive(true);
        tintImage.color = damageTintColor;
        yield return new WaitForSeconds(0.1f);
        tintImage.gameObject.SetActive(false);
    }

    IEnumerator DisplayDamageTaken(float damage){
        damageIndicator.enabled = true;
        damageIndicator.text = "-" + damage;
        yield return new WaitForSeconds(0.3f);
        damageIndicator.enabled = false;

    }

    public float shieldRegenSpeed, healthRepairSpeed;

    float undamagedDuration = 0;
    float shieldRegenCooldown = 3f;
    void RegenerateShield()
    {
        undamagedDuration += Time.deltaTime;
        if(undamagedDuration > shieldRegenCooldown) {
            shield += shieldRegenSpeed * Time.deltaTime;
            shield = Mathf.Clamp(shield, 0f, 100f);
        }
        health += healthRepairSpeed * Time.deltaTime;
        health = Mathf.Clamp(health, 0f, 100f);
        UpdateShieldAndHealth();
    }


    bool isRepairing = false;
    float repairDuration = 0;
    public float repairTimeIntervalPerMultiply;
    byte repairMultiplier = 1;
    const byte REPAIR_MULTIPLIER_MAX = 8;
    public Text repairMultiplierText;
    public Image multiplierProgressCircle;

    public void OnRepairDown()
    {
        isRepairing = true;
    }

    void Repair()
    {
        if (!isReloading) {
            if(repairMultiplier < REPAIR_MULTIPLIER_MAX){
                repairDuration += Time.deltaTime;
                multiplierProgressCircle.fillAmount = (repairDuration % repairTimeIntervalPerMultiply) / repairTimeIntervalPerMultiply;
                //If repaired long enough, increase the multiplier
                if (repairDuration / repairTimeIntervalPerMultiply > repairMultiplier / 2 + 1) {
                    repairMultiplier *= 2;
                    repairMultiplierText.text = "x" + repairMultiplier;
                }

            }

            health += healthRepairSpeed * Time.deltaTime * repairMultiplier;
            health = Mathf.Clamp(health, 0f, 100f);
            UpdateShieldAndHealth();
        }
    }

    public void OnRepairUp()
    {
        isRepairing = false;
        RepairMultiplierInterrupted();
    }

    void RepairMultiplierInterrupted(){
        repairDuration = 0;
        repairMultiplier = 1;
        repairMultiplierText.text = "x1";
        multiplierProgressCircle.fillAmount = 0;
    }


    bool isWallGrind = false;
    bool wallGrindStartAllow = true;
    void CheckForWallGrind() {
        isWallGrind = Mathf.Abs(GameControllerScript.differenceAlongDirection(transform.position, game.latestChunkPosEnd, GameControllerScript.dirAfterTurn(currentDirection, false))) > game.roadWidth;
        if (wallGrindStartAllow) {
            StartCoroutine(WallGrind());
        }
    }

    float grindDamage = 1;

    IEnumerator WallGrind() {
        wallGrindStartAllow = false;
        yield return new WaitForSeconds(2f);
        while (isWallGrind) {
            yield return new WaitForSeconds(0.5f);
            TakeDamage(grindDamage);
        }
        wallGrindStartAllow = true;
    }

    void OnCollisionEnter(Collision col){
        RepairMultiplierInterrupted();       
    }

    

    

    public void ToggleSnap() {
        isLaneInput = !isLaneInput;
        camScript.isTrailMode = false;
    }


    //called from obstacles
    public float crashExplosionForce, crashExplosionRadius;

    void NoHP() {
        game.PlayMiniGame();
        
    }

    public void GameOver(){
        isCrash = true;
        rbody.useGravity = true;
        rbody.AddExplosionForce(crashExplosionForce, transform.position + Vector3.down * UnityEngine.Random.Range(0, 1f) + Vector3.back + Vector3.right * UnityEngine.Random.Range(-1f, 1f), crashExplosionRadius);
        rbody.drag = 1;
        rbody.angularDrag = 1;
        LinkedListNode<PoliceScript> policePointer = PoliceScript.regPoliceList.First;
        while (policePointer != null) {
            policePointer.Value.inPursuit = false;

            policePointer = policePointer.Next;
        }
    }


    public void PlayShootAudio() {

        if (!(isHold && audio.isPlaying)) {
            audio.clip = game.currentWeap.shootAudios[UnityEngine.Random.Range(0, game.currentWeap.shootAudios.Count)];
            audio.Play();
        }
        
    }

    public void PlayReloadAudio() {
        if (audio.isPlaying) {
            audio.clip = game.currentWeap.reloadAudio;
            audio.Play();
        }
        
    }

    public void PlaySwitchAudio() {
        if (audio.isPlaying) {

            audio.clip = game.currentWeap.rackingAudio;
            audio.Play();

        }
    }

}
