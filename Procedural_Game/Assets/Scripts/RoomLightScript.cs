using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLightScript : MonoBehaviour
{
    public bool forceOn;
    public Transform player;
    private float _maxRange;
	void Start()
    {
	    player = GameObject.FindGameObjectWithTag("Player").transform;
	    _maxRange = GetComponent<Light>().range;
	    
        GetComponent<Animator>().Update(Random.Range(0, 16f));
        int isOn = Random.Range(0, 4);
        if (isOn == 0)
        {
	        GetComponent<Light>().enabled = true;
		}
    }

	private void Update()
	{
		Light objLight = GetComponent<Light>();
		if (forceOn)
		{
			objLight.enabled = true;
		}

		if (objLight.enabled)
		{
			if (!GetComponent<AudioSource>().isPlaying)
			{
				GetComponent<AudioSource>().Play();
			}
		}
		else
		{
			GetComponent<AudioSource>().Stop();
		}
		
		float dist = Vector3.Distance(transform.position, player.position);
		float rangeMultiplier = Mathf.Clamp(20.0f / dist, 0f, 1.0f);
		
		if (dist > 50)
		{
			rangeMultiplier = 0;
		}

		objLight.range = _maxRange * rangeMultiplier;
	}
}
