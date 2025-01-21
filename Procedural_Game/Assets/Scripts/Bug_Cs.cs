using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bug_Cs : MonoBehaviour
{
	private GameObject _player;
	private NavMeshAgent _agent;
	
	public float hp = 3;
	public float stunTime;

	private void Start()
	{
		_player = GameObject.Find("Player");
		_agent = GetComponent<NavMeshAgent>();
	}
	void Update()
	{
		if (hp <= 0)
		{
			_agent.ResetPath();
			Destroy(gameObject);
		}

		if (stunTime > 0)
		{
			stunTime -= Time.deltaTime;
			return;
		}
		RaycastHit hit;
		bool playerInView = Physics.Raycast(transform.position, _player.transform.position - transform.position, out hit, Mathf.Infinity) && hit.collider.name == "PlayerObj";
		if (playerInView)
		{
			transform.LookAt(new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z));
			MeshRenderer[] a = GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer b in a)
			{
				b.enabled = true;
			}
			_agent.SetDestination(_player.transform.position);
		}
		if (!_agent.hasPath && !playerInView)
		{
			NavMeshHit navhit;
			if (NavMesh.SamplePosition(new Vector3(Random.Range(-100, 100), transform.position.y, Random.Range(-100, 100)), out navhit, Mathf.Infinity, NavMesh.AllAreas))
			{
				_agent.SetDestination(navhit.position);
			}
		}
	}
}
