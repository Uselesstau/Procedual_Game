using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieBehaviour : MonoBehaviour
{
	private GameObject _player;
	private NavMeshAgent _agent;
	private Rigidbody _rb;
    	
	public float hp = 3;
	public float stunTime;
	private bool _isDead;
    	
    
	private void Start()
	{
		_player = GameObject.Find("Player");
		_agent = GetComponent<NavMeshAgent>();
		_rb = GetComponent<Rigidbody>();
	}
	void Update()
	{
		if (_isDead) return;
    		
		if (hp <= 0)
		{
			_agent.ResetPath();
			StartCoroutine(Stun());
			_isDead = true;
			return;
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
    
	IEnumerator Stun()
	{
		RagDoll();
		float deadTime = 15;
		while (deadTime > 0)
		{
			deadTime -= Time.deltaTime;
			yield return null;
		}
		Revive();
	}
    
	void RagDoll()
	{
		_agent.isStopped = true;
		_agent.enabled = false;
		_rb.isKinematic = false;
		_rb.velocity = transform.TransformDirection(Vector3.forward) * (10 * -1);
		GetComponentInChildren<WalkingAudio_Cs>().automated = false;
	}
    
	void Revive()
	{
		transform.LookAt(new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z));
		_agent.enabled = true;
		_agent.isStopped = false;
		_rb.isKinematic = true;
		hp = 6;
		_isDead = false;
		GetComponentInChildren<WalkingAudio_Cs>().automated = true;
	}
}
