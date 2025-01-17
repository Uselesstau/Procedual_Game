using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonOpenDoor : MonoBehaviour
{
    public GameObject Door;
    public Light elevatorLight;
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
	    if (!elevatorLight.enabled)
	    {
		    return;
	    }
        if (generateNextFloor)
        {
			Grid.grid.Clear();
			Grid.grid.AddRange(Grid.elevatorGrid);
			Grid.elevatorGrid.Clear();
			Grid.usedDoors.Clear();
			Instantiate(roomSpawner, orientation.transform.position, orientation.transform.rotation);
        }
		GetComponent<Animator>().SetBool("Press", !GetComponent<Animator>().GetBool("Press"));
        Door.GetComponent<FloorDoor>().isOpen = !Door.GetComponent<FloorDoor>().isOpen;
        Destroy(this);
	}
}
