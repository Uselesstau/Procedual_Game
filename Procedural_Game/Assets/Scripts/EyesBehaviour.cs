using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;

public class EyesBehaviour : MonoBehaviour
{
    public GameObject player;
	public GameObject playerObj;
    public float speed;
	public float runAway;
	public bool relocate;
	private void Start()
	{
        player = GameObject.Find("Player");
		playerObj = GameObject.Find("PlayerObj");
	}
	void Update()
    {
        RaycastHit hit;
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
		float dist = Vector3.Distance(transform.position, player.transform.position);
		if (runAway == 0)
		{
			if (!GetComponent<CapsuleCollider>().enabled)
			{
				GetComponent<CapsuleCollider>().enabled = true;
				GetComponentInChildren<AudioSource>().Play();
			}
			MeshRenderer[] a = GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer b in a)
			{
				b.enabled = true;
			}
			if (GeometryUtility.TestPlanesAABB(planes, gameObject.GetComponent<CapsuleCollider>().bounds) && Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
			{
				if (!relocate)
				{
					if (dist <= 10)
					{
						runAway = 5;
					}
					relocate = true;
				}

				if (hit.collider.gameObject == playerObj)
				{
					if (dist <= 15)
					{
						runAway = 15;
					}
					GetComponent<NavMeshAgent>().speed = 0;
					GetComponent<NavMeshAgent>().SetDestination(transform.position);
				}
				else
				{
					GetComponent<NavMeshAgent>().speed = speed;
					GetComponent<NavMeshAgent>().SetDestination(player.transform.position);
				}
			}
			else
			{
				GetComponent<NavMeshAgent>().speed = speed;
				GetComponent<NavMeshAgent>().SetDestination(player.transform.position);
				relocate = false;
			}
		}
		if (runAway > 0)
		{
			runAway = Mathf.Clamp(runAway -= Time.deltaTime, 0, 1000);
			GetComponent<CapsuleCollider>().enabled = false;
			MeshRenderer[] a = GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer b in a)
			{
				b.enabled = false;
			}
			GetComponent<NavMeshAgent>().speed = 10;
			NavMeshHit navhit;
			if (NavMesh.SamplePosition(new Vector3(Random.Range(-100, 100), transform.position.y, Random.Range(-100, 100)), out navhit, Mathf.Infinity, NavMesh.AllAreas) && runAway > 5f)
			{
				GetComponent<NavMeshAgent>().SetDestination(navhit.position);
			}

		}
    }
}
