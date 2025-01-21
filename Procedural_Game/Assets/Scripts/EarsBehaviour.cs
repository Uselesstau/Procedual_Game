using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EarsBehaviour : MonoBehaviour
{
    private NavMeshAgent _agent;
    
    private GameObject _player;
    private PlayerScript _playerScript;
    private PlayerController _playerController;
    
    private readonly float _runSpeed = 12.0f;
    private readonly float _baseSpeed = 2.0f;
    
    private Vector3 _soundPosition;

    private float _walkTime;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.Find("Player");
        _playerScript = _player.GetComponent<PlayerScript>();
        _playerController = _player.GetComponent<PlayerController>();
    }

    void Update()
    {
        _walkTime -= Time.deltaTime * _agent.speed;
        if (_walkTime <= 0)
        {
            _walkTime = 4;
            GetComponentInChildren<WalkingAudio_Cs>().Walk();
        }
        
        //Check if player has shot
        if (_playerScript.shootCooldown > 0)
        {
            _soundPosition = _player.transform.position;
            _agent.speed = _runSpeed;
            _agent.SetDestination(_soundPosition);
            return;
        }

        //Check if player is running nearby
        if (Mathf.Approximately(_playerController.Speed, _playerController.RunSpeed))
        {
            float dist = Vector3.Distance(_player.transform.position, transform.position);
            if (dist < 30)
            {
                _soundPosition = _player.transform.position;
                _agent.speed = _runSpeed;
                _agent.SetDestination(_soundPosition);
                return;
            }
        }
        
        //Check if player is walking nearby
        if (Mathf.Approximately(_playerController.Speed, _playerController.BaseSpeed))
        {
            float dist = Vector3.Distance(_player.transform.position, transform.position);
            if (dist < 10)
            {
                _soundPosition = _player.transform.position;
                _agent.speed = _runSpeed;
                _agent.SetDestination(_soundPosition);
                return;
            }
        }

        //Wander if there is no target
        if (!_agent.hasPath)
        {
            _agent.speed = _baseSpeed;
            NavMeshHit navhit;
            if (NavMesh.SamplePosition(new Vector3(Random.Range(-100, 100), transform.position.y, Random.Range(-100, 100)), out navhit, Mathf.Infinity, NavMesh.AllAreas))
            {
                _agent.SetDestination(navhit.position);
            }
        }
    }
}
