using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffect : MonoBehaviour
{
    public float battery;
    public bool isBattery;

    public int bullets;
    public bool isMag;
    
    public int oxygen;
    public bool isOxygen;

    public int nVBattery;
    public bool isNVBattery;

    public bool isFlare;

    public bool isFlashBang;

    public int keyCode;
    void Start()
    {
        if (isBattery)
        {
            battery = Random.Range(1,11);
        }
        if (isMag)
        {
            bullets = Random.Range(1, 31);
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
