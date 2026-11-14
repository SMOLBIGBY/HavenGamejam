using UnityEngine;
using System.Collections;

public class WeaponScript : MonoBehaviour
{
    public GameObject objectToSpawn;       // The prefab you want to spawn
    public float spawnOffsetDistance = 2f; // How far to the right it should spawn

    public bool IsAbleToFire = true; // Flag to check if the weapon can fire

    void Start()
    {
        // Ensure the weapon can fire at the start
        IsAbleToFire = true;
    }
    void Update()
    {
        // Example: Press Spacebar to trigger the spawn
        if (Input.GetKeyDown(KeyCode.Mouse0) && IsAbleToFire)
        {
            SpawnToTheRight();
            IsAbleToFire = false; // Set to false to prevent immediate firing again
            StartCoroutine(RechargeDelayCoroutine(0.2f));
        }
    }

    void SpawnToTheRight()
    {
        if (objectToSpawn == null) return;

        // 1. Calculate position: Object's position + (Local Right direction * distance)
        // transform.right automatically changes when the object rotates!
        Vector3 spawnPosition = transform.position + (transform.right * spawnOffsetDistance);

        // 2. Instantiate the object at that position (matching the spawner's rotation)
        Instantiate(objectToSpawn, spawnPosition, transform.rotation);
    }



    public IEnumerator RechargeDelayCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        IsAbleToFire = true;
    }

}