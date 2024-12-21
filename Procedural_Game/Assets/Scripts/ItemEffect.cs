using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffect : MonoBehaviour
{
    public float battery;
    public bool isBattery;

    public int bullets;
    public bool isMag;

    public int keyCode;
    void Start()
    {
        if (isBattery)
        {
            battery = Random.Range(1,10f);
        }
        if (isMag)
        {
            bullets = Random.Range(1, 31);
        }
    }
}
