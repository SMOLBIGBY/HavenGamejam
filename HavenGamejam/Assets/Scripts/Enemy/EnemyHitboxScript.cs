using UnityEngine;

public class EnemyHitboxScript : MonoBehaviour
{
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth = 100f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxHealth = 100f;
        currentHealth = maxHealth; // Initialize current health to max at the start
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerProjectile"))
        {
            Debug.Log("Enemy Hit by Player Projectile");
            TakeDamage(10f); // Example damage value
        }
    }

    void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Handle enemy death (e.g., play animation, drop loot, etc.)
        Destroy(this.gameObject);
    }
}
