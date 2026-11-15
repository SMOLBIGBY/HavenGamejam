using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class WormBossManager : MonoBehaviour
{
    BlackScreenFade blackScreenFade;
    [SerializeField] public float maxHealth = 1000f;
    [SerializeField] public float currentHealth = 1000f;

    public float CurrentHealthToMaxHealthRatio = 1f;
    public float Multiplier = 1f;

    void Start()
    {
        blackScreenFade = FindFirstObjectByType<BlackScreenFade>();
        currentHealth = maxHealth;
        UpdateMultiplier();
    }

    void Update()
    {
        UpdateMultiplier();
    }

    private void UpdateMultiplier()
    {
        // Guard against dividing by zero if maxHealth is 0
        if (maxHealth <= 0f) return;

        // Clamp currentHealth so it doesn't go negative and break the ratio
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        // Fixed: currentHealth divided by maxHealth (gives 1.0 at full HP, 0.2 at 20% HP)
        CurrentHealthToMaxHealthRatio = currentHealth / maxHealth;

        // Smooth curve: 1.0x at 100% HP, ~2.0x at 20% HP, 2.5x at 0% HP
        float missingHealth = 1f - CurrentHealthToMaxHealthRatio;
        Multiplier = 1f + 1.5f * Mathf.Pow(missingHealth, 1.8f);

        Debug.Log(Multiplier);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        UpdateMultiplier();

        if (currentHealth <= 0f)
        {
            StartCoroutine(Die(1f));
        }
    }

    IEnumerator Die(float delay)
    {
        blackScreenFade.FadeIn(0.5f);
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("BossFight2");
    }
}