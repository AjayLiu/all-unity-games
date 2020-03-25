using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyScript : MonoBehaviour {
    EnemyBehavior behavior;
    EnemyState status = EnemyState.patrol;
    Rigidbody2D rb;
    MaskScript mask;
    Transform rotatingObjects;
    SpriteRenderer expressionSprite;

    bool allowMove = true;
    int pointIndex = 0;

    void Awake(){
        behavior = GetComponentInParent<EnemyBehavior>();
        rb = GetComponent<Rigidbody2D>();
        mask = GetComponentInParent<MaskScript>();
        rotatingObjects = transform.GetChild(1);
        expressionSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        ClearExpressionPicture();
    }

    // Use this for initialization
    void Start () {
        if (behavior.useCustomPath)
            StartCoroutine("ChangeDirection");
        else
            SetStartDirection();
    }
	
    void FixedUpdate(){
        SmoothRotation();
    }

    void LateUpdate () {
        Move();
    }

    void OnHitWall() {
        StartCoroutine("ChangeDirection");
    }

    void OnHitPlayer() {
        
    }

    #region movement and rotation

    void Move(){
        if (allowMove)
            rb.velocity = rotatingObjects.up * behavior.moveSpeed * Time.deltaTime;
        else
            rb.velocity = Vector2.zero;
    }

    void SetStartDirection(){
        if(behavior.randomStartRotation){
            behavior.startRotation = UnityEngine.Random.Range(0, 361);
        }

        rb.rotation = behavior.startRotation;
                  
    }

    public float turnWallBuffer = 0.1f;

    float rotationAmount;

    public IEnumerator ChangeDirection(){
        allowMove = false;

        // IF NOT CUSTOM PATH
        if (!behavior.useCustomPath) {
            transform.position -= rotatingObjects.up * turnWallBuffer;

            if (behavior.randomTurnRotation) {
                behavior.turnAmount = UnityEngine.Random.Range(0, 361);
            }

            rotationAmount += behavior.turnAmount;
        } else {
            //CUSTOM PATH
            if (pointIndex == behavior.pathPoints.Length) {
                switch (behavior.pathMode) {
                    case PathMode.backForth:
                        if (behavior.pathPoints.Length > 2) {
                            Array.Reverse(behavior.pathPoints);
                            pointIndex = 2;
                            rotationAmount = LookAtToAngle(behavior.pathPoints[1].position);
                        } else {
                            Array.Reverse(behavior.pathPoints);
                            pointIndex = 0;
                            rotationAmount = LookAtToAngle(behavior.pathPoints[1].position);
                        }
                        break;

                    case PathMode.repeat:
                        pointIndex = 1;
                        rotationAmount = LookAtToAngle(behavior.pathPoints[0].position);
                        break;

                    case PathMode.oneWay:
                        allowMove = false;
                        yield break;                        

                    default:
                        break;
                }
            } else {                
                rotationAmount = LookAtToAngle(behavior.pathPoints[pointIndex].transform.position);
                pointIndex++;
            }

        }
        smoothRotate = true;

        yield return new WaitForSeconds(behavior.turnDuration);

        allowMove = true;
    }

    bool smoothRotate = false;
    
    void SmoothRotation(){
        if (smoothRotate)
            rotatingObjects.eulerAngles = new Vector3(0,0,Mathf.LerpAngle(rotatingObjects.eulerAngles.z, rotationAmount, behavior.turnSpeed));
    }


    float LookAtToAngle(Vector2 point){
         Vector2 diff = point - (Vector2)transform.position;
         diff.Normalize();
 
         float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
         return rot_z - 90;
    }

    #endregion

    #region reactions

    void OnStatusChange(EnemyState newState){
        switch(newState) {
            case EnemyState.patrol:
                ClearExpressionPicture();
                allowMove = true;
                break;
            case EnemyState.stunned:
                SetExpressionPicture(behavior.stunnedExpressionSprite);
                allowMove = false;
                break;
            case EnemyState.confused:
                SetExpressionPicture(behavior.confusedExpressionSprite);
                break;
            case EnemyState.hostile:
                SetExpressionPicture(behavior.hostileExpressionSprite);
                break;
            default:
                break;
        }
    }

    public void ClearExpressionPicture(){
        expressionSprite.sprite = null;
    }
    public void SetExpressionPicture(Sprite expressionPicture){
        expressionSprite.sprite = expressionPicture;
    }

    public IEnumerator Stun(){
        OnStatusChange(EnemyState.stunned);
        
        yield return new WaitForSeconds(behavior.stunDuration);

        OnStatusChange(EnemyState.patrol);
    }

    public void OnPlayerSeen() {
        if(mask.maskID != PlayerControl.mask.maskID) {
            StartCoroutine("PlayerSeenCountdown");
        }
    }    

    IEnumerator PlayerSeenCountdown(){
        print("SEEN");
        yield return new WaitForSeconds(behavior.escapeDuration);
        //If this coroutine is not cancelled, then game over
        OnHostile();
    }

    public void OnEscapeView(){
        if(mask.maskID != PlayerControl.mask.maskID){
            print("ESCAPE");
            OnStatusChange(EnemyState.confused);
            StopCoroutine("PlayerSeenCountdown");
        }        
    }

    void OnHostile(){
        OnStatusChange(EnemyState.hostile);
    }

    #endregion

    #region collision detection
    void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.tag == "Tilemap"){
            OnHitWall();
        }
    }
    
    
    #endregion

    #region game over
    void GameOver() {
        print("Game Over");
    }
    #endregion
}
