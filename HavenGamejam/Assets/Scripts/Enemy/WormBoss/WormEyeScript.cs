using UnityEngine;

public class WormEyeScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject prefab;
    public float minCooldown = 4f;
    public float maxCooldown = 9f;

    private Transform player;
    private float timer;
    private float currentCooldown;

    void Start()
    {
        minCooldown = 4f;
        maxCooldown = 9f;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        currentCooldown = Random.Range(minCooldown, maxCooldown);
    }

    void Update()
    {
        if (player == null) return;

        timer += Time.deltaTime;
        if (timer >= currentCooldown)
        {
            Spawn();
            timer = 0f;
            currentCooldown = Random.Range(minCooldown, maxCooldown);
        }
    }

    void Spawn()
    {
        Vector2 direction = player.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        Instantiate(prefab, transform.position, rotation);
    }
}