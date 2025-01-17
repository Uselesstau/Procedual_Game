using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using Unity.VisualScripting;
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

	private GameObject _lastRoom;
	private Transform _lastDoor;
	public int numLastGridPoints;

	private int _retries = 2;
	private bool _spawnerMoved = true;

	private void Start()
	{
		//Finds Grid Manager
		_grid = GameObject.Find("Grid").GetComponent<gridScript>();
		//Destroy this if next to a door
		foreach (Vector3 v in _grid.usedDoors)
		{
			if (branch && Vector3.Distance(transform.position,v) < 1)
			{
				Destroy(gameObject);
			}
		}
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
        //Handles when launched from game scene and not menu scene or from the first floor
		if ((GameObject.Find("SeedManager") == null || name != "1") && CompareTag("Original"))
		{
			currentFloor = _grid.currentFloor;
			complexity = _grid.complexity;
		}
        //Adds List of Rooms that spawner can spawn and how many rooms must be spawned
		roomsToCheck.AddRange(rooms);
		deadEndsToCheck.AddRange(deadEnds);
		if (CompareTag("Original"))
		{
			spawnCount = Mathf.RoundToInt(complexity * currentFloor / 5f) + 2;
		}
		else
		{
			float odds = Random.Range(0, 100);
			spawnCount = odds <= 80 ? 1 : 2;
		}
        //Destroys previous floor
		if (GameObject.Find((currentFloor - 1) + ".") != null && CompareTag("Original"))
		{
			Destroy(GameObject.Find((currentFloor - 1) + "."));
		}
	}
	void Update()
	{
		bool startGeneration = _grid.generators[0].GetComponent<SpawningRooms>().order == order;
		
		if (startGeneration)
		{
			SpawnRoom();
		}
	}

	public void FinishFloor()
	{
        //Puts all rooms in a floor in one parent
		GameObject endCurrentFloor = Instantiate(empty);
		endCurrentFloor.name = currentFloor + ".";
		for (int i = 0; i < createdRooms.Count - 1; i++)
		{
			createdRooms[i].transform.SetParent(endCurrentFloor.transform);
		}
		if (GameObject.Find("Start") != null)
		{
			GameObject.Find("Start").transform.SetParent(endCurrentFloor.transform);
			GameObject.Find("Start").name = "Starting Room";
		}
		if (_grid.generators.Count > 0)
		{
			return;
		}
		_grid.Bake();
		_grid.currentFloor++;
	}

	public void FinishDeadEnd()
	{
        //Moves Dead Ends into parent floor
        if (GameObject.Find(_grid.currentFloor + ".") == null)
        {
	        return;
        }
		foreach (GameObject g in createdRooms)
		{
			g.transform.SetParent(GameObject.Find(_grid.currentFloor + ".").transform);
		}
		if (_grid.generators.Count > 0)
		{
			return;
		}
		_grid.Bake();
		_grid.currentFloor++;
		if (GameObject.FindGameObjectWithTag("Maintenance") == null)
		{
			return;
		}
		List<GameObject> lights = GameObject.FindGameObjectsWithTag("RoomLight").ToList();
		foreach (GameObject g in lights)
		{
			g.GetComponent<RoomLightScript>().forceOn = false;
			g.GetComponent<Light>().enabled = false;
		}
	}

	private void SpawnRoom()
	{
		//Check if spawner has moved
		if (!_spawnerMoved)
		{
			return;
		}
        //Starts the GenerateRoom function
		if (spawnCount > 1)
		{
			GenerateRoom(false);
			return;
		}
		//Spawn a dead end if it's a branch
		if (branch)
		{
			GenerateRoom(false, true);
			return;
		}
        //Spawn the elevator if spawn count is 1
		if (spawnCount == 1 && !branch)
		{
			GenerateRoom(true);
		}
	}
    
	private void GenerateRoom(bool end, bool deadEnd = false)
	{
		//determine the seed
		Random.InitState(_grid.seed + createdRooms.Count() + _grid.currentFloor + order);
		
		//Generate Room
		Vector3 initialSpawnPos = transform.position + new Vector3(0, 100f, 0);
		GameObject room = null;
		int doorIndex = 0;
		if (end)
		{
			GameObject newRoom = Instantiate(endFloor, initialSpawnPos, Quaternion.identity);
			doorways.Clear();
			doorways.AddRange(newRoom.GetComponentsInChildren<Transform>().Where(t => t.CompareTag("Doorway")));
			int frontDoorway = 0;
		
			//Place Room at the next position
			Vector3 rotation = doorways[frontDoorway].localEulerAngles;
			if (Mathf.Approximately(rotation.y, 180))
			{
				rotation.y = 0;
			}
			newRoom.transform.eulerAngles = transform.eulerAngles - doorways[frontDoorway].localEulerAngles;
			newRoom.transform.position += transform.position - doorways[frontDoorway].position;
			Transform[] gridPositions = newRoom.GetComponentsInChildren<Transform>().Where(t => t.CompareTag("GridPoint")).ToArray();
			if (_grid.lastElevator != null)
			{
				createdRooms.Add(_grid.lastElevator);
			}
			_grid.lastElevator = newRoom;

			room = newRoom;
			doorIndex = frontDoorway;
			foreach (Transform t in gridPositions)
			{
				Vector3 pos = t.position;
				_grid.grid.Add(pos);
				_grid.elevatorGrid.Add(pos);
				numLastGridPoints++;
				_grid.UpdateGrid();
			}
		}
		if (!end && !deadEnd)
		{
			for (int i = 0; i < rooms.Count(); i++)
			{
				if (!roomsToCheck.Any())
				{
					break;
				}
				int randIndex = Random.Range(0, roomsToCheck.Count);
				GameObject newRoom = Instantiate(roomsToCheck[randIndex], initialSpawnPos, Quaternion.identity);
				doorways.Clear();
				doorways.AddRange(newRoom.GetComponentsInChildren<Transform>().Where(t => t.CompareTag("Doorway")));
				int frontDoorway = Random.Range(0, doorways.Count);
		
				//Place Room at the next position
				Vector3 rotation = doorways[frontDoorway].localEulerAngles;
				if (Mathf.Approximately(rotation.y, 180))
				{
					rotation.y = 0;
				}
				newRoom.transform.eulerAngles = transform.eulerAngles - doorways[frontDoorway].localEulerAngles;
				newRoom.transform.position += transform.position - doorways[frontDoorway].position;
				Transform[] gridPositions = newRoom.GetComponentsInChildren<Transform>().Where(t => t.CompareTag("GridPoint")).ToArray();
		
				//Check if any grid points intersect with an existing grid point
				bool flag = false;
				foreach (Transform t in gridPositions)
				{
					Vector3 pos = new Vector3(Mathf.RoundToInt(t.position.x), Mathf.RoundToInt(t.position.y), Mathf.RoundToInt(t.position.z));
					if (_grid.grid.Contains(pos))
					{
						Destroy(newRoom);
						roomsToCheck.Remove(roomsToCheck[randIndex]);
						flag = true;
						break;
					}
				}
				if (flag)
				{
					continue;
				}
				room = newRoom;
				doorIndex = frontDoorway;
				foreach (Transform t in gridPositions)
				{
					Vector3 pos = t.position;
					_grid.grid.Add(pos);
					numLastGridPoints++;
					_grid.UpdateGrid();
				}
				break;
			}
		}

		if (deadEnd && !end)
		{
			for (int i = 0; i < rooms.Count(); i++)
			{
				int randIndex = Random.Range(0, deadEndsToCheck.Count);
				GameObject newRoom = Instantiate(deadEndsToCheck[randIndex], initialSpawnPos, Quaternion.identity);
				doorways.Clear();
				doorways.AddRange(newRoom.GetComponentsInChildren<Transform>().Where(t => t.CompareTag("Doorway")));
				int frontDoorway = 0;
		
				//Place Room at the next position
				newRoom.transform.eulerAngles = transform.eulerAngles - doorways[frontDoorway].localEulerAngles;
				newRoom.transform.position += transform.position - doorways[frontDoorway].position;
				Transform[] gridPositions = newRoom.GetComponentsInChildren<Transform>().Where(t => t.CompareTag("GridPoint")).ToArray();
		
				//Check if any grid points intersect with an existing grid point
				bool flag = false;
				foreach (Transform t in gridPositions)
				{
					Vector3 pos = new Vector3(Mathf.RoundToInt(t.position.x), Mathf.RoundToInt(t.position.y), Mathf.RoundToInt(t.position.z));
					if (_grid.grid.Contains(pos))
					{
						Destroy(newRoom);
						deadEndsToCheck.Remove(deadEndsToCheck[randIndex]);
						flag = true;
						break;
					}
				}
				if (flag)
				{
					continue;
				}
				room = newRoom;
				doorIndex = frontDoorway;
				foreach (Transform t in gridPositions)
				{
					Vector3 pos = t.position;
					_grid.grid.Add(pos);
					numLastGridPoints++;
					_grid.UpdateGrid();
				}
				break;
			}
		}

		if (room == null && !branch)
		{
			transform.position = _lastDoor.position;
			transform.eulerAngles = _lastDoor.eulerAngles;
			for (int i = 0; i < numLastGridPoints; i++)
			{
				_grid.grid.RemoveAt(_grid.grid.Count-i-1);
			}
			createdRooms.Remove(_lastRoom);
			Destroy(_lastRoom);
			roomsToCheck.AddRange(rooms);
			roomsToCheck.Remove(_lastRoom);
			_retries--;
			if (_retries == 0)
			{
				spawnCount = 1;
			}
			_spawnerMoved = true;
			SpawnRoom();
			return;
		}
		if (room == null && branch)
		{
			Destroy(gameObject);
		}

		if (doorways.Count == 0)
		{
			Destroy(gameObject);
			return;
		}
        //Reset Rooms to check lists
		roomsToCheck.Clear();
		roomsToCheck.AddRange(rooms);
		deadEndsToCheck.Clear();
		deadEndsToCheck.AddRange(deadEnds);
		numLastGridPoints = 0;
		createdRooms.Add(room);

		_lastRoom = room;
		_lastDoor = doorways[doorIndex];
		_grid.usedDoors.Add(doorways[doorIndex].position);
		doorways.Remove(_lastDoor);
		spawnCount--;
		
		//Destroy if no more rooms can spawn
		if (spawnCount == 0)
		{
			Destroy(gameObject);
			return;
		}
		
		//Spawn Next Room
		NextRoom();
		SpawnRoom();
	}
	private void NextRoom()
	{
        //Sets Spawners position to next location
        _spawnerMoved = false;
		int t = doorways.Count;
		int randomDoorwayIndex = Random.Range(0, doorways.Count);
		if (t > 0)
		{
			Transform door = doorways[randomDoorwayIndex];
			Vector3 frontDoor = door.position + door.right * 2.5f + new Vector3(0,3,0);
			frontDoor = new Vector3(Mathf.RoundToInt(frontDoor.x), Mathf.RoundToInt(frontDoor.y), Mathf.RoundToInt(frontDoor.z));
			if (_grid.grid.Contains(frontDoor))
			{
				doorways.RemoveAt(randomDoorwayIndex);
				NextRoom();
				return;
			}
			transform.position = door.position;
			transform.eulerAngles = door.eulerAngles + new Vector3(0, 180, 0);
			_spawnerMoved = true;
		}
	}

	private void OnDestroy()
	{
		_grid.generators.Remove(gameObject);
		if (branch)
		{
			FinishDeadEnd();
		}
		else
		{
			FinishFloor();
		}
	}
}
