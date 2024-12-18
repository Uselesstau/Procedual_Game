using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float BaseSpeed = 1f;
	public float Speed = 1f;
	public float CrouchSpeed = 0.3f;
	public float RunSpeed = 1.5f;
	private bool Crouched = false;
	private bool WantToStand = false;
	public float JumpStrength = 1f;

	private bool Grounded = false;
	public CharacterController Controller;
	private Vector3 CurrentForceVelocity;

	public GameObject walking;
	public float walkSoundCooldown;

	private void Awake()
	{
		Controller.GetComponent<CharacterController>();
	}

	void Update()
	{
		Walking();
		Crouching();
		Jumping();

		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		
		walkSoundCooldown -= Time.deltaTime;
		if (Input.GetAxis("Vertical") == 0 && Input.GetAxis("Horizontal") == 0)
		{
			Speed = 0;
		}

		if (walkSoundCooldown < 0 && Speed != 0 && Grounded)
		{
			walkSoundCooldown = 3 / Speed;
			walking.GetComponent<WalkingAudio_Cs>().Walk();
		}
	}

	void Walking()
	{
		//Basic Movement
		float playerVerticalInput = Input.GetAxis("Vertical");
		float playerHorizontalInput = Input.GetAxis("Horizontal");

		Vector3 forward = Camera.main.transform.forward;
		Vector3 right = Camera.main.transform.right;
		forward.y = 0;
		right.y = 0;
		forward = forward.normalized;
		right = right.normalized;

		Vector3 forwardRelativeVerticalInput = playerVerticalInput * forward;
		Vector3 rightRelativeVerticalInput = playerHorizontalInput * right;

		Vector3 cameraRelativeMovement = forwardRelativeVerticalInput + rightRelativeVerticalInput;
		
		Controller.Move(cameraRelativeMovement * (Speed * Time.deltaTime));
		//Sprinting
		if (Input.GetKey(KeyCode.LeftShift) && Crouched == false)
		{
			Speed = RunSpeed;
		}
		if (!Input.GetKey(KeyCode.LeftShift) && Crouched == false && (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0))
		{
			Speed = BaseSpeed;
		}
	}

	void Crouching()
	{
		//Crouching
		Ray roofCheckRay = new Ray(new Vector3(transform.position.x, transform.position.y - 0.38f, transform.position.z), Vector3.up);
		Debug.DrawRay(new Vector3(transform.position.x, transform.position.y - 0.38f, transform.position.z), Vector3.up);
		if (Input.GetKeyDown(KeyCode.C) && Crouched == false)
		{
			Camera.main.transform.localPosition = new Vector3(0, 0.05f, 0.191f);
			Controller.height = 0.05f;
			Speed = CrouchSpeed;
			Crouched = true;
		}
		if (Input.GetKeyUp(KeyCode.C) && Crouched == true)
		{
			if (!Physics.Raycast(roofCheckRay, 1f))
			{
				Camera.main.transform.localPosition = new Vector3(0, 0.83f, 0.191f);
				Controller.height = 2f;
				Speed = BaseSpeed;
				Crouched = false;
			}
			else
			{
				WantToStand = true;
			}
		}
		if (WantToStand == true && !Physics.Raycast(roofCheckRay, 1f))
		{
			Camera.main.transform.localPosition = new Vector3(0, 0.83f, 0.191f);
			Controller.height = 2f;
			Speed = BaseSpeed;
			Crouched = false;
			WantToStand = false;
		}
	}

	void Jumping()
	{
		//Gravity
		Ray groundCheckRay = new Ray(transform.position, Vector3.down);
		if (Physics.Raycast(groundCheckRay, 1.1f))
		{
			Land(CurrentForceVelocity.y);
			CurrentForceVelocity.y = -1f;
		}
		else
		{
			CurrentForceVelocity.y -= 20f * Time.deltaTime;
			Grounded = false;
		}

		//Jumping
		if (Input.GetKeyDown(KeyCode.Space) && Grounded == true)
		{
			for (int i = 1; i < 30; i++)
			{
				CurrentForceVelocity.y += JumpStrength;
			}
		}

		Controller.Move(CurrentForceVelocity * Time.deltaTime);
	}

	void Land(float velocity)
	{
		Grounded = true;
		if (velocity < -5f)
		{
			walking.GetComponent<WalkingAudio_Cs>().Walk();
		}
	}

}
