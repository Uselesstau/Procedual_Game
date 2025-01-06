using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashBangScript : MonoBehaviour
{
    Light flashBangLight;
    void Start()
    {
        flashBangLight = GetComponentInChildren<Light>();
        flashBangLight.range = 0;
        StartCoroutine(LightUp());
    }

    void FlashEffect()
    {
        List<GameObject> targets = new List<GameObject>();
        targets.Add(GameObject.FindGameObjectWithTag("Player"));
        targets.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        
        RaycastHit hit;
        foreach (GameObject target in targets)
        {
            if (Physics.Raycast(transform.position, target.transform.position - transform.position, out hit))
            {
                if (hit.distance < 20 && targets.Contains(hit.collider.gameObject))
                {
                    if (targets[0] == hit.collider.gameObject)
                    {
                        hit.collider.gameObject.GetComponentInParent<PlayerScript>().flashedTime = (20 - hit.distance) / 4;
                    }

                    if (hit.collider.gameObject.GetComponent<Bug_Cs>() != null)
                    {
                        hit.collider.gameObject.GetComponent<Bug_Cs>().stunTime = (20 - hit.distance) / 4;
                    }
                    if (hit.collider.gameObject.GetComponent<EyesBehaviour>() != null)
                    {
                        hit.collider.gameObject.GetComponent<EyesBehaviour>().runAway = 20 - hit.distance;
                    }
                    if (hit.collider.gameObject.GetComponent<Automaton_Behaviour>() != null)
                    {
                        hit.collider.gameObject.GetComponent<Automaton_Behaviour>().stunTime = (20 - hit.distance) / 4;
                    }
                }
            }
        }
    }

    IEnumerator LightUp()
    {
        yield return new WaitForSeconds(1f);
        FlashEffect();
        while (flashBangLight.range < 20)
        {
            flashBangLight.range++;
            yield return null;
        }
        StartCoroutine(Dim());
        yield return null;
    }
    
    IEnumerator Dim()
    {
        while (flashBangLight.range > 0)
        {
            flashBangLight.range -= Time.deltaTime * 240;
            yield return null;
        }
        Destroy(gameObject);
        yield return null;
    }
}
