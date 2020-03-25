using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarScript : MonoBehaviour
{

    public int myChartIndex;

    // Start is called before the first frame update
    void Start()
    {
        healthBar = transform.Find("HealthBar").transform.GetChild(1);
        healthBar.transform.parent.gameObject.SetActive(false);
        fullHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        MakeHealthBarFaceCamera();
    }

    [HideInInspector] public Transform healthBar;

    [HideInInspector] public float health = 100f;
    [HideInInspector] public float fullHealth;

    public void HitFromPlayer(DamageChart chart, bool isDirect) {
        healthBar.transform.parent.gameObject.SetActive(true);
        health -= isDirect ? fullHealth / chart.directHitsToKill : fullHealth / chart.indirectHitsToKill;

        UpdateHealthBar();

        if (health <= 0) {
            BroadcastMessage("ShotDownFromBullet");
            healthBar.transform.parent.gameObject.SetActive(false);
        }
    }

    public void HitFromPlayer(List<DamageChart> charts, bool isDirect) {
        HitFromPlayer(charts[myChartIndex], isDirect);
    }


    public void UpdateHealthBar() {
        healthBar.localScale = new Vector3(health / fullHealth, healthBar.localScale.y, healthBar.localScale.z);
    }

    void MakeHealthBarFaceCamera() {
        //Look at the camera but only move the x angle
        healthBar.parent.LookAt(Camera.main.transform.position - GameControllerScript.dirToVector() * 50 + Vector3.up * 50, Vector3.up);
        //healthBar.parent.localEulerAngles = new Vector3(healthBar.parent.localEulerAngles.x, 0, 0);
    }
   
}
