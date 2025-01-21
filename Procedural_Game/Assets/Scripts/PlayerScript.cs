using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
	private Camera _playerCamera;
	private PlayerController _playerController;
	
	[Header("HUD")]
	public GameObject canGrabIcon;
	public GameObject batteryMeter;
	public GameObject nightVisionMeter;
	public TextMeshProUGUI bulletCount;
	public float nightVisionPercent = 100.0f;
	public GameObject flashBangEffect;
	public float flashedTime;
	private Vector3 lastMousePos;

	[Header("Oxygen")]
	public GameObject oxygenMeter;
    public float maxOxygen = 100;
    public float oxygenLeft;
    public int lossChance = 5;
    public int lossRateMax = 10;
    private float _currentRate;

    [Header("Items")] 
    public List<int> keyCards;
    public GameObject flareItem;
    public GameObject flareUVItem;
    public GameObject flashBangItem;
    public List<GameObject> inventory;

	[Header("Gun")]
	public AudioClip flashLightOn;
	public AudioClip flashLightOff;
	public GameObject gunShot;
	public bool isLight;
	private bool _aiming;
	private bool _canShoot = true;
	private bool _canReload = true;
	private bool _canAim = true;
	public float reloadingTime;
	private float _recoilTime;
	private float _camFOV = 90;
	public float flashLightBattery = 100;
	private float _rateOfFire = 0.1f;
	public float shootCooldown;
	private float _firstShotAngleY;
	private float _firstShotAngleX;
	private bool _isRifle;
	private bool _isShotgun;
	private bool _isPistol;
	public List<int> weapons;
	private int _selectedIndex;

	[Header("Rifle")]
	public GameObject rifle;
    public GameObject muzzleR;
	public GameObject frontSightR;
	public Animation muzzleFlashR;
	public GameObject bulletR;
	public Light flashLightR;
    public Animator weaponAnimatorR;
	public Animator magazineR;
	public Animator armR;
	public int totalBulletsR;
	public int bulletsInMagR;
	
	[Header("ShotGun")]
	public GameObject shotgun;
	public GameObject muzzleS;
	public GameObject frontSightS;
	public Animation muzzleFlashS;
	public GameObject bulletS;
	public Light flashLightS;
	public Animator weaponAnimatorS;
	public Animator armS;
	public int totalBulletsS;
	public int bulletsInMagS;
	
	[Header("Pistol")]
	public GameObject pistol;
	public GameObject muzzleP;
	public GameObject frontSightP;
	public Animation muzzleFlashP;
	public GameObject bulletP;
	public Light flashLightP;
	public Animator weaponAnimatorP;
	public Animator magazineP;
	public Animator armP;
	public GameObject handP;
	private GameObject _heldItem;
	public int totalBulletsP;
	public int bulletsInMagP;
	void Start()
    {
	    _playerCamera = Camera.main;
	    _playerController = GetComponent<PlayerController>();
        oxygenLeft = maxOxygen;
        _currentRate = lossRateMax;
        weapons.Add(0);
    }
	private void FixedUpdate()
	{
		int layerMask = 1 << LayerMask.NameToLayer("Items");
		int layerMask2 = 1 << LayerMask.NameToLayer("Buttons");
		if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.TransformDirection(Vector3.forward), 1.5f, layerMask) || Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.TransformDirection(Vector3.forward), 1.5f, layerMask2))
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
		_playerCamera.fieldOfView = _camFOV;
        _currentRate -= Time.deltaTime * _playerController.Speed;
        oxygenMeter.transform.localScale = new Vector3(1, oxygenLeft / maxOxygen, 1);
		batteryMeter.transform.localScale = new Vector3(1, flashLightBattery / 100, 1);
		nightVisionMeter.transform.localScale = new Vector3(1, nightVisionPercent / 100, 1);
		if (_isRifle)
		{
			bulletCount.text = bulletsInMagR + " / " + totalBulletsR;
		}
		if (_isShotgun)
		{
			bulletCount.text = bulletsInMagS + " / " + totalBulletsS;
		}
		if (_isPistol)
		{
			bulletCount.text = bulletsInMagP + " / " + totalBulletsP;
		}
		flashLightBattery = Mathf.Clamp(flashLightBattery,0,100);
		oxygenLeft = Mathf.Clamp(oxygenLeft, 0, maxOxygen);
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

		if (flashedTime > 0)
		{
			flashBangEffect.GetComponent<Image>().color = new Color(1, 1, 1, flashedTime/5);
			flashedTime -= Time.deltaTime;
		}
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
			if (_isRifle)
			{
				flashLightR.enabled = true;
			}
			if (_isShotgun)
			{
				flashLightS.enabled = true;
			}
			if (_isPistol)
			{
				flashLightP.enabled = true;
			}
		}
		else
		{
			if (_isRifle)
			{
				flashLightR.enabled = false;
			}
			if (_isShotgun)
			{
				flashLightS.enabled = false;
			}
			if (_isPistol)
			{
				flashLightP.enabled = false;
			}
		}
		
		Items();
		Buttons();
		Gun();
		AimingAnimation();
		NightVision();
	}

	void NightVision()
	{
		nightVisionPercent = Mathf.Clamp(nightVisionPercent, 0, 100);
		ColorGrading cg = _playerCamera.gameObject.GetComponent<PostProcessVolume>().profile.GetSetting<ColorGrading>();
		Grain grain = _playerCamera.gameObject.GetComponent<PostProcessVolume>().profile.GetSetting<Grain>();
		
		if (nightVisionPercent <= 0)
		{
			GetComponent<Light>().enabled = false;
			cg.active = false;
			grain.active = false;
		}
		
		if (Input.GetKeyDown(KeyCode.T) && nightVisionPercent > 0)
		{
			GetComponent<Light>().enabled = !GetComponent<Light>().enabled;
			cg.active = !cg.active;
			grain.active = !grain.active;
		}

		if (cg.active)
		{
			nightVisionPercent -= Time.deltaTime * 5f;
			_playerCamera.cullingMask = LayerMask.GetMask("Default", "Ignore Raycast", "Water", "UI", "Items", "Buttons", "Bullet", "UV");
		}
		else
		{
			_playerCamera.cullingMask = LayerMask.GetMask("Default", "Ignore Raycast", "Water", "UI", "Items", "Buttons", "Bullet");
		}
	}

    void AimingAnimation()
    {
		if (Input.GetMouseButton(1) && _canAim)
		{
			if (_isRifle)
			{
				weaponAnimatorR.SetBool("isAiming", true);
				if (weaponAnimatorR.GetCurrentAnimatorStateInfo(0).IsName("RifleEndAiming"))
				{
					weaponAnimatorR.Play("RifleStartAim", 0, 1 - weaponAnimatorR.GetCurrentAnimatorStateInfo(0).normalizedTime);
				}
			}
			
			if (_isShotgun)
			{
				weaponAnimatorS.SetBool("isAiming", true);
				if (weaponAnimatorS.GetCurrentAnimatorStateInfo(0).IsName("AimingStop_ShotGun"))
				{
					weaponAnimatorS.Play("AimStart_ShotGun", 0, 1 - weaponAnimatorS.GetCurrentAnimatorStateInfo(0).normalizedTime);
				}
			}

			if (_isPistol)
			{
				weaponAnimatorP.SetBool("isAiming", true);
				if (weaponAnimatorP.GetCurrentAnimatorStateInfo(0).IsName("AimingStop_Pistol"))
				{
					weaponAnimatorP.Play("AimStart_Pistol", 0, 1 - weaponAnimatorP.GetCurrentAnimatorStateInfo(0).normalizedTime);
				}
			}
			_canReload = false;
			_camFOV -= Time.deltaTime * 90;
		}
		else
		{
			if (_isRifle)
			{
				weaponAnimatorR.SetBool("isAiming", false);
				if (weaponAnimatorR.GetCurrentAnimatorStateInfo(0).IsName("RifleStartAim"))
				{
					weaponAnimatorR.Play("RifleEndAiming", 0, 1 - weaponAnimatorR.GetCurrentAnimatorStateInfo(0).normalizedTime);
				}
			}

			if (_isShotgun)
			{
				weaponAnimatorS.SetBool("isAiming", false);
				if (weaponAnimatorS.GetCurrentAnimatorStateInfo(0).IsName("AimStart_ShotGun"))
				{
					weaponAnimatorS.Play("AimingStop_ShotGun", 0, 1 - weaponAnimatorS.GetCurrentAnimatorStateInfo(0).normalizedTime);
				}
			}
			
			if (_isPistol)
			{
				weaponAnimatorP.SetBool("isAiming", false);
				if (weaponAnimatorP.GetCurrentAnimatorStateInfo(0).IsName("AimStart_Pistol"))
				{
					weaponAnimatorP.Play("AimingStop_Pistol", 0, 1 - weaponAnimatorP.GetCurrentAnimatorStateInfo(0).normalizedTime);
				}
			}
			_camFOV += Time.deltaTime * 90;
		}

		if (_isRifle && weaponAnimatorR.GetCurrentAnimatorStateInfo(0).IsName("RifleIdle"))
		{
			_canReload = true;
		}
		if (_isShotgun && weaponAnimatorS.GetCurrentAnimatorStateInfo(0).IsName("Idle_ShotGun"))
		{
			_canReload = true;
		}
		if (_isPistol && weaponAnimatorP.GetCurrentAnimatorStateInfo(0).IsName("Idle_Pistol"))
		{
			_canReload = true;
		}
	}
	void Gun()
	{
		//Limit the index
		if (_selectedIndex <= -1)
		{
			_selectedIndex = weapons.Count - 1;
		}
		if (_selectedIndex > weapons.Count - 1)
		{
			_selectedIndex = 0;
		}

		//Swap weapons
		if (weapons[_selectedIndex] == 0)
		{
			_isRifle = false;
			_isPistol = true;
			_isShotgun = false;
		}
		if (weapons[_selectedIndex] == 1)
		{
			_isRifle = true;
			_isPistol = false;
			_isShotgun = false;
		}
		if (weapons[_selectedIndex] == 2)
		{
			_isRifle = false;
			_isPistol = false;
			_isShotgun = true;
		}
		
		//Check Properties of the weapon
		if (_isRifle)
		{
			rifle.gameObject.SetActive(true);
			pistol.gameObject.SetActive(false);
			shotgun.gameObject.SetActive(false);
			_rateOfFire = 0.1f;
		}
		if (_isShotgun)
		{
			rifle.gameObject.SetActive(false);
			pistol.gameObject.SetActive(false);
			shotgun.gameObject.SetActive(true);
			_rateOfFire = 1f;
			if (shootCooldown <= 0)
			{
				weaponAnimatorS.SetBool("shot", false);
				armS.SetBool("shot", false);
			}
		}
		if (_isPistol)
		{
			rifle.gameObject.SetActive(false);
			pistol.gameObject.SetActive(true);
			shotgun.gameObject.SetActive(false);
			_rateOfFire = 0.2f;
			if (shootCooldown <= 0)
			{
				weaponAnimatorP.SetBool("shot", false);
			}
		}
		shootCooldown -= Time.deltaTime;

		//Scroll to change weapons
		if (_canShoot && shootCooldown <= 0 && Input.mouseScrollDelta.y > 0 && _heldItem == null)
		{
			_selectedIndex++;
		}
		if (_canShoot && shootCooldown <= 0 && Input.mouseScrollDelta.y < 0 && _heldItem == null)
		{
			_selectedIndex--;
		}
		
		//Shooting Logic
		if (Input.GetMouseButton(0) && _canShoot && shootCooldown <= 0 && _isRifle)
		{
			shootCooldown = _rateOfFire;
			if (bulletsInMagR == 0)
			{
				//Play audio
				return;
			}
			Instantiate(gunShot, muzzleR.transform);
			GameObject b = Instantiate(bulletR, frontSightR.transform.position, muzzleR.transform.rotation);
			b.GetComponent<bulletScript>().isplayers = true;
			b.GetComponent<bulletScript>().shooter = GameObject.Find("PlayerObj");
			Recoil();
			muzzleFlashR.Play();
			bulletsInMagR--;
		}
		
		if (Input.GetMouseButtonDown(0) && _canShoot && shootCooldown <= 0 && _isShotgun)
		{
			shootCooldown = _rateOfFire;
			if (bulletsInMagS == 0)
			{
				//Play audio
				return;
			}
			weaponAnimatorS.SetBool("shot", true);
			armS.SetBool("shot", true);
			Instantiate(gunShot, muzzleS.transform);
			for (int i = 0; i < 20; i++)
			{
				GameObject b = Instantiate(bulletS, frontSightS.transform.position, muzzleS.transform.rotation);
				b.GetComponent<bulletScript>().isplayers = true;
				b.GetComponent<bulletScript>().shooter = GameObject.Find("PlayerObj");
				b.transform.eulerAngles += new Vector3(Random.Range(-5,5), Random.Range(-5,5), 0);
			}
			Recoil();
			muzzleFlashS.Play();
			bulletsInMagS--;
		}

		if (Input.GetMouseButtonDown(0) && _canShoot && shootCooldown <= 0 && _isPistol)
		{
			shootCooldown = _rateOfFire;
			if (bulletsInMagP == 0)
			{
				//Play audio
				return;
			}
			weaponAnimatorP.SetBool("shot", true);
			Instantiate(gunShot, muzzleP.transform);
			GameObject b = Instantiate(bulletP, frontSightP.transform.position, muzzleP.transform.rotation);
			b.GetComponent<bulletScript>().isplayers = true;
			b.GetComponent<bulletScript>().shooter = GameObject.Find("PlayerObj");
			Recoil();
			muzzleFlashP.Play();
			bulletsInMagP--;
		}
		
		//Reload Logic
		if (Input.GetKeyDown(KeyCode.R) && _canReload && _heldItem == null)
		{
			if (_isRifle)
			{
				reloadingTime = 3;
			}
			if (_isShotgun)
			{
				if (bulletsInMagS == 5 || totalBulletsS == 0)
				{
					return;
				}
				reloadingTime = 999;
			}
			if (_isPistol)
			{
				reloadingTime = 1;
			}
			_canShoot = false;
			_canReload = false;
			_canAim = false;
			
			if (_isRifle)
			{
				magazineR.SetBool("reload", true);
				armR.SetBool("reload", true);
			}

			if (_isShotgun)
			{
				armS.SetBool("reload", true);
				if (bulletsInMagS < 4)
				{
					armS.SetBool("multipleReload", true);
				}
			}
			if (_isPistol)
			{
				magazineP.SetBool("reload", true);
				armP.SetBool("reload", true);
			}
		}

		//Shotgun reload animation logic
		if (Input.GetMouseButtonDown(0) && _isShotgun && !_canReload)
		{
			armS.SetBool("multipleReload", false);
		}

		if (_isShotgun && armS.GetBool("multipleReload") && bulletsInMagS == 5)
		{
			armS.SetBool("reload", false);
			armS.SetBool("multipleReload", false);
		}
		
		//General reload logic
		if (reloadingTime > 0)
		{
			reloadingTime -= Time.deltaTime;
		}
		if (reloadingTime < 0)
		{
			reloadingTime = 0;
			if (_isRifle)
			{
				magazineR.SetBool("reload", false);
				armR.SetBool("reload", false);
			}
			if (_isPistol)
			{
				magazineP.SetBool("reload", false);
				armP.SetBool("reload", false);
			}
			_canShoot = true;
			_canAim = true;
			_canReload = true;
			
			//Reload if Rifle
			if (totalBulletsR >= 30 - bulletsInMagR && _isRifle)
			{
				totalBulletsR -= 30 - bulletsInMagR;
				bulletsInMagR += 30 - bulletsInMagR;
			}
			if (totalBulletsR < 30 - bulletsInMagR && _isRifle)
			{
				bulletsInMagR += totalBulletsR;
				totalBulletsR = 0;
			}
			//Reload if pistol
			if (totalBulletsP >= 8 - bulletsInMagP && _isPistol)
			{
				totalBulletsP -= 8 - bulletsInMagP;
				bulletsInMagP += 8 - bulletsInMagP;
			}
			if (totalBulletsP < 8 - bulletsInMagP && _isPistol)
			{
				bulletsInMagP += totalBulletsP;
				totalBulletsP = 0;
			}
		}
	}
	
	void Recoil()
	{
		float multiplier = _isRifle ? 1.5f : _isShotgun ? 10f : 3f;
		float rand = Random.Range(0.5f, 1.4f);
		_playerCamera.transform.GetComponent<PlayerCamera>().rotation.y += rand * multiplier;
		_playerCamera.transform.GetComponent<PlayerCamera>().rotation.x += rand * multiplier * Random.Range(-0.2f, 0.67f);
	}
	// ReSharper disable Unity.PerformanceAnalysis
	void Items()
	{
		RaycastHit hit;
		int layerMask = 1 << LayerMask.NameToLayer("Items");
		if (Input.GetKeyDown(KeyCode.E) && Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.TransformDirection(Vector3.forward), out hit, 1.5f, layerMask))
		{
			ItemEffect item = hit.collider.GetComponent<ItemEffect>();
			flashLightBattery += item.battery;
			totalBulletsR += item.bulletsR;
			totalBulletsP += item.bulletsP;
			totalBulletsS += item.bulletsS;
			oxygenLeft += item.oxygen;
			nightVisionPercent += item.nVBattery;
			if (item.keyCode != 0)
			{
				keyCards.Add(item.keyCode);
			}

			if (item.isFlare)
			{
				if (inventory.Count == 5)
				{
					return;
				}
				inventory.Add(flareItem);
			}
			
			if (item.isUVFlare)
			{
				if (inventory.Count == 5)
				{
					return;
				}
				inventory.Add(flareUVItem);
			}
			
			if (item.isFlashBang)
			{
				if (inventory.Count == 5)
				{
					return;
				}
				inventory.Add(flashBangItem);
			}

			if (item.isRifle)
			{
				if (weapons.Contains(1))
				{
					totalBulletsR += 30;
				}
				else
				{
					weapons.Add(1);
				}
			}
			if (item.isShotgun)
			{
				if (weapons.Contains(2))
				{
					totalBulletsR += 5;
				}
				else
				{
					weapons.Add(2);
				}
			}
			
			Destroy(hit.collider.gameObject);
		}
		
		if (Input.GetKeyDown(KeyCode.G) && _isPistol && _canAim && _canReload && inventory.Count > 0 && inventory[0].GetComponent<FlareItem>() != null)
		{
			GameObject flare = Instantiate(inventory[0], handP.transform.position, handP.transform.rotation);
			flare.transform.SetParent(handP.transform);
			flare.GetComponent<Rigidbody>().isKinematic = true;
			_heldItem = flare;
			armP.SetBool("flare", true);
			inventory.Remove(inventory[0]);
			_canReload = false;
			_canAim = false;
			return;
		}
		
		if (Input.GetKeyDown(KeyCode.G) && inventory.Count > 0 && _heldItem == null)
		{
			GameObject item = Instantiate(inventory[0], transform.position + _playerCamera.transform.forward, Quaternion.identity);
			item.GetComponent<Rigidbody>().velocity = _playerCamera.transform.forward * 10;
			inventory.Remove(inventory[0]);
		}

		if (Input.GetKeyDown(KeyCode.G) && _heldItem != null)
		{
			_heldItem.transform.SetParent(null);
			_heldItem.GetComponent<Rigidbody>().isKinematic = false;
			_heldItem.GetComponent<Rigidbody>().velocity = _playerCamera.transform.forward * 10;
			_heldItem = null;
		}

		if (armP.GetBool("flare") && _heldItem == null)
		{
			armP.SetBool("flare", false);
			_canReload = true;
			_canAim = true;
		}
	}
	// ReSharper disable Unity.PerformanceAnalysis
	void Buttons()
	{
		RaycastHit hit;
		int layerMask = 1 << LayerMask.NameToLayer("Buttons");
		if (Input.GetKeyDown(KeyCode.E) && Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.TransformDirection(Vector3.forward), out hit, 1.5f, layerMask))
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
			if (button.GetComponent<MaintenanceButton>() != null)
			{
				button.GetComponent<MaintenanceButton>().OnPress();
			}
		}
	}
}
