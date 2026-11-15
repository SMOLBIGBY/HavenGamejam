using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    public GameObject objectToSpawn;       // The prefab you want to spawn
    public TrueFormController playerManager; // Reference to the player's TrueFormController script
    public float spawnOffsetDistance = 2f; // How far to the right it should spawn

    public float fireCooldown = 1f;        // Cooldown duration in seconds
    private float nextFireTime = 0f;       // Timestamp when the weapon can fire again

    // Read-only property so other scripts can still check IsAbleToFire without accidentally locking it
    public bool IsAbleToFire => Time.time >= nextFireTime;

    public int AmmoCount = 5;
    public int MaxAmmoCount = 5;

    public AudioClip FiringSound;



    Animator animator;

    void Start()
    {
        playerManager = FindAnyObjectByType<TrueFormController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        AmmoCount = Mathf.Clamp(AmmoCount, 0, MaxAmmoCount); // Ensure AmmoCount doesn't exceed MaxAmmoCount

        if (Input.GetKeyDown(KeyCode.Mouse0) && IsAbleToFire && AmmoCount > 0)
        {
            AudioManager.Instance.PlaySFX(FiringSound);
            SpawnToTheRight();
            nextFireTime = Time.time + fireCooldown;
            AmmoCount -= 1;
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
}