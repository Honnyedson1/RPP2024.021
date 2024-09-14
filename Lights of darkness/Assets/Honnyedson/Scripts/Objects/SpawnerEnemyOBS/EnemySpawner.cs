using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; 
    public List<Transform> spawnLocations; 
    public int initialEnemyCount = 5; 
    private int currentEnemyCount; 
    private bool isSpawning = false; 

    private void OnEnable()
    {
        EnemySpawnObserver.OnEnemySpawn += HandleEnemyDeath;
        SpawnInitialEnemies(); 
    }

    private void OnDisable()
    {
        EnemySpawnObserver.OnEnemySpawn -= HandleEnemyDeath;
    }

    private void SpawnInitialEnemies()
    {
        currentEnemyCount = initialEnemyCount;
        SpawnEnemiesAtLocations();
    }

    private void SpawnEnemiesAtLocations()
    {
        foreach (Transform location in spawnLocations)
        {
            Instantiate(enemyPrefab, location.position, Quaternion.identity);
        }
    }

    private void HandleEnemyDeath(Transform spawnLocation, GameObject enemyPrefab)
    {
        currentEnemyCount--;
        if (currentEnemyCount <= 0 && !isSpawning)
        {
            StartCoroutine(SpawnEnemiesAfterDelay(6f));
        }
    }

    private IEnumerator SpawnEnemiesAfterDelay(float delay)
    {
        isSpawning = true; 
        yield return new WaitForSeconds(delay);
        SpawnEnemiesAtLocations();
        currentEnemyCount = initialEnemyCount; 
        isSpawning = false; 
    }
    public void SpawnExtraEnemies(int extraEnemyCount)
    {
        for (int i = 0; i < extraEnemyCount; i++)
        {
            Transform randomLocation = spawnLocations[Random.Range(0, spawnLocations.Count)];
            Instantiate(enemyPrefab, randomLocation.position, Quaternion.identity);
        }
        currentEnemyCount += extraEnemyCount;
    }
}
