using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAudio : MonoBehaviour
{
    void Start()
    {
        GetComponent<AudioSource>().Play();
    }
	private void Update()
	{
		if (!GetComponent<AudioSource>().isPlaying)
		{
			Destroy(gameObject);
		}
	}
}
