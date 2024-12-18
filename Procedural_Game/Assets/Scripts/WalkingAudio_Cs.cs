using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingAudio_Cs : MonoBehaviour
{
    public List<AudioClip> steps;
    public bool automated;
	private void Update()
	{
		if (!GetComponent<AudioSource>().isPlaying && automated)
		{
			int source = Random.Range(0, steps.Count);
			GetComponent<AudioSource>().clip = steps[source];
			GetComponent<AudioSource>().Play();
		}
	}
	public void Walk()
    {
        int source = Random.Range(0, steps.Count);
        GetComponent<AudioSource>().clip = steps[source];
        GetComponent<AudioSource>().Play();
	}
}
