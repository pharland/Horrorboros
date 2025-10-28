using UnityEngine;
using System.Collections.Generic;

public class TailController : MonoBehaviour
{
    public Transform[] segments;
    public float segmentSpacing; // Distance between segments

    private readonly List<Vector3> _positionHistory = new();

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
}
