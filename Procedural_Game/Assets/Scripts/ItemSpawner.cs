using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public List<GameObject> items;
    void Start()
    {
        int randomItem = Random.Range(0, items.Count);
        int spawnchance = Random.Range(0, 100);
        if (spawnchance <= 20)
        {
            GameObject Item = Instantiate(items[randomItem], transform.position, transform.rotation);
            Item.transform.SetParent(transform.parent);
        }
        Destroy(gameObject);
    }
}
