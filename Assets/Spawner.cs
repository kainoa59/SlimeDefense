using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int baseEnemyCount = 10; // Base number of enemies per wave
    [SerializeField] private GameObject testGO; // Reference to the enemy prefab
    [SerializeField] private float delayBtwSpawns = 1.0f; // Delay between enemy spawns

    [Header("Wave Settings")]
    [SerializeField] private int totalWaves = 5; // Total number of waves
    [SerializeField] private float firstWaveInterval = 5.0f; // Time before the first wave
    [SerializeField] private float subsequentWaveInterval = 10.0f; // Time between subsequent waves

    private float _spawnTimer;
    private int _enemiesSpawned;
    private ObjectPooler _pooler;
    private int _currentWave = 0;
    private float _currentHealthMultiplier = 1.00f;
    private float _currentMoneyMultiplier = 1.00f;

    [SerializeField] private Waypoints[] waveWaypoints; // Array to hold waypoints for each wave

    // Start is called before the first frame update
    void Start()
    {
        _pooler = GetComponent<ObjectPooler>();
        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        // First wave wait time
        yield return new WaitForSeconds(firstWaveInterval);

        while (_currentWave < totalWaves)
        {
            int enemyCount = baseEnemyCount + _currentWave * 2; // Increase enemy count with each wave
            _enemiesSpawned = 0; // Reset the enemies spawned for this wave

            if (_currentWave > 0)
            {
                _currentHealthMultiplier = 1.1f; // Increase the health multiplier after each wave
                _currentMoneyMultiplier = 1.2f; // Update the money multiplier
            }

            while (_enemiesSpawned < enemyCount)
            {
                _spawnTimer = delayBtwSpawns; // Reset the spawn timer
                while (_spawnTimer > 0)
                {
                    _spawnTimer -= Time.deltaTime; // Countdown the timer
                    yield return null; // Wait for the next frame
                }

                SpawnEnemy(); // Spawn the enemy
                _enemiesSpawned++; // Increment the count of spawned enemies
            }

            _currentWave++; // Move to the next wave
            delayBtwSpawns *= 0.8f;

            // Wait for the subsequent waves
            if (_currentWave < totalWaves)
            {
                yield return new WaitForSeconds(subsequentWaveInterval);
            }
        }
        SceneManager.LoadScene("StartScene");
        Debug.Log("All waves completed!");
    }

    private void SpawnEnemy()
    {
        GameObject newInstance = _pooler.GetInstanceFromPool();
        newInstance.transform.position = new Vector3(-12.3f, 1.67f, 0); // Adjust the spawn position as needed
        newInstance.SetActive(true);

        EnemyHealth enemyHealth = newInstance.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.IncreaseMaxHealth(_currentHealthMultiplier); // Apply the health multiplier for the current wave
            enemyHealth.IncreaseMoneyValue(_currentMoneyMultiplier); // Apply the money multiplier for the current wave
        }

        // Set the waypoints for the new enemy
        MoveToPoints enemyMovement = newInstance.GetComponent<MoveToPoints>();
        if (enemyMovement != null && _currentWave < waveWaypoints.Length)
        {
            enemyMovement.waypoints = waveWaypoints[_currentWave]; // Assign the correct waypoints based on the current wave
        }

        Debug.Log("Enemy spawned: " + newInstance.name + " with health multiplier: " + _currentHealthMultiplier);
    }
}
