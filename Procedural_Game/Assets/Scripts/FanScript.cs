using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanScript : MonoBehaviour
{
    private float fanSpeed = 30;
	private void Start()
	{
		transform.localEulerAngles = new Vector3(0, Random.Range(0,360),0);
	}
	void Update()
    {
        transform.localEulerAngles += new Vector3(0, fanSpeed * Time.deltaTime, 0);
        Mathf.Clamp(transform.localEulerAngles.y, 0, 360);
	}
}
