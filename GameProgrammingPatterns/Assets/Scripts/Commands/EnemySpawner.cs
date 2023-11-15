using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;  // Reference to the enemy prefab.
    public int numberOfEnemiesToSpawn = 15;  // Number of enemies to spawn.
    public float spawnAreaSize = 50.0f;  // Size of the spawn area (50 by 50).
      


    private void Start()
    {
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < numberOfEnemiesToSpawn; i++)
        {
            // Generate random positions within the spawn area.
            float randomX = Random.Range(-spawnAreaSize / 2, spawnAreaSize / 2);
            float randomZ = Random.Range(-spawnAreaSize / 2, spawnAreaSize / 2);

            Vector3 spawnPosition = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

            // Instantiate and spawn the enemy prefab at the random position.
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.Euler(0f, 0f, 0f));

            
        }
    }
}
