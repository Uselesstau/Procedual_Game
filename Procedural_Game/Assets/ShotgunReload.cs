using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunReload : MonoBehaviour
{
    public GameObject player;

    public void ReloadShotGun()
    {
        player.GetComponent<PlayerScript>().totalBulletsS--;
        player.GetComponent<PlayerScript>().bulletsInMagS++;
    }

    public void StopReloading()
    {
        player.GetComponent<PlayerScript>().reloadingTime = -1;
        player.GetComponent<PlayerScript>().armS.SetBool("reload", false);
        player.GetComponent<PlayerScript>().armS.SetBool("multipleReload", false);
    }
}
