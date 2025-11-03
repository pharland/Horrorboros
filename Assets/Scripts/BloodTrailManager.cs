using UnityEngine;
using System.Collections.Generic;

public class BloodTrailManager : MonoBehaviour
{
    public GameObject bloodTrailPrefab;
    public int poolSize = 50;
    public float spawnInterval = 0.5f;
    public float decalLifetime = 10f;
    public float shrinkDuration = 1f;
    public float shrinkSizeFactor = 0.2f;

    private TailController tailController;
    private float timer = 0f;
    private SoundManager _soundManager;
    private Queue<GameObject> bloodTrailPool;

    private bool isFirstSegment; // Only true for the first tail segment

    private void Start()
    {
        tailController = FindFirstObjectByType<TailController>();
        PlayTailSFX();

        // Determine if this manager is attached to the first tail segment
        isFirstSegment = false;
        if (tailController != null && tailController.segments != null && tailController.segments.Length > 0)
        {
            // This assumes BloodTrailManager is attached to a tail segment GameObject
            isFirstSegment = (transform == tailController.segments[0]);
        }

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
        // Only spawn blood trails if this is the first segment
        if (!isFirstSegment)
            return;

        // Update timer and spawn decal if interval reached
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnBloodTrail();
            timer = 0f;
        }
    }

    /// <summary>
    /// Spawn blood trail decal behind the first tail segment only
    /// </summary>
    void SpawnBloodTrail()
    {
        if (bloodTrailPool == null || bloodTrailPool.Count == 0 || tailController == null || tailController.segments == null || tailController.segments.Length == 0)
            return;

        Transform firstSegment = tailController.segments[0];
        Vector3 spawnPosition = firstSegment.position;
        spawnPosition.y = 0.004f;

        // Random Z rotation between 0 and 360 degrees
        float randomZ = Random.Range(0f, 360f);
        Quaternion randomRotation = Quaternion.Euler(90f, 0f, randomZ);

        // Dequeue a decal from the pool
        GameObject decal = bloodTrailPool.Dequeue();
        decal.transform.SetPositionAndRotation(spawnPosition, randomRotation);

        // Set random scale between 0.7 and 1 times the original size
        float randomScale = Random.Range(0.7f, 1.5f);
        decal.transform.localScale = Vector3.one * randomScale;

        // Show the decal
        decal.SetActive(true);

        // Start coroutine to disable after lifetime with shrink effect
        StartCoroutine(DisableAfterSeconds(decal, decalLifetime));
        bloodTrailPool.Enqueue(decal);
    }

    private System.Collections.IEnumerator DisableAfterSeconds(GameObject obj, float seconds)
    {
        // Wait for (seconds - 1) before starting shrink
        float waitTime = Mathf.Max(0f, seconds - shrinkDuration);
        yield return new WaitForSeconds(waitTime);

        // Shrink the decal over 1 second
        Vector3 initialScale = obj.transform.localScale;
        Vector3 targetScale = Vector3.one * shrinkSizeFactor;
        float elapsed = 0f;
        while (elapsed < shrinkDuration)
        {
            obj.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsed / shrinkDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        obj.transform.localScale = targetScale;

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
