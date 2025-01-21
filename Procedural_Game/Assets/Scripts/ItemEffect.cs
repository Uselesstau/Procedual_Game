using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffect : MonoBehaviour
{
    public float battery;
    public bool isBattery;

    public int bulletsR;
    public bool isMagR;
    
    public int bulletsP;
    public bool isMagP;
    
    public int bulletsS;
    public bool isMagS;
    
    public int oxygen;
    public bool isOxygen;

    public int nVBattery;
    public bool isNVBattery;

    public bool isFlare;
    
    public bool isUVFlare;

    public bool isFlashBang;

    public bool isRifle;

    public bool isShotgun;

    public int keyCode;
    void Start()
    {
        if (isBattery)
        {
            battery = Random.Range(1,11);
        }
        if (isMagR)
        {
            bulletsR = Random.Range(1, 11);
        }
        if (isMagP)
        {
            bulletsP = Random.Range(1, 17);
        }
        if (isMagS)
        {
            bulletsS = Random.Range(1, 5);
        }
        if (isOxygen)
        {
            oxygen = Random.Range(5, 26);
        }
        if (isNVBattery)
        {
            nVBattery = Random.Range(10, 21);
        }
    }
}
