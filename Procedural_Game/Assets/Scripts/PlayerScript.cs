using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
	[Header("HUD")]
	public GameObject canGrabIcon;
	public GameObject batteryMeter;
	public TextMeshProUGUI bulletCount;

	[Header("Oxygen")]
	public GameObject oxygenMeter;
    public float maxOxygen = 100;
    public float oxygenLeft;
    public int lossChance = 5;
    public int lossRateMax = 10;
    private float _currentRate;

    [Header("Items")] 
    public List<int> keyCards;

	[Header("Gun")]
	public AudioClip flashLightOn;
	public AudioClip flashLightOff;
	public GameObject gunShot;
    public GameObject muzzle;
	public GameObject frontsight;
	public Animation muzzleFlash;
	public GameObject bullet;
	public Light FlashLight;
    public Animator weaponAnimator;
	public Animator magazine;
	public Animator Arm;
	public bool isLight;
    private float _isAimingCoolDown = 1;
    private bool _aiming;
	public int totalBullets;
	public int bulletsInMag;
	private bool _canShoot = true;
	private bool _canReload = true;
	private bool _canAim = true;
	private float _reloadingTime;
	private float _recoilTime;
	private float _recoilAcceleration;
	private float _camFOV = 90;
	public float flashLightBattery = 100;
	private readonly float _rateOfFire = 0.1f;
	private float _shootCooldown;
	private float _firstShotAngleY;
	private float _firstShotAngleX;
	void Start()
    {
        oxygenLeft = maxOxygen;
        _currentRate = lossRateMax;
    }
	private void FixedUpdate()
	{
		int layerMask = 1 << LayerMask.NameToLayer("Items");
		int layerMask2 = 1 << LayerMask.NameToLayer("Buttons");
		if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), 1.5f, layerMask) || Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), 1.5f, layerMask2))
		{
			canGrabIcon.SetActive(true);
		}
		else
		{
			canGrabIcon.SetActive(false);
		}
	}
	void Update()
    {
        _camFOV = Mathf.Clamp(_camFOV, 45, 90);
		Camera.main.fieldOfView = _camFOV;
        _currentRate -= Time.deltaTime * GetComponent<PlayerController>().Speed;
        oxygenMeter.transform.localScale = new Vector3(1, oxygenLeft / maxOxygen, 1);
		batteryMeter.transform.localScale = new Vector3(1, flashLightBattery / 100, 1);
		bulletCount.text = bulletsInMag + " / " + totalBullets;
		flashLightBattery = Mathf.Clamp(flashLightBattery,0,100);
        int randomTick = Random.Range(0, lossChance);
        if (randomTick == 0 && _currentRate <= 0)
        {
            oxygenLeft--;
            _currentRate = lossRateMax;
        }
		if (oxygenLeft <= 0)
		{
			SceneManager.LoadScene("MainMenu");
		}
		AimingAnimation();
		if (Input.GetKeyDown(KeyCode.F))
		{
			isLight = !isLight;
			if (GetComponent<AudioSource>().clip == flashLightOff)
			{
				GetComponent<AudioSource>().clip = flashLightOn;
				GetComponent<AudioSource>().Play();
			}
			else
			{
				GetComponent<AudioSource>().clip = flashLightOff;
				GetComponent<AudioSource>().Play();
			}
		}
		if (isLight && flashLightBattery > 0)
		{
			flashLightBattery -= Time.deltaTime * 0.8f;
			FlashLight.enabled = true;
		}
		else
		{
			FlashLight.enabled = false;
		}
		
		Items();
		Buttons();
		Gun();
	}

    void AimingAnimation()
    {
		if (Input.GetMouseButton(1) && _canAim)
		{
			_aiming = true;
		}
		if (_aiming)
		{
			_isAimingCoolDown -= Time.deltaTime;
			_camFOV -= Time.deltaTime * 90;
			_canReload = false;
		}
		else
		{
			_camFOV += Time.deltaTime * 90;
		}
		if (!Input.GetMouseButton(1) && _isAimingCoolDown <= 0)
		{
			_isAimingCoolDown = 1;
			_aiming = false;
			_canReload = true;
		}
		if (_isAimingCoolDown < 1)
		{
			weaponAnimator.SetBool("isAiming", true);
		}
		else
		{
			weaponAnimator.SetBool("isAiming", false);
		}
	}
	void Gun()
	{
		_shootCooldown -= Time.deltaTime;
		if (Input.GetMouseButtonDown(0) && _canShoot && _shootCooldown <= 0)
		{
			_firstShotAngleY = Camera.main.GetComponent<PlayerCamera>().rotation.y;
			_firstShotAngleX = Camera.main.GetComponent<PlayerCamera>().rotation.x;
		}
		if (Input.GetMouseButton(0) && _canShoot && _shootCooldown <= 0)
		{
			_shootCooldown = _rateOfFire;
			if (bulletsInMag == 0)
			{
				//Play audio
			}
			else
			{
				Instantiate(gunShot, muzzle.transform);
				GameObject b = Instantiate(bullet, frontsight.transform.position, muzzle.transform.rotation);
				b.GetComponent<bulletScript>().isplayers = true;
				b.GetComponent<bulletScript>().shooter = GameObject.Find("PlayerObj");
				Recoil();
				muzzleFlash.Play();
				bulletsInMag--;
			}
		}
		if (_recoilTime > 0 && (!Input.GetMouseButton(0) || bulletsInMag == 0))
		{
			if (Camera.main.GetComponent<PlayerCamera>().rotation.y - _firstShotAngleY <= 0.1f)
			{
				_recoilTime = 0;
				return;
			}
			Camera.main.GetComponent<PlayerCamera>().rotation.y -= (Camera.main.GetComponent<PlayerCamera>().rotation.y - _firstShotAngleY) * Time.deltaTime * 5;
			Camera.main.GetComponent<PlayerCamera>().rotation.x -= (Camera.main.GetComponent<PlayerCamera>().rotation.x - _firstShotAngleX) * Time.deltaTime * 5;
			_recoilTime -= 2 * Time.deltaTime;
		}
		if (Input.GetKeyDown(KeyCode.R) && _canReload)
		{
			_reloadingTime = 3;
			_canShoot = false;
			_canReload = false;
			_canAim = false;
			_isAimingCoolDown = 1;
			magazine.SetBool("reload", true);
			Arm.SetBool("reload", true);
		}
		if (_reloadingTime > 0)
		{
			_reloadingTime -= Time.deltaTime;
		}
		if (_reloadingTime < 0)
		{
			_reloadingTime = 0;
			magazine.SetBool("reload", false);
			Arm.SetBool("reload", false);
			_canShoot = true;
			_canAim = true;
			_canReload = true;

			if (totalBullets >= 30 - bulletsInMag)
			{
				totalBullets -= 30 - bulletsInMag;
				bulletsInMag += 30 - bulletsInMag;
			}
			else
			{
				bulletsInMag += totalBullets;
				totalBullets = 0;
			}
		}
	}
	void Recoil()
	{
		float rand = Random.Range(0.5f, 1.4f);
		Camera.main.transform.GetComponent<PlayerCamera>().rotation.y += rand;
		Camera.main.transform.GetComponent<PlayerCamera>().rotation.x += rand * Random.Range(-0.2f, 0.67f);
		_recoilTime += rand;
	}
	void Items()
	{
		RaycastHit hit;
		int layerMask = 1 << LayerMask.NameToLayer("Items");
		if (Input.GetKeyDown(KeyCode.E) && Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out hit, 1.5f, layerMask))
		{
			flashLightBattery += hit.collider.GetComponent<ItemEffect>().battery;
			totalBullets += hit.collider.GetComponent<ItemEffect>().bullets;
			if (hit.collider.GetComponent<ItemEffect>().keyCode != 0)
			{
				keyCards.Add(hit.collider.GetComponent<ItemEffect>().keyCode);
			}
			Destroy(hit.collider.gameObject);
		}
	}
	void Buttons()
	{
		RaycastHit hit;
		int layerMask = 1 << LayerMask.NameToLayer("Buttons");
		if (Input.GetKeyDown(KeyCode.E) && Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out hit, 1.5f, layerMask))
		{
			GameObject button = hit.collider.gameObject;
			if (button.GetComponent<ButtonOpenDoor>() != null)
			{
				button.GetComponent<ButtonOpenDoor>().OnPress();
			}
			if (button.GetComponent<ButtonUseElevator>() != null)
			{
				button.GetComponent<ButtonUseElevator>().OnPress();
			}
			if (button.GetComponent<KeyCard>() != null)
			{
				button.GetComponent<KeyCard>().CheckToOpenDoor();
			}
		}
	}
}
