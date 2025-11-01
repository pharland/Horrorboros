using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TailController : MonoBehaviour
{
    public Transform[] segments;
    public float firstSpawnDistance; // Distance to spawn the first segment behind the player
    public float segmentSpacing; // Distance between segments
    public float segmentMinSize;
    public float segmentMaxSize;
    public float segmentSpawnHeight;
    public GameObject[] segmentPrefabs;

    private readonly List<Vector3> _positionHistory = new(); // History of leader tail positions
    private Vector3[] segmentDirections; // Directions of each segment
    private int _lastPrefabIndex = -1; // To avoid spawning the same prefab consecutively

    public float waveCurveIntensity = 0.5f; // How far segments wave from the path
    public float waveCurveFrequency = 2f;   // How many waves per tail length
    public float waveCurveSpeed = 2f;       // How fast the wave animates

    void Start()
    {
        segmentDirections = new Vector3[segments.Length];
        // Fill position history with the leader's starting position
        for (int i = 0; i < segments.Length * 2; i++)
        {
            _positionHistory.Add(transform.position);
        }
    }

    void FixedUpdate()
    {
        // Record current position of the leader segment
        _positionHistory.Insert(0, transform.position);

        // Limit history size (for performance reasons)
        int maxHistory = Mathf.CeilToInt(segments.Length / segmentSpacing) * 2;
        if (_positionHistory.Count > maxHistory)
            _positionHistory.RemoveAt(_positionHistory.Count - 1);
    }

    void LateUpdate()
    {
        // Calculate time for wave animation
        float time = Time.time * waveCurveSpeed;

        // Update segment positions based on history
        for (int i = 0; i < segments.Length; i++)
        {
            // Determine which two history points to interpolate between
            float floatIndex = (i + 1) / segmentSpacing; // +1 to offset from leader
            int indexA = Mathf.FloorToInt(floatIndex);// Lower index
            int indexB = Mathf.Min(indexA + 1, _positionHistory.Count - 1);
            float t = floatIndex - indexA;

            // Interpolate position
            Vector3 posA = _positionHistory[Mathf.Min(indexA, _positionHistory.Count - 1)];
            Vector3 posB = _positionHistory[indexB];

            // Set segment position
            Vector3 newPos = Vector3.Lerp(posA, posB, t);
            newPos.y = segmentSpawnHeight; 
            
            // Calculate movement direction
            Vector3 moveDir = (posB - posA).normalized;

            // Calculate perpendicular direction for wave offset
            Vector3 perpDir = Vector3.Cross(moveDir, Vector3.up).normalized;

            // Calculate wave offset
            float wavePhase = time + i * waveCurveFrequency;
            float waveOffset = Mathf.Sin(wavePhase) * waveCurveIntensity;

            // Apply wave offset
            newPos += perpDir * waveOffset;

            segments[i].position = newPos;

            // Only update rotation if direction changed significantly
            if (segmentDirections[i] != moveDir && moveDir.sqrMagnitude > 0.0001f)
            {
                segments[i].rotation = Quaternion.LookRotation(moveDir, Vector3.up);
                segmentDirections[i] = moveDir;
            }
        }
    }

    // Add a new segment to the tail
    public void AddSegment()
    {
        if (segmentPrefabs == null || segmentPrefabs.Length == 0) return;

        Vector3 newPosition;

        // Determine spawn position for new segment
        if (segments.Length == 0)
        {
            float spawnDistance = firstSpawnDistance;
            newPosition = transform.position - transform.forward * spawnDistance;
        }
        else
        {
            newPosition = segments[^1].position;
        }
        newPosition.y = segmentSpawnHeight;

        // Select a random tail prefab index, but not the same as last time
        int prefabIndex;
        do
        {
            prefabIndex = UnityEngine.Random.Range(0, segmentPrefabs.Length);
        } while (segmentPrefabs.Length > 1 && prefabIndex == _lastPrefabIndex);
        _lastPrefabIndex = prefabIndex;
        GameObject prefabToSpawn = segmentPrefabs[prefabIndex];

        // Create new segment with random rotation and scale
        Quaternion randomRotation = UnityEngine.Random.rotation;
        float randomScale = UnityEngine.Random.Range(segmentMinSize, segmentMaxSize);

        // Set the parent to be the same as the player's transform
        Transform playerTransform = transform.parent;
        GameObject newSegment = Instantiate(prefabToSpawn, newPosition, randomRotation, playerTransform);
        newSegment.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

        // Add new segment to arrays
        Array.Resize(ref segments, segments.Length + 1);
        segments[^1] = newSegment.transform;

        // Resize segmentDirections array
        Array.Resize(ref segmentDirections, segments.Length);

        // Initialize new direction
        _positionHistory.Add(newPosition);
    }
}
