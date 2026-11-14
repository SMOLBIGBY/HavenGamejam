using UnityEngine;
using System.Collections;

public class EnemyProjectileScript : MonoBehaviour
{
    public float moveSpeed = 40f;

    void Start()
    {
        // Start the coroutine to destroy the projectile after 3 seconds
        StartCoroutine(ExistTime(2f));
    }
    void Update()
    {
        // Move in the object's local right (or left) direction
        // transform.right corresponds to the red axis of the object in the Scene view
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.Self);
    }

    IEnumerator ExistTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(this.gameObject);
    }
}