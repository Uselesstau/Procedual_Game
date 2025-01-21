using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingAudio_Cs : MonoBehaviour
{
	private AudioSource _audioSource;
	
    public List<AudioClip> steps;
    public bool automated;

    void Start()
    {
	    _audioSource = GetComponent<AudioSource>();
    }
	private void Update()
	{
		if (!_audioSource.isPlaying && automated)
		{
			int source = Random.Range(0, steps.Count);
			_audioSource.clip = steps[source];
			_audioSource.pitch = Random.Range(0.8f, 1.2f);
			_audioSource.Play();
		}
	}
	public void Walk()
    {
        int source = Random.Range(0, steps.Count);
        _audioSource.clip = steps[source];
        _audioSource.pitch = Random.Range(0.8f, 1.2f);
        _audioSource.Play();
	}
}
