using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class KeyCard : MonoBehaviour
{
    public GameObject keyCardPrefab;
    public GameObject door;
    private GameObject _player;
    public int value;
    public List<GameObject> keySpawners;
    void Start()
    {
        _player = GameObject.Find("Player");
        value = Random.Range(1, 10000);
        
        keySpawners = GameObject.FindGameObjectsWithTag("KeySpawner").ToList();
        GameObject keySpawner = keySpawners[Random.Range(0, keySpawners.Count)];
        GameObject keyCard = Instantiate(keyCardPrefab, keySpawner.transform);
        //Destroy(keySpawner);
        keyCard.GetComponent<ItemEffect>().keyCode = value;
    }

    public void CheckToOpenDoor()
    {
        if (_player.GetComponent<PlayerScript>().keyCards.Contains(value))
        {
            Destroy(door);
            _player.GetComponent<PlayerScript>().keyCards.Remove(value);
        }
    }
}
