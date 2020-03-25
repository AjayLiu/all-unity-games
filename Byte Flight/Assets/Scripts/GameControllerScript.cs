using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameControllerScript : MonoBehaviour {

    public static int hpToColorRatio = 30;
    public static Color[] colors = new Color[] { Color.red, Color.yellow, Color.green, Color.cyan, Color.blue, Color.magenta, Color.black };

    [HideInInspector] public List<GameObject> enemies = new List<GameObject>();

    [SerializeField]
    GameObject enemyBlock;

    [SerializeField]
    GameObject marker;

    [SerializeField]
    float enemyBlockSpawnTime;

    float timer = 0;
    public Vector2 [] diffculty;
    GameObject player;
    int currenntDiffcullty=0;
    int enemysKilled = 0; 
    public void OnEnemyKilled()
    {
        if (enemysKilled>diffculty[currenntDiffcullty].x)
        {
enemyBlockSpawnTime= diffculty[currenntDiffcullty].y;
            currenntDiffcullty++;
        }   enemysKilled++;
    }
    float dist_x, dist_y;
    public static float leftBorder, rightBorder, upBorder, downBorder;


    int enemyBlock_MIN_HP = 1;
    int enemyBlock_MAX_HP = colors.Length * hpToColorRatio;
    float enemyBlock_MIN_SPEED = 1f;
    float enemyBlock_MAX_SPEED = 2f;

    // Use this for initialization
    void Start () {
        InvokeRepeating("Spawnbomb", 5, 5);






       
        player = GameObject.Find("Player");

        leftBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist_x)).x;
        rightBorder = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, dist_x)).x;
        upBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, dist_y)).y;
        downBorder = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, dist_y)).y;
    }
	

	// Update is called once per frame
	void Update () {
        TrackPlayer();
        Timer();
	}

    void TrackPlayer() {
        dist_x = (player.transform.position - Camera.main.transform.position).x;
        dist_y = (player.transform.position - Camera.main.transform.position).y;

        if (PlayerScript.player_hp <= 0)
            GameOver();
    }
    void Timer() {
        if (timer < enemyBlockSpawnTime) {
            timer += Time.deltaTime;
        } else {
            SpawnEnemyBlock();
            timer = 0;
        }
    }

    void SpawnEnemyBlock() {
        Vector3 randPos = new Vector3(Random.Range(leftBorder, rightBorder), Random.Range(downBorder, upBorder));
        GameObject instance = Instantiate(enemyBlock, randPos, Quaternion.identity);

        EnemyBlockScript enemyScript = instance.GetComponent<EnemyBlockScript>();
        enemyScript.block_hp = Random.Range(enemyBlock_MIN_HP, enemyBlock_MAX_HP);
        enemyScript.speed = Random.Range(enemyBlock_MIN_SPEED, enemyBlock_MAX_SPEED);

        enemies.Add(instance);
    }

    void GameOver() {
        print("LOL GAMEOVER!");
        PlayerScript.player_hp = 1000;
        PlayerPrefs.SetInt("score", enemysKilled);
        SceneManager.LoadScene(2);
    }
    public GameObject bomb;
    void Spawnbomb()
    {
        Vector3 randPos = new Vector3(Random.Range(leftBorder, rightBorder), Random.Range(downBorder, upBorder));
        GameObject instance = Instantiate(bomb, randPos, Quaternion.identity);

        BombScript enemyScript = instance.GetComponent<BombScript>();
        enemyScript.block_hp = Random.Range(enemyBlock_MIN_HP, enemyBlock_MAX_HP);

      


    }

}

