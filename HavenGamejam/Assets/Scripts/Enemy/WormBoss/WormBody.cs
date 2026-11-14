using System.Collections.Generic;
using UnityEngine;

public class WormBody : MonoBehaviour
{
    [Header("Prefabs")]
    public WormSegment bodySegmentPrefab;
    public WormSegment tailSegmentPrefab; // Optional: leave empty to use bodySegmentPrefab
    public GameObject eyePrefab;          // Drag your Eye prefab here

    [Header("Worm Settings")]
    public int bodyLength = 8;
    public float segmentSpacing = 0.6f;     // Distance between each segment
    public float recordResolution = 0.05f;  // How frequently path points are recorded

    [Header("Eye Settings")]
    [Range(0f, 1f)]
    public float eyeSpawnChance = 0.2f;     // 0.2 = 20% chance
    public int skipFirstSegments = 4;       // Number of segments behind the head that cannot have eyes
    public bool allowEyeOnTail = false;     // Toggle whether the tail tip can spawn an eye

    private List<WormSegment> spawnedSegments = new List<WormSegment>();
    private List<Vector2> pathHistory = new List<Vector2>();
    private List<float> cumulativeDistances = new List<float>();

    void Start()
    {
        SpawnSegments();
    }

    void LateUpdate()
    {
        if (spawnedSegments.Count == 0) return;

        RecordHeadPosition();
        UpdateSegmentsAlongPath();
    }

    private void SpawnSegments()
    {
        pathHistory.Add(transform.position);
        cumulativeDistances.Add(0f);

        for (int i = 0; i < bodyLength; i++)
        {
            bool isTail = (i == bodyLength - 1);
            WormSegment prefabToSpawn = (isTail && tailSegmentPrefab != null)
                ? tailSegmentPrefab
                : bodySegmentPrefab;

            if (prefabToSpawn == null) continue;

            Vector2 spawnPos = (Vector2)transform.position - (Vector2)transform.up * (segmentSpacing * (i + 1));
            WormSegment seg = Instantiate(prefabToSpawn, spawnPos, transform.rotation);

            int segmentOrder = -(i + 1);
            SpriteRenderer sr = seg.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = segmentOrder;
            }

            // Skip the first 'skipFirstSegments' (indices 0, 1, 2, 3 when set to 4)
            bool canSpawnEye = i >= skipFirstSegments && (!isTail || allowEyeOnTail);

            if (eyePrefab != null && canSpawnEye)
            {
                if (Random.value <= eyeSpawnChance)
                {
                    GameObject eye = Instantiate(eyePrefab, seg.transform);
                    eye.transform.localPosition = Vector3.zero;
                    eye.transform.localRotation = Quaternion.identity;

                    SpriteRenderer eyeSr = eye.GetComponent<SpriteRenderer>();
                    if (eyeSr != null)
                    {
                        eyeSr.sortingOrder = segmentOrder + 1;
                    }
                }
            }

            spawnedSegments.Add(seg);
        }
    }

    private void RecordHeadPosition()
    {
        Vector2 currentHeadPos = transform.position;
        float distMoved = Vector2.Distance(currentHeadPos, pathHistory[0]);

        if (distMoved >= recordResolution)
        {
            pathHistory.Insert(0, currentHeadPos);
            cumulativeDistances.Insert(0, 0f);

            for (int i = 1; i < cumulativeDistances.Count; i++)
            {
                cumulativeDistances[i] += distMoved;
            }

            float maxRequiredDistance = segmentSpacing * (spawnedSegments.Count + 2);
            while (cumulativeDistances.Count > 2 && cumulativeDistances[cumulativeDistances.Count - 1] > maxRequiredDistance)
            {
                pathHistory.RemoveAt(pathHistory.Count - 1);
                cumulativeDistances.RemoveAt(cumulativeDistances.Count - 1);
            }
        }
    }

    private void UpdateSegmentsAlongPath()
    {
        Vector2 previousSegmentPos = transform.position;

        for (int i = 0; i < spawnedSegments.Count; i++)
        {
            if (spawnedSegments[i] == null) continue;

            float targetDistance = segmentSpacing * (i + 1);
            Vector2 segmentPos = GetPointAtDistance(targetDistance);

            spawnedSegments[i].UpdateSegment(segmentPos, previousSegmentPos);
            previousSegmentPos = segmentPos;
        }
    }

    private Vector2 GetPointAtDistance(float targetDistance)
    {
        if (pathHistory.Count == 1) return pathHistory[0];

        for (int i = 0; i < cumulativeDistances.Count - 1; i++)
        {
            if (cumulativeDistances[i] <= targetDistance && cumulativeDistances[i + 1] >= targetDistance)
            {
                float segmentLength = cumulativeDistances[i + 1] - cumulativeDistances[i];
                float t = (segmentLength > 0f) ? (targetDistance - cumulativeDistances[i]) / segmentLength : 0f;
                return Vector2.Lerp(pathHistory[i], pathHistory[i + 1], t);
            }
        }

        return pathHistory[pathHistory.Count - 1];
    }

    private void OnDestroy()
    {
        foreach (WormSegment seg in spawnedSegments)
        {
            if (seg != null) Destroy(seg.gameObject);
        }
    }
}