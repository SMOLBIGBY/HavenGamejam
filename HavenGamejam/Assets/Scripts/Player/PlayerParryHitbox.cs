using UnityEngine;

public class PlayerParryHitbox : MonoBehaviour
{
    TrueFormController playerController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       playerController = GetComponentInParent<TrueFormController>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DamageHitbox"))
        {
            playerController.Parried();
        }
    }
}
