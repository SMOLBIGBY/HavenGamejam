using UnityEngine;
using UnityEngine.UI;

public class NewMonoBehaviourScript : MonoBehaviour
{
    WormBossManager wormManager;
    [SerializeField] Image bossHealthBar;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        wormManager = FindFirstObjectByType<WormBossManager>();
    }

    // Update is called once per frame
    void Update()
    {
        bossHealthBar.fillAmount = wormManager.currentHealth / wormManager.maxHealth;
    }
}
