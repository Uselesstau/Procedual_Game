using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.XR;
using Debug = System.Diagnostics.Debug;

public class EyesBehaviour : MonoBehaviour
{
	private NavMeshAgent _agent;
	private CapsuleCollider _collider;
	
    private GameObject _player;
	private GameObject _playerObj;
	private ColorGrading _cg;
	private Camera _camera;
	
    private float _baseSpeed = 3;
    private float _maxSpeed = 10;
    
	public float runAway;
	private bool _relocate;
	private void Start()
	{
		_camera = Camera.main;
		_cg = _camera.gameObject.GetComponent<PostProcessVolume>().profile.GetSetting<ColorGrading>();
		_collider = GetComponent<CapsuleCollider>();
		_agent = GetComponent<NavMeshAgent>();
        _player = GameObject.Find("Player");
		_playerObj = GameObject.Find("PlayerObj");
	}
	void Update()
    {
        RaycastHit hit;
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(_camera);
        transform.LookAt(new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z));
		if (runAway == 0)
		{
			if (!_collider.enabled)
			{
				_collider.enabled = true;
				GetComponentInChildren<AudioSource>().Play();
			}
			MeshRenderer[] a = GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer b in a)
			{
				b.enabled = true;
			}
			if (GeometryUtility.TestPlanesAABB(planes, _collider.bounds) && Physics.Raycast(transform.position,  _player.transform.position - transform.position, out hit, Mathf.Infinity) && hit.collider.gameObject == _playerObj && _cg.active)
			{
				_agent.speed = 0;
				_agent.SetDestination(transform.position - new Vector3(0, _agent.height/2,0));
				return;
			}
			_agent.speed = _baseSpeed;
			_agent.SetDestination(_player.transform.position);
		}
		
		if (runAway > 0)
		{
			runAway = Mathf.Clamp(runAway -= Time.deltaTime, 0, 1000);
			_collider.enabled = false;
			MeshRenderer[] a = GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer b in a)
			{
				b.enabled = false;
			}
			_agent.speed = _maxSpeed;
			NavMeshHit navhit;
			if (NavMesh.SamplePosition(new Vector3(Random.Range(-100, 100), transform.position.y, Random.Range(-100, 100)), out navhit, Mathf.Infinity, NavMesh.AllAreas) && runAway > 5f)
			{
				_agent.SetDestination(navhit.position);
			}
		}
    }
}
