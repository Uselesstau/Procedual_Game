using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonOpenDoor : MonoBehaviour
{
    public GameObject Door;
    public bool generateNextFloor;
    public GameObject roomSpawner;
    public GameObject orientation;
    private gridScript Grid;
	private void Start()
	{
		Grid = GameObject.Find("Grid").GetComponent<gridScript>();
	}
	public void OnPress()
    {
        if (generateNextFloor)
        {
			for (int i = 0; i < Grid.grid.Count - 2; i++)
            {
                Grid.grid.RemoveAt(i);
            }
			Instantiate(roomSpawner, orientation.transform.position, orientation.transform.rotation);
        }
		GetComponent<Animator>().SetBool("Press", !GetComponent<Animator>().GetBool("Press"));
        Door.GetComponent<FloorDoor>().isOpen = !Door.GetComponent<FloorDoor>().isOpen;
        Destroy(this);
	}
}
