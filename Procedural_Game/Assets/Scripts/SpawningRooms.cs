using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class SpawningRooms : MonoBehaviour
{
	public GameObject empty;

	private gridScript _grid;
    public GameObject[] rooms;
	public GameObject[] deadEnds;
	public GameObject endFloor;
	public int currentFloor;
	public int complexity;
    public int spawnCount;
	public List<Transform> doorways;
	public bool branch;

	public List<GameObject> createdRooms;

	public int order;

	public List<GameObject> roomsToCheck;
	public List<GameObject> deadEndsToCheck;

	private int _randomRoom;
	private GameObject _room;

	private GameObject _lastRoom;
	private Transform _lastDoor;
	public List<Vector3> lastGridPositions;

	private bool _wait;

	private void Start()
	{
		_grid = GameObject.Find("Grid").GetComponent<gridScript>();
		_grid.generators.Add(gameObject);
		order = _grid.generators.Count;
		name = order.ToString();
		if (GameObject.Find("SeedManager") != null && CompareTag("Original") && name == "1")
		{
			StartScript info = GameObject.Find("SeedManager").GetComponent<StartScript>();
			_grid.seed = info.seed;
			currentFloor = 1;
			_grid.currentFloor = 1;
			complexity = _grid.complexity;
		}
		if (GameObject.Find("SeedManager") == null && CompareTag("Original"))
		{
			currentFloor = _grid.currentFloor;
			complexity = _grid.complexity;
		}
		if (CompareTag("Original") && name != "1")
		{
			currentFloor = _grid.currentFloor;
			complexity = _grid.complexity;
		}
		roomsToCheck.AddRange(rooms);
		deadEndsToCheck.AddRange(deadEnds);
		spawnCount = Mathf.RoundToInt((complexity + 1) * currentFloor / 6f) + 2;
		foreach (Vector3 v in _grid.usedDoors)
		{
			if (branch && Vector3.Distance(transform.position,v) < 1)
			{
				Destroy(gameObject);
			}
		}
		if (GameObject.Find((currentFloor - 1) + ".") != null && CompareTag("Original"))
		{
			Destroy(GameObject.Find((currentFloor - 1) + "."));
		}
		if (CompareTag("Original"))
		{
			Random.InitState(_grid.seed + int.Parse(name));
		}
		if (CompareTag("Original") && name == "1")
		{
			Spawnroom();
		}
		else
		{
			StartCoroutine(Suspend());
		}
	}
	void Update()
    {
		if (spawnCount <= 0)
		{
			Destroy(gameObject);
		}
		for(int i = 0; i < _grid.grid.Count(); i++)
		{
			_grid.grid[i] = new Vector3(Mathf.Round(_grid.grid[i].x), Mathf.Round(_grid.grid[i].y), Mathf.Round(_grid.grid[i].z));
		}
	}

	IEnumerator Suspend()
	{
		while (GameObject.FindGameObjectWithTag("Original") != null && !CompareTag("Original"))
		{
			if (GameObject.FindGameObjectWithTag("Original").GetComponent<SpawningRooms>().order > order)
			{
				break;
			}
			yield return null;
		}
		while (GameObject.Find((order - 1).ToString()) != null)
		{
			yield return null;
		}
		Spawnroom();
	}

	IEnumerator Wait()
	{
		while (_wait)
		{
			yield return null;
		}
		Spawnroom();
	}

	public void FinishFloor()
	{
		GameObject endFloor = Instantiate(empty);
		endFloor.name = currentFloor.ToString() + ".";
		foreach (GameObject g in createdRooms)
		{
			g.transform.SetParent(endFloor.transform);
		}
		if (GameObject.Find("Start") != null)
		{
			GameObject.Find("Start").transform.SetParent(endFloor.transform);
			GameObject.Find("Start").name = "a";
		}
		createdRooms.Clear();
	}

	public void FinishDeadEnd()
	{
		foreach (GameObject g in createdRooms)
		{
			g.transform.SetParent(GameObject.Find((_grid.currentFloor - 1).ToString() + ".").transform);
		}
		createdRooms.Clear();
		for (int i = 0; i < _grid.generators.Count; i++)
		{
			if (GameObject.Find(i.ToString()) != null)
			{
				return;
			}
		}
		_grid.Bake();
	}

	public void Spawnroom()
	{
		NextRoom();
		if (roomsToCheck.Count == 0 && !branch)
		{
			transform.position = _lastDoor.position;
			transform.eulerAngles = _lastDoor.eulerAngles;
			for (int i = 0; i < lastGridPositions.Count(); i++)
			{
				_grid.grid.Remove(lastGridPositions[i]);
			}
			Destroy(_lastRoom);
			spawnCount = 0;
			FinishFloor();
			GenerateRoom(endFloor);
			return;
		}
		if (roomsToCheck.Count == 0 && branch)
		{
			Destroy(gameObject);
			return;
		}
		if (deadEndsToCheck.Count == 0)
		{
			Destroy(gameObject);
			return;
		}
		if (spawnCount > 1)
		{
			_randomRoom = Random.Range(0, roomsToCheck.Count);
			_room = roomsToCheck[_randomRoom];
			GenerateRoom(_room, _randomRoom);
			return;
		}
		if (branch)
		{
			_randomRoom = Random.Range(0, deadEndsToCheck.Count);
			_room = deadEndsToCheck[_randomRoom];
			GenerateRoom(_room, _randomRoom, true);
			return;
		}
		if (spawnCount == 1 && !branch && roomsToCheck.Count == rooms.Length)
		{
			FinishFloor();
			GenerateRoom(endFloor);
		}
	}
	void GenerateRoom(GameObject newRoomPrefab, int roomindex = 0, bool deadEnd = false)
	{
		_wait = true;
		StartCoroutine(Wait());
		//Generate Room
		GameObject newRoom = Instantiate(newRoomPrefab, transform.position, transform.rotation);
		Transform[] gridPositions = newRoom.GetComponentsInChildren<Transform>().Where(t => t.CompareTag("GridPoint")).ToArray();
		doorways.Clear();
		doorways.AddRange(newRoom.GetComponentsInChildren<Transform>().Where(t => t.CompareTag("Doorway")));
		if (newRoomPrefab == endFloor)
		{
			_grid.currentFloor++;
			if (currentFloor != 1)
			{
				GameObject nextfloor = GameObject.Find("nextfloor" + (currentFloor - 1));
				Debug.Log(nextfloor);
				nextfloor.transform.SetParent(GameObject.Find(_grid.currentFloor - 1 + ".").transform);
			}
			GameObject.Find("NextFloorRoom(Clone)").name = "nextfloor" + currentFloor;
		}
		//Place Room at the next position
		newRoom.transform.position += transform.position - doorways[0].position;
		newRoom.transform.eulerAngles = transform.eulerAngles;
		//Check if any grid points intersect with an existing grid point
		foreach (Transform t in gridPositions)
		{
			Vector3 pos = new Vector3(Mathf.Round(t.position.x), Mathf.Round(t.position.y), Mathf.Round(t.position.z));
			if (_grid.grid.Contains(pos))
			{
				doorways.Clear();
				Destroy(newRoom);
				if (deadEnd)
				{
					deadEndsToCheck.Remove(_room);
					_wait = false;
					return;
				}
				roomsToCheck.Remove(_room);
				_wait = false;
				return;
			}
		}

		roomsToCheck.Clear();
		roomsToCheck.AddRange(rooms);
		deadEndsToCheck.Clear();
		deadEndsToCheck.AddRange(deadEnds);

		createdRooms.Add(newRoom);

		_lastRoom = newRoom;
		_lastDoor = doorways[0];
		foreach (Transform t in gridPositions)
		{
			Vector3 pos = new Vector3(Mathf.Round(t.position.x), Mathf.Round(t.position.y), Mathf.Round(t.position.z));
			_grid.grid.Add(pos);
			lastGridPositions.Add(pos);
		}
		spawnCount--;
		//Spawn Next Room
		if (spawnCount > 0)
		{
			Spawnroom();
		}

	}
	public void NextRoom()
	{
		int t = doorways.Count;
		int randomDoorwayIndex = Random.Range(1, doorways.Count);
		if (t > 0)
		{
			Transform door = doorways[randomDoorwayIndex];
			Vector3 FrontDoor = door.position + (door.rotation * door.forward * 2.5f);
			foreach (Vector3 xyz in _grid.grid)
			{
				if (Vector3.Distance(xyz, FrontDoor) < 1)
				{
					NextRoom();
					return;
				}
				else
				{
					transform.position = door.position;
					transform.eulerAngles = door.eulerAngles + new Vector3(0, 180, 0);
					_grid.usedDoors.Add(door.position);
				}
			}
		}
	}

	private void OnDestroy()
	{
		if (!CompareTag("Original"))
		{
			FinishDeadEnd();
		}
	}
}