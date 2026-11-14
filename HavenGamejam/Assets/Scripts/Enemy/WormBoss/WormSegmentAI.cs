using UnityEngine;

public class WormSegment : MonoBehaviour
{
    [Tooltip("Set to -90 if your sprite faces UP by default, or 0 if it faces RIGHT.")]
    public float rotationOffset = -90f;

    /// <summary>
    /// Updates the segment's position and rotates it to face the segment ahead of it.
    /// </summary>
    public void UpdateSegment(Vector2 targetPosition, Vector2 lookAtPosition)
    {
        transform.position = targetPosition;

        Vector2 direction = lookAtPosition - targetPosition;
        if (direction.sqrMagnitude > 0.0001f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + rotationOffset;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}