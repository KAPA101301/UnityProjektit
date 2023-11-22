using UnityEngine;

public class GoldCoinSpawner : MonoBehaviour
{
    public GameObject goldCoinPrefab;  // Reference to the Gold Coin prefab.
    public int numberOfCoinsToSpawn = 10;  // Number of coins to spawn.
    public float spawnAreaSize = 50.0f;  // Size of the spawn area (50 by 50).
    public float coinLifetime = 60.0f;  // Time in seconds before coins are destroyed.


    private void Start()
    {
        SpawnCoins();
    }

    private void SpawnCoins()
    {
        for (int i = 0; i < numberOfCoinsToSpawn; i++)
        {
            // Generate random positions within the spawn area.
            float randomX = Random.Range(-spawnAreaSize / 2, spawnAreaSize / 2);
            float randomZ = Random.Range(-spawnAreaSize / 2, spawnAreaSize / 2);

            Vector3 spawnPosition = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

            // Instantiate and spawn the gold coin prefab at the random position.
            GameObject newCoin = Instantiate(goldCoinPrefab, spawnPosition, Quaternion.Euler(90f,0f,0f));

            // Destroy the coin after a specified time (coinLifetime).
            // Destroy(newCoin, coinLifetime);
        }
    }
}
