using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonUseElevator : MonoBehaviour
{
    public GameObject Elevator;
    public GameObject Player;
    public void OnPress()
    {
		GetComponent<Animator>().SetBool("Press", !GetComponent<Animator>().GetBool("Press"));
		InvokeRepeating("MoveElevator",0,Time.deltaTime);
        Player = GameObject.Find("Player");
        Player.GetComponent<PlayerScript>().oxygenLeft = Player.GetComponent<PlayerScript>().maxOxygen;
	}
    void MoveElevator()
    {
        Elevator.transform.localPosition = new Vector3(Elevator.transform.localPosition.x, Mathf.Clamp(Elevator.transform.localPosition.y - Time.deltaTime, -15, -5), Elevator.transform.localPosition.z);
    }
}
