using UnityEngine;

public class BloodTrailManager : MonoBehaviour
{
    public GameObject bloodTrailPrefab;
    public float spawnInterval = 0.5f;
    public float spawnDistanceBehindPlayer = 0.5f;

    private Transform playerTransform;
    private float timer = 0f;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Head").transform;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnBloodTrail();
            timer = 0f;
        }
    }

    /// <summary>
    /// Spawns a blood decal at regular intervals behind the player
    /// </summary>
    void SpawnBloodTrail()
    {
        if (bloodTrailPrefab == null || playerTransform == null)
        {
            Debug.LogWarning("BloodTrailManager: Missing bloodTrailPrefab or playerTransform.");
            return;
        }

        Vector3 spawnPosition = playerTransform.position - (playerTransform.forward * spawnDistanceBehindPlayer);
        spawnPosition.y = 0.004f; // Slightly above ground to avoid z-fighting
        Instantiate(bloodTrailPrefab, spawnPosition, Quaternion.Euler(90f, 0f, 0f));
    }
}
