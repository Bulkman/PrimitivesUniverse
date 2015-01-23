using UnityEngine;
using System.Collections;

public class AsteroidsSpawner : MonoBehaviour {
	public GameObject AsteroidPrefab;

	public int MaxAsteroidsNumber;

	public int AsteroidsCount;

	public float MinSpawnTime;

	public float MaxSpawnTime;

	private SpaceGridObject rootGrid;

	// Use this for initialization
	void Start () {
		AsteroidsCount = 0;

		rootGrid = GameObject.FindGameObjectWithTag("UniverseRootObject").GetComponent<SpaceGridObject>();

		StartCoroutine(SpawnAsteroid());
	}
	
	// Update is called once per frame
	IEnumerator SpawnAsteroid () {
		while(true){
			if(AsteroidsCount < MaxAsteroidsNumber){
				GameObject asteroid = Instantiate(AsteroidPrefab, rootGrid.GridBoundsCenter + Random.onUnitSphere * rootGrid.GridBoundsSize.x / 2f, Random.rotation) as GameObject;
				asteroid.transform.parent = transform;
				AsteroidsCount++;
			}

			yield return new WaitForSeconds(Random.Range(MinSpawnTime, MaxSpawnTime));
		}
	}
}
