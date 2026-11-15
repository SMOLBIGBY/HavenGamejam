using System.Collections;
using UnityEngine;

public class FalseSun : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint; // Optional: defaults to boss center if left empty

    FalseSunManager falseSunManager;

    [Header("General Settings")]
    [SerializeField] private float timeBetweenAttacks = 2.0f;

    [Header("Spiral Attack Settings")]
    [SerializeField] private float spiralDuration = 5.0f;
    [SerializeField] private float spiralFireRate = 0.08f; // Lower = denser spiral
    [SerializeField] private float spiralRotationSpeed = 120f; // Degrees per second
    [SerializeField] private int spiralStreams = 2; // 1 = single spiral, 2 = double helix, etc.

    [Header("Burst Attack Settings")]
    [SerializeField] private int burstCount = 3; // How many rings to fire
    [SerializeField] private int projectilesPerBurst = 24; // Bullets per ring
    [SerializeField] private float timeBetweenBursts = 0.6f;

    private float currentAngle = 0f;

    private void Start()
    {
        falseSunManager = FindAnyObjectByType<FalseSunManager>();
        if (firePoint == null)
            firePoint = transform;

        // Start the main attack loop
        StartCoroutine(Intro(3f));
    }
    void Update()
    {
        spiralFireRate = 0.08f / falseSunManager.Multiplier;
        timeBetweenBursts = 0.6f / falseSunManager.Multiplier;
    }
    IEnumerator Intro(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(AttackLoop());
    }

    private IEnumerator AttackLoop()
    {
        while (true)
        {
            // Attack 1: Spiral
            yield return StartCoroutine(SpiralAttack());
            yield return new WaitForSeconds(timeBetweenAttacks);

            // Attack 2: Radial Burst
            yield return StartCoroutine(BurstAttack());
            yield return new WaitForSeconds(timeBetweenAttacks);
        }
    }

    private IEnumerator SpiralAttack()
    {
        float timer = 0f;

        while (timer < spiralDuration)
        {
            // Calculate the angle offset between each stream (e.g., 180 degrees apart for 2 streams)
            float angleStep = 360f / spiralStreams;

            for (int i = 0; i < spiralStreams; i++)
            {
                float spawnAngle = currentAngle + (angleStep * i);
                SpawnProjectile(spawnAngle);
            }

            // Advance the angle for the next shot to create the spiral effect
            currentAngle += spiralRotationSpeed * spiralFireRate;
            if (currentAngle >= 360f) currentAngle -= 360f;

            timer += spiralFireRate;
            yield return new WaitForSeconds(spiralFireRate);
        }
    }

    private IEnumerator BurstAttack()
    {
        float angleStep = 360f / projectilesPerBurst;

        for (int b = 0; b < burstCount; b++)
        {
            // Optional: offset every second burst slightly so the player has to weave
            float offset = (b % 2 == 1) ? (angleStep / 2f) : 0f;

            for (int i = 0; i < projectilesPerBurst; i++)
            {
                float spawnAngle = (i * angleStep) + offset;
                SpawnProjectile(spawnAngle);
            }

            yield return new WaitForSeconds(timeBetweenBursts);
        }
    }

    private void SpawnProjectile(float angleInDegrees)
    {
        // NOTE: This assumes a 2D game where projectiles move along their local UP (transform.up) 
        // or RIGHT (transform.right) axis. See notes below if using 3D!
        Quaternion rotation = Quaternion.Euler(0f, 0f, angleInDegrees);
        Instantiate(projectilePrefab, firePoint.position, rotation);
    }
}