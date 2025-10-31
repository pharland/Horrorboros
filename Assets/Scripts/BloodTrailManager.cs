using UnityEngine;
using System.Collections.Generic;

public class BloodTrailManager : MonoBehaviour
{
    public GameObject bloodTrailPrefab;
    public int poolSize = 50; // Adjust based on expected max decals
    public float spawnInterval = 0.5f;
    public float spawnDistanceBehindPlayer = 0.5f;
    public float decalLifetime = 10f;

    private Transform playerTransform;
    private float timer = 0f;
    private SoundManager _soundManager;

    private Queue<GameObject> bloodTrailPool;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Head").transform;
        PlayTailSFX();

        // Initialize pool
        bloodTrailPool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(bloodTrailPrefab);
            obj.SetActive(false);
            bloodTrailPool.Enqueue(obj);
        }
    }

    void Update()
    {
        // Spawn blood trail decals at intervals
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnBloodTrail();
            timer = 0f;
        }
    }

    // Spawn a blood trail decal behind the player
    void SpawnBloodTrail()
    {
        if (bloodTrailPool == null || bloodTrailPool.Count == 0 || playerTransform == null)
            return;

        // Dequeue a decal from the pool
        GameObject decal = bloodTrailPool.Dequeue();
        Vector3 spawnPosition = playerTransform.position - (playerTransform.forward * spawnDistanceBehindPlayer);
        spawnPosition.y = 0.004f; // Slightly above ground to avoid z-fighting
        decal.transform.SetPositionAndRotation(spawnPosition, Quaternion.Euler(90f, 0f, 0f));
        decal.SetActive(true);

        // Disable after some time
        StartCoroutine(DisableAfterSeconds(decal, decalLifetime));

        bloodTrailPool.Enqueue(decal); // Re-enqueue for reuse
    }

    // Coroutine to disable decal after lifetime
    private System.Collections.IEnumerator DisableAfterSeconds(GameObject obj, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        obj.SetActive(false);
    }

    private void PlayTailSFX()
    {
        if (_soundManager == null)
        {
            _soundManager = FindFirstObjectByType<SoundManager>();
        }
        if (_soundManager != null)
        {
            _soundManager.TailSFX();
        }
    }
}
