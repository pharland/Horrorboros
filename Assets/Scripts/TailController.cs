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
    public GameObject segmentPrefab;

    private readonly List<Vector3> _positionHistory = new(); // History of leader tail positions
    private Vector3[] segmentDirections;

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
            newPos.y = 0.5f; 
            segments[i].position = newPos;

            // Calculate movement direction
            Vector3 moveDir = (posB - posA).normalized;

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
        if (segmentPrefab == null) return;

        Vector3 newPosition;

        if (segments.Length == 0)
        {
            float spawnDistance = firstSpawnDistance;
            newPosition = transform.position - transform.forward * spawnDistance;
        }
        else
        {
            newPosition = segments[^1].position;
        }

        // Set the y position low to the ground
        newPosition.y = 0.5f;

        Quaternion randomRotation = UnityEngine.Random.rotation;
        float randomScale = UnityEngine.Random.Range(segmentMinSize, segmentMaxSize);

        Transform playerTransform = transform.parent;
        GameObject newSegment = Instantiate(segmentPrefab, newPosition, randomRotation, playerTransform);
        newSegment.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

        Array.Resize(ref segments, segments.Length + 1);
        segments[^1] = newSegment.transform;

        // Resize segmentDirections to match segments length
        Array.Resize(ref segmentDirections, segments.Length);

        // Add the new segment's position to the history so LateUpdate can safely interpolate
        _positionHistory.Add(newPosition);
    }
}
