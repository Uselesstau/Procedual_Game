using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public List<GameObject> items;
    public int spawnOdds;
    void Start()
    {
        int randomItem = Random.Range(0, items.Count);
        int spawnChance = Random.Range(0, 100);
        if (spawnChance <= spawnOdds)
        {
            GameObject item = Instantiate(items[randomItem], transform.position, transform.rotation);
            item.transform.SetParent(transform.parent);
        }
        Destroy(gameObject);
    }
}
