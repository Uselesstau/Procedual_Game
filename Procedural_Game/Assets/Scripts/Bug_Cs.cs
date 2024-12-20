using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bug_Cs : MonoBehaviour
{
	private GameObject player;
	public float hp = 5;

	private void Start()
	{
		player = GameObject.Find("Player");
	}
	void Update()
	{
		if (hp <= 0)
		{
			Destroy(gameObject);
		}
		RaycastHit hit;
		bool player_in_view = false;
		if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hit, Mathf.Infinity) && hit.collider.name == "PlayerObj")
		{
			player_in_view = true;
		}
		if (player_in_view)
		{
			transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
			MeshRenderer[] a = GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer b in a)
			{
				b.enabled = true;
			}
			GetComponent<NavMeshAgent>().SetDestination(player.transform.position);
		}
		if (!GetComponent<NavMeshAgent>().hasPath && !player_in_view)
		{
			NavMeshHit navhit;
			if (NavMesh.SamplePosition(new Vector3(Random.Range(-100, 100), transform.position.y, Random.Range(-100, 100)), out navhit, Mathf.Infinity, NavMesh.AllAreas))
			{
				GetComponent<NavMeshAgent>().SetDestination(navhit.position);
			}
		}
	}
}
