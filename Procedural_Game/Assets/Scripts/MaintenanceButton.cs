using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MaintenanceButton : MonoBehaviour
{
    public List<GameObject> lights;
    public GameObject door;
    
    public void OnPress()
    {
        GetComponent<Animator>().SetBool("Press", !GetComponent<Animator>().GetBool("Press"));
        
        lights = GameObject.FindGameObjectsWithTag("RoomLight").ToList();
        foreach (GameObject g in lights)
        {
            g.GetComponent<RoomLightScript>().forceOn = true;
        }
        Destroy(door);
    }
}
