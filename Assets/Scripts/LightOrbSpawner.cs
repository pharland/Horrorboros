using UnityEngine;

public class LightOrbSpawner : MonoBehaviour
{
    public GameObject lightOrbPrefab;
    public GameObject spawnArea;
    public LightManager tutorialOrbManager;
    public float spawnInterval = 5f;
    public int maxAttempts = 20;
    public float minDistance = 0.1f;

    private float timer;


    private void Update()
    {
        // Prevent spawning until the tutorial orb has been collected/destroyed
        if (tutorialOrbManager != null && tutorialOrbManager.isTutorialOrb && tutorialOrbManager.gameObject != null)
        {
            // Tutorial orb still exists, do not spawn
            return;
        }

        // Update timer and check if it's time to spawn a new orb
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnLightOrb();
            timer = 0f;
        }
    }

    private void SpawnLightOrb()
    {
        // Cache spawn area transform and scale for efficiency
        Transform areaTransform = spawnArea.transform;
        Vector3 areaScale = areaTransform.localScale;
        Vector3 areaPosition = areaTransform.position;

        // Initialize variables for spawn attempt
        int attempts = 0;
        bool validPosition = false;
        Vector3 spawnPosition = Vector3.zero;

        // Try to find a valid spawn position within the area
        while (!validPosition && attempts < maxAttempts)
        {
            // Use cached values for random position
            Vector3 randomOffset = new(
                Random.Range(-areaScale.x * 0.5f, areaScale.x * 0.5f),
                Random.Range(-areaScale.y * 0.5f, areaScale.y * 0.5f),
                Random.Range(-areaScale.z * 0.5f, areaScale.z * 0.5f)
            );
            spawnPosition = areaPosition + randomOffset;

            // Use a non-alloc version to reduce GC pressure
            Collider[] colliders = Physics.OverlapSphere(spawnPosition, minDistance, ~0, QueryTriggerInteraction.Ignore);
            validPosition = true;
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].CompareTag("Obstacle") || colliders[i].CompareTag("LightOrb"))
                {
                    validPosition = false;
                    break;
                }
            }
            attempts++;
        }

        // Spawn the light orb if a valid position was found
        if (validPosition)
        {
            Instantiate(lightOrbPrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Failed to find a valid spawn position for Light Orb after maximum attempts.");
        }
    }
}
