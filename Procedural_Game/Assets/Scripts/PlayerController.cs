using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float BaseSpeed;
	public float Speed;
	public float CrouchSpeed;
	public float RunSpeed;
	private bool Crouched;
	private bool WantToStand;
	public float JumpStrength;

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
		//Sprinting speed
		if (Input.GetKey(KeyCode.LeftShift) && !Crouched)
		{
			Speed = RunSpeed;
		}
		//Walking speed
		if (!Input.GetKey(KeyCode.LeftShift) && !Crouched && (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0))
		{
			Speed = BaseSpeed;
		}
		//Crouching speed
		if (!Input.GetKey(KeyCode.LeftShift) && Crouched && (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0))
		{
			Speed = CrouchSpeed;
		}
	}

	void Crouching()
	{
		//Crouching
		if (Crouched)
		{
			Controller.height -= 2f * Time.deltaTime;
			Camera.main.transform.localPosition = new Vector3(0, Mathf.Clamp(Camera.main.transform.localPosition.y - 1.2f * Time.deltaTime,0.3f, 0.83f), 0);
		}
		else
		{
			if (Camera.main.transform.localPosition.y >= 0.83f + 0.25f)
			{
				Controller.height = 2;
				Camera.main.transform.localPosition = new Vector3(0, 0.83f, 0);
			}
			if (Controller.height < 2)
			{
				Camera.main.transform.localPosition = new Vector3(0, Mathf.Clamp(Camera.main.transform.localPosition.y + 2f * Time.deltaTime,0.3f, 0.83f + 0.25f), 0);
			}
		}
		
		Controller.height = Mathf.Clamp(Controller.height, 1.5f, 2f);
		
		Ray roofCheckRay = new Ray(Camera.main.transform.position, Vector3.up);

		if (Input.GetKey(KeyCode.C))
		{
			Crouched = true;
		}
		if (Input.GetKeyUp(KeyCode.C))
		{
			if (!Physics.Raycast(roofCheckRay, 0.8f))
			{
				Crouched = false;
			}
			else
			{
				WantToStand = true;
			}
		}
		if (WantToStand && !Physics.Raycast(roofCheckRay, 0.8f))
		{
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
		if (Input.GetKeyDown(KeyCode.Space) && Grounded)
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
