using UnityEngine;
using UnityEngine.UI;

public class BossBarSun : MonoBehaviour
{
    FalseSunManager falseSunManager;
    [SerializeField] Image bossHealthBar;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        falseSunManager = FindFirstObjectByType<FalseSunManager>();
    }

    // Update is called once per frame
    void Update()
    {
        bossHealthBar.fillAmount = falseSunManager.currentHealth / falseSunManager.maxHealth;
    }
}
