using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum PathMode{
    backForth, repeat, oneWay 
}

public enum EnemyState{
    patrol, stunned, confused, hostile 
}

public class EnemyBehavior : MonoBehaviour {


    [Tooltip("Create custom path or just let it turn when hitting a wall?")]
    public bool useCustomPath;
    [Tooltip("ID to match path ID so enemy follows the correct path")]
    public int pathId;
    [Tooltip("If the enemy reaches the end of path, then...")]
    public PathMode pathMode;
    [Tooltip("GameObject containing all path points in order")]
    public GameObject customPath;
    [HideInInspector]public Transform[] pathPoints;
    [Tooltip("Rotation at the start of the game (counterclockwise)")]
    public float startRotation;
    [Tooltip("Should the enemy start with a random rotation?")]
    public bool randomStartRotation;
    [Tooltip("If enemy touches a wall, turn how many degrees counterclockwise?")]
    public float turnAmount;
    [Tooltip("If enemy touches a wall, should it rotate randomly?")]
    public bool randomTurnRotation;
    [Tooltip("How fast is the enemy?")]
    public float moveSpeed;
    [Tooltip("How quick should the enemy turn when changing directions (0.03 is default)")]
    public float turnSpeed = 0.1f;
    [Tooltip("How long should turning the enemy turn when changing directions (in seconds)?")]
    public float turnDuration;
    [Tooltip("When mask stolen, how long should the enemy be stun?")]
    public float stunDuration;
    [Tooltip("When the player is seen, how much time will the player have to escape?")]
    public float escapeDuration;

    public Sprite stunnedExpressionSprite;
    public Sprite hostileExpressionSprite;
    public Sprite confusedExpressionSprite;

    // Use this for initialization
    void Awake () {
        pathPoints = customPath.GetComponentsInChildren<Transform>();
        pathPoints = pathPoints.Skip(1).ToArray();
    }

    // Update is called once per frame
    void Update () {
	} 
}
