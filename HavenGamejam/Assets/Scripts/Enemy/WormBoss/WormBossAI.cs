using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyCharger : MonoBehaviour
{
    [Header("Movement & Rotation Settings")]
    public float moveSpeed = 6f;
    public float turnSpeed = 60f;          // Degrees per second while chasing
    public float windupTurnSpeed = 90f;     // Slower turn speed while aiming a charge
    public float detectionRange = 10f;
    [Tooltip("Set to -90 if your sprite faces UP by default, or 0 if it faces RIGHT.")]
    public float rotationOffset = -90f;

    [Header("Charge Settings")]
    public float chargeSpeed = 12f;
    public float chargeDuration = 0.35f;
    public float chargeWindup = 0.5f;       // Pause before dashing so the player can react
    public float chargeCooldown = 3f;       // Time between charges

    [SerializeField] GameObject chargeIndicator ;         // Drag your child DamageHitbox here

    [Header("References")]
    public GameObject damageHitbox;         // Drag your child DamageHitbox here
    public bool onlyDamageDuringCharge = false;

    WormBossManager wormBossManager;

    private Rigidbody2D rb;
    private Transform player;
    private bool isCharging = false;
    private float cooldownTimer;

    void Start()
    {
        wormBossManager = FindFirstObjectByType<WormBossManager>();
        chargeIndicator.SetActive(false);
        rb = GetComponent<Rigidbody2D>();
        cooldownTimer = chargeCooldown;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        if (damageHitbox != null && onlyDamageDuringCharge)
        {
            damageHitbox.SetActive(false);
        }
    }

    void Update()
    {
        moveSpeed = 6 * wormBossManager.Multiplier;
        turnSpeed = 60 * wormBossManager.Multiplier;
        if (player == null || isCharging) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0f)
            {
                StartCoroutine(ChargeRoutine());
            }
    }

    void FixedUpdate()
    {
        if (player == null || isCharging) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        RotateTowardsPlayer(turnSpeed);

        // 2. Move in the direction the boss is currently facing (not directly at the player)
        Vector2 facingDirection = GetFacingDirection();
        rb.linearVelocity = facingDirection * moveSpeed; // Use rb.velocity if on Unity 2022 or older
    }

    private IEnumerator ChargeRoutine()
    {
        isCharging = true;
        rb.linearVelocity = Vector2.zero;

        // 1. Windup: Boss stops moving and slowly turns to track the player
        float windupTimer = 0f;
        while (windupTimer < chargeWindup)
        {
            chargeIndicator.SetActive(true);
            RotateTowardsPlayer(windupTurnSpeed);
            windupTimer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // 2. Execute Charge: Lock in the direction the boss is facing and dash
        if (damageHitbox != null && onlyDamageDuringCharge)
        {
            damageHitbox.SetActive(true);
        }

        Vector2 chargeDirection = GetFacingDirection();
        rb.linearVelocity = chargeDirection * chargeSpeed;
        chargeIndicator.SetActive(false);
        yield return new WaitForSeconds(chargeDuration);

        // 3. Recover
        rb.linearVelocity = Vector2.zero;
        if (damageHitbox != null && onlyDamageDuringCharge)
        {
            damageHitbox.SetActive(false);
        }

        cooldownTimer = chargeCooldown;
        isCharging = false;
    }

    private void RotateTowardsPlayer(float speed)
    {
        Vector2 direction = (player.position - transform.position).normalized;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + rotationOffset;
        
        // Smoothly step the Z rotation toward the target angle
        float newAngle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, speed * Time.fixedDeltaTime);
        rb.MoveRotation(newAngle);
    }

    private Vector2 GetFacingDirection()
    {
        // Convert the current Rigidbody angle back into a forward direction vector
        float angleRad = (rb.rotation - rotationOffset) * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }
}