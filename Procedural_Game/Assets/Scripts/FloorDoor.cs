using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorDoor : MonoBehaviour
{
    public bool isOpen;

    void Update()
    {
        if (isOpen && transform.localPosition.y > -10)
        {
            transform.localPosition -= new Vector3(0,Time.deltaTime * 5,0);
        }
		if (!isOpen && transform.localPosition.y < 0)
		{
			transform.localPosition += new Vector3(0, Time.deltaTime * 5, 0);
		}
	}
}
