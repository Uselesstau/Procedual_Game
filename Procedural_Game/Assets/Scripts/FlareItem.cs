using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FlareItem : MonoBehaviour
{
    private GameObject _player;
    private PlayerScript _playerScript;
    
    public Light flareLight;
    
    public bool isUV;
    public float flareTime = 10.0f;

    void Start()
    {
        _player = GameObject.Find("Player");
        _playerScript = _player.GetComponent<PlayerScript>();
        Physics.IgnoreLayerCollision(9, 3);
    }
    void Update()
    {
        if (isUV)
        {
            flareTime -= Time.deltaTime;
            if (flareTime < 0)
            {
                flareLight.intensity -= Time.deltaTime;
            }
        }

        if (flareLight.intensity <= 0.0f)
        {
            Destroy(gameObject);
        }
    }
}
