using UnityEngine;

public class LightOrbSpawner : MonoBehaviour
{
    public GameObject lightOrbPrefab;
    public float spawnInterval = 5f; // Time in seconds between spawns
    public GameObject spawnArea;

    private float timer;
    private Transform spawnLocation;

    private void Update()
    {
        // Increment the timer for the next spawn
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnLightOrb();
            timer = 0f;
        }
    }

    private void SpawnLightOrb()
    {
        // Pick a random position within the spawn area (relative to its center)
        Vector3 randomOffset = new(
            Random.Range(-spawnArea.transform.localScale.x / 2, spawnArea.transform.localScale.x / 2),
            Random.Range(-spawnArea.transform.localScale.y / 2, spawnArea.transform.localScale.y / 2),
            Random.Range(-spawnArea.transform.localScale.z / 2, spawnArea.transform.localScale.z / 2)
        );
        Vector3 spawnPosition = spawnArea.transform.position + randomOffset;

        // Instantiate the light orb at the random position
        Instantiate(lightOrbPrefab, spawnPosition, Quaternion.identity);
    }
}
