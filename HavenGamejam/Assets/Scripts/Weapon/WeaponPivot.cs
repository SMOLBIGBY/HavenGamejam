using UnityEngine;

public class WeaponPoint : MonoBehaviour
{
    private PlayerManager playerManager;
    private PlayerController playerController;
    [SerializeField] private SpriteRenderer weaponRenderer;
    private Vector3 initialLocalPos;

    void Start()
    {
        playerManager = FindAnyObjectByType<PlayerManager>();
        playerController = GetComponentInParent<PlayerController>();
        
        weaponRenderer = GetComponentInChildren<SpriteRenderer>();
        if (weaponRenderer == null)
        {
            weaponRenderer = GetComponent<SpriteRenderer>();
        }

        // Cache the default offset position relative to the player
        initialLocalPos = transform.localPosition;
    }

    void FixedUpdate()
    {
        if (playerManager != null && playerManager.CanMove)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mousePosition - transform.position;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Rotate toward cursor
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // Determine if aiming to the left or right relative to the pivot
            bool aimingLeft = direction.x < 0;

            // Flip weapon sprite horizontally (flipX) depending on aim direction 
            // so it never looks like it's backing into the player
            if (weaponRenderer != null)
            {
                
                // If your weapon sprite also needs vertical flipping when pointing down-left/up-left:
                weaponRenderer.flipY = aimingLeft; 
            }
        }
    }
}