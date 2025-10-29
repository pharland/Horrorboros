using System;
using System.Collections.Generic;
using UnityEngine;

public class TailController : MonoBehaviour
{
    public Transform[] segments;
    public float segmentSpacing; // Distance between segments
    public float segmentMinSize;
    public float segmentMaxSize;
    public GameObject segmentPrefab;

    private readonly List<Vector3> _positionHistory = new(); // History of leader tail positions

    void LateUpdate()
    {
        // Record the current position of the leader
        _positionHistory.Insert(0, transform.position);

        // Limit history to what's needed
        int maxHistory = Mathf.CeilToInt(segments.Length / segmentSpacing) * 2;
        if (_positionHistory.Count > maxHistory)
            _positionHistory.RemoveAt(_positionHistory.Count - 1);

        // Move each segment to the position the leader was at a certain time ago
        for (int i = 0; i < segments.Length; i++)
        {
            int index = Mathf.Min(Mathf.FloorToInt((i + 1) / segmentSpacing), _positionHistory.Count - 1);
            segments[i].position = _positionHistory[index];
        }
    }

    // Add a new segment to the tail
    public void AddSegment()
    {
        if (segments.Length == 0 || segmentPrefab == null) return;

        Transform lastSegment = segments[^1];
        Vector3 newPosition = lastSegment.position;

        // Generate random rotation
        Quaternion randomRotation = UnityEngine.Random.rotation;

        // Generate random uniform scale
        float randomScale = UnityEngine.Random.Range(segmentMinSize, segmentMaxSize);

        // Spawn the tail prefab at the end of the tail chain
        GameObject newSegment = Instantiate(segmentPrefab, newPosition, randomRotation, transform);
        newSegment.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

        // Add the new segment to the segments array
        Array.Resize(ref segments, segments.Length + 1);
        segments[^1] = newSegment.transform;
    }
}
