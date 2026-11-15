using UnityEngine;


public class FalseSunHitbox : MonoBehaviour
{
    private FalseSunManager falseSunManager;
    private ICanBeFlashed eyeFlasher;

    [SerializeField] private GameObject eye;

    void Start()
    {
        falseSunManager = FindFirstObjectByType<FalseSunManager>();

        // Cache the interface reference once at startup
        if (eye != null)
        {
            eyeFlasher = eye.GetComponent<ICanBeFlashed>();
            
            // Fallback: check children if it wasn't on the root 'eye' object
            if (eyeFlasher == null)
            {
                eyeFlasher = eye.GetComponentInChildren<ICanBeFlashed>();
            }
        }

        if (eyeFlasher == null)
        {
            Debug.LogWarning($"No component implementing ICanBeFlashed found on {eye?.name ?? "null eye"}!", this);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerProjectile"))
        {
            Debug.Log("Enemy Hit by Player Projectile");
            
            if (falseSunManager != null)
            {
                falseSunManager.TakeDamage(30);
            }

            // Trigger the flash safely using null-conditional operator
            eyeFlasher?.Flash();
        }
    }
}