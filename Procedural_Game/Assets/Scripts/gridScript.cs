using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;

public class gridScript : MonoBehaviour
{
	private GameObject _player;
	
    public List<Vector3> grid;
    public List<Vector3> elevatorGrid;
    public List<Vector3> usedDoors;
	public List<GameObject> generators;
	public GameObject lastElevator;
	public int numGenerators;

	public int currentFloor;
	public int complexity;

	public int seed;

	public List<GameObject> enemies;
	
	private void Awake()
	{
		grid.Add(GameObject.Find("GridPosition").transform.position);
		if (GameObject.Find("SeedManager") != null)
		{
			complexity = GameObject.Find("SeedManager").GetComponent<StartScript>().complexity;
		}
		_player = GameObject.Find("Player");
	}

	public void UpdateGrid()
	{
		for (int i = 0; i < grid.Count; i++)
		{
			grid[i] = new Vector3(Mathf.RoundToInt(grid[i].x), Mathf.RoundToInt(grid[i].y), Mathf.RoundToInt(grid[i].z));
		}
	}

	public void Bake()
	{
		generators.Clear();
		
		//Generate new navMesh
		GameObject navMeshGenerator = GameObject.Find("NavMesh Surface");
		navMeshGenerator.GetComponent<NavMeshSurface>().BuildNavMesh();
		
		//Destroy Enemies from previous floor
		if (GameObject.FindGameObjectsWithTag("Enemy").Any())
		{
			foreach (GameObject g in GameObject.FindGameObjectsWithTag("Enemy"))
			{
				Destroy(g);
			}
		}
		
		int randomNum = 1;
		for (int i = 0; i < randomNum; i++)
		{
			ChooseEnemy();
		}
	}

	public void ChooseEnemy()
	{
		//Spawn new random Enemy
		int randomNum = Random.Range(0, 15);
		
		if (randomNum < 15)
		{
			//Spawns Eyes
			StartCoroutine(SpawnEnemy(enemies[0]));
		}
		if (randomNum is >= 15 and < 30)
		{
			//Spawns Automaton
			StartCoroutine(SpawnEnemy(enemies[1]));
		}
		if (randomNum is >= 30 and < 45)
		{
			//Spawns 5 bugs
			StartCoroutine(SpawnEnemy(enemies[2]));
			StartCoroutine(SpawnEnemy(enemies[2]));
			StartCoroutine(SpawnEnemy(enemies[2]));
			StartCoroutine(SpawnEnemy(enemies[2]));
			StartCoroutine(SpawnEnemy(enemies[2]));
		}
		if (randomNum is >= 45 and < 60)
		{
			//spawns 7 bugs
			StartCoroutine(SpawnEnemy(enemies[2]));
			StartCoroutine(SpawnEnemy(enemies[2]));
			StartCoroutine(SpawnEnemy(enemies[2]));
			StartCoroutine(SpawnEnemy(enemies[2]));
			StartCoroutine(SpawnEnemy(enemies[2]));
			StartCoroutine(SpawnEnemy(enemies[2]));
			StartCoroutine(SpawnEnemy(enemies[2]));
		}
		if (randomNum is >= 60 and < 75)
		{
			//Spawns Ears
			StartCoroutine(SpawnEnemy(enemies[3]));
		}
		if (randomNum is >= 75 and < 90)
		{
			//Spawns 3 Zombies
			StartCoroutine(SpawnEnemy(enemies[4]));
			StartCoroutine(SpawnEnemy(enemies[4]));
			StartCoroutine(SpawnEnemy(enemies[4]));
		}
	}
	
	IEnumerator SpawnEnemy(GameObject enemy)
	{
		int attempts = 0;
		NavMeshHit hit;
		while (true)
		{
			if (attempts >= 10)
			{
				break;
			}
			Vector3 testPoint = new Vector3(Random.Range(-500, 500), transform.position.y, Random.Range(-500, 500));
			float dist = Vector3.Distance(testPoint, _player.transform.position);
			
			if (NavMesh.SamplePosition(testPoint, out hit, Mathf.Infinity, NavMesh.AllAreas) && dist > 20)
			{
				Instantiate(enemy, hit.position, Quaternion.identity);
				break;
			}
			attempts++;
			yield return null;
		}
	}
}
