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
	public bool testBool = true;

	public List<GameObject> roomsToCheck;
	public List<GameObject> deadEndsToCheck;

	private int _randomRoom;
	private GameObject _room;

	private GameObject _lastRoom;
	private Transform _lastDoor;
	public List<Vector3> lastGridPositions;

	private bool _wait;
	private int _falseAttempts;

	private void Start()
	{
        //Finds Grid Manager
		_grid = GameObject.Find("Grid").GetComponent<gridScript>();
		_grid.generators.Add(gameObject);
		_grid.numGenerators++;
		order = _grid.numGenerators;
		name = order.ToString();
        //Set initial Values of the first Room Spawner
		if (GameObject.Find("SeedManager") != null && CompareTag("Original") && name == "1")
		{
			StartScript info = GameObject.Find("SeedManager").GetComponent<StartScript>();
			_grid.seed = info.seed;
			currentFloor = 1;
			_grid.currentFloor = 1;
			complexity = _grid.complexity;
		}
        //Handles when launched from game scene and not menu scene
		if (GameObject.Find("SeedManager") == null && CompareTag("Original"))
		{
			currentFloor = _grid.currentFloor;
			complexity = _grid.complexity;
		}
        //Handles Spawners from beyond the first floor
		if (CompareTag("Original") && name != "1")
		{
			currentFloor = _grid.currentFloor;
			complexity = _grid.complexity;
		}
        //Adds List of Rooms that spawner can spawn and how many rooms must be spawned
		roomsToCheck.AddRange(rooms);
		deadEndsToCheck.AddRange(deadEnds);
		if (CompareTag("Original"))
		{
			spawnCount = Mathf.RoundToInt((complexity + 1) * currentFloor / 6f) + 2;
		}
		else
		{
			float odds = Random.Range(0, 100);
			spawnCount = odds <= 80 ? 1 : 2;
		}
        
		foreach (Vector3 v in _grid.usedDoors)
		{
			if (branch && Vector3.Distance(transform.position,v) < 1)
			{
				Destroy(gameObject);
			}
		}
        //Destroys previous floor
		if (GameObject.Find((currentFloor - 1) + ".") != null && CompareTag("Original"))
		{
			Destroy(GameObject.Find((currentFloor - 1) + "."));
		}
        //Change seed as floors are generated
		if (CompareTag("Original"))
		{
			Random.InitState(_grid.seed);
		}
        //Starts room generation
        StartCoroutine(Suspend());
	}
	void Update()
    {
		if (spawnCount <= 0)
		{
			Destroy(gameObject);
		}
        //Snap Spawner to integer grid
		for(int i = 0; i < _grid.grid.Count(); i++)
		{
			_grid.grid[i] = new Vector3(Mathf.Round(_grid.grid[i].x), Mathf.Round(_grid.grid[i].y), Mathf.Round(_grid.grid[i].z));
		}
	}

	IEnumerator Suspend()
	{
        //Wait for main branch to finish
		while (GameObject.FindGameObjectWithTag("Original") != null && !CompareTag("Original"))
		{
			if (GameObject.FindGameObjectWithTag("Original").GetComponent<SpawningRooms>().order > order)
			{
				break;
			}
			yield return null;
		}
		while (true)
		{
			float spawners = 0;
			//Check if there are any spawners that must go first
			for (int i = 1; i < order - 1; i++)
			{
				if (GameObject.Find((order - i).ToString()) != null)
				{
					spawners++;
				}
			}
			
			if (spawners == 0)
			{
				break;
			}
			yield return new WaitForSeconds(Time.deltaTime);
		}
		//Placeholder to test Room Generation
		/*
		if (CompareTag("Original"))
		{
			while (testBool)
			{
				yield return null;
			}
		}
		*/
		yield return new WaitForSeconds(Time.deltaTime);
		NextRoom();
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
        //Puts all rooms in a floor in one parent
		GameObject endFloor = Instantiate(empty);
		endFloor.name = currentFloor + ".";
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
        //Moves Dead Ends into parent floor
		foreach (GameObject g in createdRooms)
		{
			g.transform.SetParent(GameObject.Find((_grid.currentFloor - 1) + ".").transform);
		}
		createdRooms.Clear();
		for (int i = 0; i < _grid.generators.Count; i++)
		{
			if (GameObject.Find(_grid.generators[i].name) != null)
			{
				return;
			}
		}
		_grid.Bake();
		if (GameObject.FindGameObjectWithTag("Maintenance") != null)
		{
			List<GameObject> lights = GameObject.FindGameObjectsWithTag("RoomLight").ToList();
			foreach (GameObject g in lights)
			{
				g.GetComponent<RoomLightScript>().forceOn = false;
				g.GetComponent<Light>().enabled = false;
			}
		}
	}

	public void Spawnroom()
	{
        //Handles if no room can be generated (forces end room to spawn)
		if (roomsToCheck.Count == 0 && !branch)
		{
			transform.position = _lastDoor.position;
			transform.eulerAngles = _lastDoor.eulerAngles;
			for (int i = 0; i < lastGridPositions.Count(); i++)
			{
				_grid.grid.Remove(lastGridPositions[i]);
			}
			createdRooms.Remove(_lastRoom);
			Destroy(_lastRoom);
			roomsToCheck.AddRange(rooms);
			roomsToCheck.Remove(_lastRoom);
			_falseAttempts++;
			if (_falseAttempts == rooms.Length)
			{
				spawnCount = 1;
				roomsToCheck.Clear();
				roomsToCheck.AddRange(rooms);
			}
		}
        //Handles if a Branch can't create a room
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
        //Starts the GenerateRoom function
		if (spawnCount > 1)
		{
			_randomRoom = Random.Range(0, roomsToCheck.Count);
			_room = roomsToCheck[_randomRoom];
			GenerateRoom(_room);
			return;
		}
		if (branch)
		{
			_randomRoom = Random.Range(0, deadEndsToCheck.Count);
			_room = deadEndsToCheck[_randomRoom];
			GenerateRoom(_room, true);
			return;
		}
        //Spawn the elevator if spawn count is 1
		if (spawnCount == 1 && !branch && roomsToCheck.Count == rooms.Length)
		{
			FinishFloor();
			GenerateRoom(endFloor);
		}
	}
    
	void GenerateRoom(GameObject newRoomPrefab, bool deadEnd = false)
	{
		_wait = true;
		StartCoroutine(Wait());
		
		//Generate Room
		GameObject newRoom = Instantiate(newRoomPrefab, transform.position, transform.rotation);
		doorways.Clear();
		doorways.AddRange(newRoom.GetComponentsInChildren<Transform>().Where(t => t.CompareTag("Doorway")));
		if (newRoomPrefab == endFloor)
		{
			_grid.currentFloor++;
			if (currentFloor != 1)
			{
				GameObject nextfloor = GameObject.Find("nextfloor" + (currentFloor - 1));
				nextfloor.transform.SetParent(GameObject.Find(_grid.currentFloor - 1 + ".").transform);
			}
			GameObject.Find("NextFloorRoom(Clone)").name = "nextfloor" + currentFloor;
		}
		
		//Place Room at the next position
		newRoom.transform.position += transform.position - doorways[0].position;
		newRoom.transform.eulerAngles = transform.eulerAngles;
		Transform[] gridPositions = newRoom.GetComponentsInChildren<Transform>().Where(t => t.CompareTag("GridPoint")).ToArray();
		
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
		
        //Reset Rooms to check lists
		roomsToCheck.Clear();
		roomsToCheck.AddRange(rooms);
		deadEndsToCheck.Clear();
		deadEndsToCheck.AddRange(deadEnds);
		lastGridPositions.Clear();
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
			NextRoom();
			Spawnroom();
		}

	}
	private void NextRoom()
	{
        //Sets Spawners position to next location
		int t = doorways.Count;
		int randomDoorwayIndex = Random.Range(1, doorways.Count);
		if (t > 0)
		{
			Transform door = doorways[randomDoorwayIndex];
			Vector3 frontDoor = door.position + door.right * 2.5f + new Vector3(0,3,0);
			frontDoor = new Vector3(Mathf.RoundToInt(frontDoor.x), Mathf.RoundToInt(frontDoor.y), Mathf.RoundToInt(frontDoor.z));
			if (_grid.grid.Contains(frontDoor))
			{
				roomsToCheck.Clear();
				return;
			}
			transform.position = door.position;
			transform.eulerAngles = door.eulerAngles + new Vector3(0, 180, 0);
			_grid.usedDoors.Add(door.position);
		}
	}

	private void OnDestroy()
	{
		_grid.generators.Remove(gameObject);
		if (!CompareTag("Original"))
		{
			FinishDeadEnd();
		}
	}
}
