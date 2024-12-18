using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobbingScript : MonoBehaviour
{
	[Header("References")]
	public GameObject Player;

	[Header("Settings")]
	public float bobSpeed = 4.8f;
	public float bobAmount = 0.05f;

	private Vector3 restPosition;
	private float timer = Mathf.PI / 2;
	private void Start()
	{
		restPosition = transform.localPosition;
	}
	private void Update()
	{
		if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
		{
			timer += bobSpeed * Time.deltaTime * Player.GetComponent<PlayerController>().Speed / 2;

			Vector3 newPosition = transform.rotation * new Vector3(Mathf.Cos(timer) * bobAmount, restPosition.y + Mathf.Abs(Mathf.Sin(timer) * bobAmount), restPosition.z);
			Camera.main.transform.localPosition = newPosition;
		}
		else
		{
			timer += bobSpeed * Time.deltaTime * Player.GetComponent<PlayerController>().Speed / 4;

			Vector3 newPosition = transform.rotation * new Vector3(Mathf.Cos(timer) * bobAmount / 2, restPosition.y + Mathf.Abs(Mathf.Sin(timer) * bobAmount / 2), restPosition.z);
			Camera.main.transform.localPosition = newPosition;
		}

		if (timer > Mathf.PI * 2)
		{
			timer = 0;
		}
	}
}
