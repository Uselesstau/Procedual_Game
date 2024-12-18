using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLightScript : MonoBehaviour
{
    [SerializeField] bool forceOn;
	void Start()
    {
        GetComponent<Animator>().Update(Random.Range(0, 16f));
        if (forceOn)
        {
			GetComponent<Light>().enabled = true;
            return;
		}
        int isOn = Random.Range(0, 4);
        if (isOn != 0)
        {
			Destroy(gameObject);
		}
    }

	private void Update()
	{
        if (forceOn)
        {
            return;
        }
        RaycastHit hit;
		if (Physics.Raycast(transform.position, GameObject.Find("Player").transform.position - transform.position, out hit))
        {
			if (hit.collider.name != "PlayerObj")
            {
                GetComponent<Light>().enabled = false;
			}
            else
            {
				GetComponent<Light>().enabled = true;
			}
        }
        float dist = Vector3.Distance(transform.position, GameObject.Find("Player").transform.position);
        if (dist < 20)
        {
			GetComponent<Light>().enabled = true;
		}
	}
}
