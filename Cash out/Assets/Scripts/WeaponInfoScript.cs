using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DamageChart {
    public int directHitsToKill;
    public int indirectHitsToKill;
    public DamageChart(int d, int i) {
        directHitsToKill = d;
        indirectHitsToKill = i;
    }
};

public class WeaponInfoScript : MonoBehaviour
{
    public string weapName;
    public bool isAutomatic;
    public float automaticIntervalTime;
    public float cooldownBetweenTriggers;
    public int clipSize;
    public Sprite ammoSprite;
    [HideInInspector] public int bulletsInMyChamber;
    public float reloadTime;

    public bool isShotgun;
    public int shotgunBulletsPerShot;
    public float bulletSpreadDegrees;

    public List<DamageChart> chart = new List<DamageChart>();

    public AudioClip reloadAudio;
    public AudioClip rackingAudio;

    public List<AudioClip> shootAudios;

    // Start is called before the first frame update
    void Awake()
    {
        bulletsInMyChamber = clipSize;
    }

    void Start() {
    }

    // Update is called once per frame
    void Update()
    {

    }

    
}
