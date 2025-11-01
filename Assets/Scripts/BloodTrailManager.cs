using UnityEngine;
using System.Collections.Generic;

public class BloodTrailManager : MonoBehaviour
{
    public GameObject bloodTrailPrefab;
    public int poolSize = 50;
    public float spawnInterval = 0.5f;
    public float decalLifetime = 10f;

    private TailController tailController;
    private float timer = 0f;
    private SoundManager _soundManager;
    private Queue<GameObject> bloodTrailPool;

    private void Start()
    {
        tailController = FindFirstObjectByType<TailController>();
        PlayTailSFX();

        // Initialize blood trail decal pool
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
        // Update timer and spawn blood trail if interval reached
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnBloodTrail();
            timer = 0f;
        }
    }

    /// <summary>
    /// Spawn blood trail decal behind the last tail segment
    /// </summary>
    void SpawnBloodTrail()
    {
        if (bloodTrailPool == null || bloodTrailPool.Count == 0 || tailController == null || tailController.segments == null || tailController.segments.Length == 0)
            return;

        // Get the first segment of the tail
        Transform firstSegment = tailController.segments[0];
        Vector3 spawnPosition = firstSegment.position;
        spawnPosition.y = 0.004f; // Slightly above ground to avoid z-fighting

        GameObject decal = bloodTrailPool.Dequeue();
        decal.transform.SetPositionAndRotation(spawnPosition, Quaternion.Euler(90f, 0f, 0f));
        decal.SetActive(true);

        // Schedule decal to be disabled after lifetime
        StartCoroutine(DisableAfterSeconds(decal, decalLifetime));
        bloodTrailPool.Enqueue(decal);
    }

    /// <summary>
    /// Coroutine to disable decal after specified seconds
    /// </summary>
    private System.Collections.IEnumerator DisableAfterSeconds(GameObject obj, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        obj.SetActive(false);
    }

    /// <summary>
    /// Play tail sound effect
    /// </summary>
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
