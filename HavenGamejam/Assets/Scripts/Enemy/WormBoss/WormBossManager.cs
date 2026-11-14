using UnityEngine;

public class WormBossManager : MonoBehaviour
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

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        // Handle enemy death (e.g., play animation, drop loot, etc.)
        Debug.Log("Worm Boss has been defeated!");
    }
}
