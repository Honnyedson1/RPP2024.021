using System;
using UnityEngine;

public static class EnemySpawnObserver
{
    public static event Action<Transform, GameObject> OnEnemySpawn;

    public static void TriggerEnemySpawn(Transform spawnLocation, GameObject enemyPrefab)
    {
        OnEnemySpawn?.Invoke(spawnLocation, enemyPrefab);
    }
}