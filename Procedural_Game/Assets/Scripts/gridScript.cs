using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;

public class gridScript : MonoBehaviour
{
    public List<Vector3> grid;
    public List<Vector3> elevatorGrid;
    public List<Vector3> usedDoors;
	public List<GameObject> generators;
	public GameObject lastElevator;
	public int numGenerators;

	public int currentFloor;
	public int complexity;

	public int seed;

	public GameObject eyes;
	public GameObject automaton;
	public GameObject bug;
	
	private void Awake()
	{
		grid.Add(GameObject.Find("GridPosition").transform.position);
		if (GameObject.Find("SeedManager") != null)
		{
			complexity = GameObject.Find("SeedManager").GetComponent<StartScript>().complexity;
		}
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
		GameObject navMeshGenerator = GameObject.Find("NavMesh Surface");
		navMeshGenerator.GetComponent<NavMeshSurface>().BuildNavMesh();
		if (GameObject.FindGameObjectsWithTag("Enemy").Count() > 0)
		{
			foreach (GameObject g in GameObject.FindGameObjectsWithTag("Enemy"))
			{
				Destroy(g);
			}
		}
		int randomnum = Random.Range(0, 100);
		if (randomnum < 30)
		{
			StartCoroutine(SpawnEnemy(eyes));
		}
		if (randomnum >= 30 && randomnum < 50)
		{
			StartCoroutine(SpawnEnemy(automaton));
		}
		if (randomnum >= 50 && randomnum < 60)
		{
			StartCoroutine(SpawnEnemy(bug));
			StartCoroutine(SpawnEnemy(bug));
			StartCoroutine(SpawnEnemy(bug));
		}
		if (randomnum >= 60 && randomnum < 95)
		{
			StartCoroutine(SpawnEnemy(bug));
			StartCoroutine(SpawnEnemy(bug));
		}
		if (randomnum >= 95 && randomnum < 100)
		{
			StartCoroutine(SpawnEnemy(bug));
			StartCoroutine(SpawnEnemy(bug));
			StartCoroutine(SpawnEnemy(bug));
			StartCoroutine(SpawnEnemy(bug));
			StartCoroutine(SpawnEnemy(bug));
		}
	}
	IEnumerator SpawnEnemy(GameObject enemy)
	{
		NavMeshHit hit;
		while (true)
		{
			if (NavMesh.SamplePosition(new Vector3(Random.Range(-500, 500), transform.position.y, Random.Range(-500, 500)), out hit, Mathf.Infinity, NavMesh.AllAreas))
			{
				Instantiate(enemy, hit.position, Quaternion.identity);
				break;
			}
			yield return null;
		}
	}
}
