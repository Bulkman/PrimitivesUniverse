using UnityEngine;
using System.Collections;

public class EnemyShipsSpawner : MonoBehaviour {

	public GameObject EnemyShipPrefab;
	
	public int MaxEnemyShipsNumber;
	
	public int EnemyShipsCount;
	
	public float MinSpawnTime;
	
	public float MaxSpawnTime;
	
	private SpaceGridObject rootGrid;
	
	// Use this for initialization
	void Start () {
		EnemyShipsCount = 0;
		
		rootGrid = GameObject.FindGameObjectWithTag("UniverseRootObject").GetComponent<SpaceGridObject>();
		
		StartCoroutine(SpawnAsteroid());
	}
	
	// Update is called once per frame
	IEnumerator SpawnAsteroid () {
		while(true){
			if(EnemyShipsCount < MaxEnemyShipsNumber){
				GameObject asteroid = Instantiate(EnemyShipPrefab, rootGrid.GridBoundsCenter + Random.onUnitSphere * rootGrid.GridBoundsSize.x / 2f, Random.rotation) as GameObject;
				asteroid.transform.parent = transform;
				EnemyShipsCount++;
			}
			
			yield return new WaitForSeconds(Random.Range(MinSpawnTime, MaxSpawnTime));
		}
	}
}
