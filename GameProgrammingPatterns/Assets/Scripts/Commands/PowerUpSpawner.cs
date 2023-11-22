using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    public GameObject powerUpPrefab;  // Reference to the Power Up prefab.
    public int numberOfPowerUpsToSpawn = 1;  // Number of power ups to spawn.
    public float spawnAreaSize = 50.0f;  // Size of the spawn area (50 by 50).
    public float pwerUpLifetime = 60.0f;  // Time in seconds before power up is destroyed.


    private void Start()
    {
        SpawnPowerUp();
    }

    private void SpawnPowerUp()
    {
        for (int i = 0; i < numberOfPowerUpsToSpawn; i++)
        {
            // Generate random positions within the spawn area.
            float randomX = Random.Range(-spawnAreaSize / 2, spawnAreaSize / 2);
            float randomZ = Random.Range(-spawnAreaSize / 2, spawnAreaSize / 2);

            Vector3 spawnPosition = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

            // Instantiate and spawn the Power Up prefab at the random position.
            GameObject newPowerUp = Instantiate(powerUpPrefab, spawnPosition, Quaternion.Euler(90f, 0f, 0f));

            // Destroy the power up after a specified time (powerUpLifetime).
            // Destroy(newPowerUp, powerUpLifetime);
        }
    }
}
