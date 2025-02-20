using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Automaton_Behaviour : MonoBehaviour
{
	private GameObject _player;
	private NavMeshAgent _agent;

	public GameObject gunShot;
	public GameObject bullet;
	public Animation muzzleFlash;
	private float _rof = 0.5f;
	private float _coolDown;
	public float stunTime;

	public float hp = 5;
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
		_coolDown -= Time.deltaTime;
		if (stunTime > 0)
		{
			stunTime -= Time.deltaTime;
			return;
		}
		RaycastHit hit;
		bool playerInView = Physics.Raycast(transform.position, _player.transform.position - transform.position, out hit, Mathf.Infinity) && hit.collider.name == "PlayerObj";
		if (playerInView)
		{
			if (_coolDown < 0)
			{
				_coolDown = _rof;
				GameObject b = Instantiate(bullet, transform.position, transform.rotation);
				b.GetComponent<bulletScript>().shooter = gameObject;
				Instantiate(gunShot, transform);
				muzzleFlash.Play();
			}
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
