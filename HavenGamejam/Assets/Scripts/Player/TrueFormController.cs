using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class TrueFormController : MonoBehaviour
{
    Animator animator;
    private PlayerManager playerManager;
    WeaponScript weaponScript;



    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [Header("Jump")]
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float normalGravity = 3f;
    [SerializeField] private float fallGravity = 6f;
    [SerializeField] private float lowJumpGravity = 8f;
    [SerializeField] private float maxFallSpeed = 11f;
    [SerializeField] private float groundCheckOffset = 0.6f;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.8f, 0.1f);
    [SerializeField] private LayerMask groundLayer;

    [Header("Physics Materials")]
    public PhysicsMaterial2D frictionlessMat;
    public PhysicsMaterial2D highFrictionMat;

    [Header("Collider Settings")]
    [SerializeField] private Collider2D playerCollider;

    [SerializeField] private bool isGrounded;

    [SerializeField] GameObject Weapon;

    float jumpButtonPressedTime = 0f;
    bool jumping;

    [Header("Sprite")]

    [SerializeField] private SpriteRenderer weaponRenderer;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] private Transform weapon;
    [SerializeField] private Vector3 weaponOffset;



    [Header("Footstep Sound")]
    [SerializeField] private AudioClip footstepClip;
    [SerializeField][Range(0f, 1f)] private float footstepVolume = 0.7f;
    [SerializeField] private float minPitch = 0.9f;
    [SerializeField] private float maxPitch = 1.1f;
    [SerializeField] private float walkStepInterval = 0.45f;
    [SerializeField] private float runStepInterval = 0.3f;



    [Header("Parry Settings")]
    [SerializeField] public bool isParrying = false;
    [SerializeField] private bool IsAbleToParry = true;
    [SerializeField] private float parryWindow = 0.1f;
    [SerializeField] private float parryDuration = 0.5f;
    [SerializeField] private float parryCooldown = 0.7f;

    [SerializeField] private bool isParrySuccessful = false;

    [SerializeField] GameObject ParryHitbox;
    [SerializeField] GameObject Hitbox;

    private float moveInput;
    private Rigidbody2D rb;
    private AudioSource audioSource;
    private bool isFacingRight = true;
    private bool isRunning = false;

    private float currentSpeed;
    private float stepTimer;

    private float initialWeaponX;

    void Awake()
    {
        maxFallSpeed = 11f;
        parryCooldown = 0.7f;
        jumpForce = 15f;
        parryWindow = 0.1f;
        parryDuration = 0.5f;
        playerManager = FindAnyObjectByType<PlayerManager>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        playerCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        weaponScript = GetComponentInChildren<WeaponScript>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;

        currentSpeed = walkSpeed;

        // Cache the base positive X offset of the weapon
        if (weapon != null)
        {
            initialWeaponX = Mathf.Abs(weapon.localPosition.x);
        }

        ParryHitbox.SetActive(false);
        Hitbox.SetActive(true);
    }

    void Update()
    {

        if (isParrying)
        {
            Weapon.SetActive(false);
        }
        else
        {
            Weapon.SetActive(true);
        }
        FaceCursor();


        animator.SetBool("IsRunning", Mathf.Abs(rb.linearVelocity.x) > 0.1f && isGrounded);
        animator.SetBool("IsFlying", rb.linearVelocity.y != 0 && !isGrounded);

        animator.SetBool("IsIdle",
        Mathf.Abs(rb.linearVelocity.x) < 0.1f &&
        isGrounded);

        Vector2 checkPosition = transform.position + Vector3.down * groundCheckOffset;

        isGrounded = Physics2D.OverlapBox(checkPosition, groundCheckSize, 0f, groundLayer);
        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && playerManager.CanMove && isParrying == false)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetTrigger("WingAnimation");
        }

        if (Mathf.Abs(moveInput) < 0.01f && isGrounded)
        {
            playerCollider.sharedMaterial = highFrictionMat;
        }
        else
        {
            playerCollider.sharedMaterial = frictionlessMat;
        }

        // Variable jump
        if (rb.linearVelocity.y > 0)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                // Holding jump = floatier
                rb.gravityScale = normalGravity;

            }
            else
            {
                // Released early = shorter jump
                rb.gravityScale = lowJumpGravity;
            }
        }
        else
        {
            // Falling
            rb.gravityScale = fallGravity;
        }
    




        HandleFootsteps();

        if (!playerManager.CanMove)
        {
            currentSpeed = 0f;
        }
        else if (currentSpeed == 0f)
        {
            currentSpeed = 5f;
        }

        //if (playerManager.CanMove)
        //{
        //    LaserGun.SetActive(true);
        //}
        //else
        //{
        //    LaserGun.SetActive(false);
        //}


        if(Input.GetKeyDown(KeyCode.F) && IsAbleToParry && playerManager.CanMove)
        {
            animator.SetTrigger("Parry");
            StartCoroutine(StartParry());
        }
    }



    void FaceCursor()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (mousePosition.x > transform.position.x && !isFacingRight && playerManager.CanMove)
        {
            Flip();
        }
        else if (mousePosition.x < transform.position.x && isFacingRight && playerManager.CanMove)
        {
            Flip();
        }
    }


    void FixedUpdate()
    {
            if (!isParrying)
            {
                rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
            }
        
            rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            Mathf.Max(rb.linearVelocity.y, -maxFallSpeed)
        );
    }


    void HandleFootsteps()
    {
        if (footstepClip == null)
            return;

        if (moveInput == 0)
        {
            stepTimer = 0f;
            return;
        }

        stepTimer -= Time.deltaTime;

        float currentStepInterval = isRunning ? runStepInterval : walkStepInterval;

        if (stepTimer <= 0f)
        {
            PlayFootstep();
            stepTimer = currentStepInterval;
        }
    }

    void PlayFootstep()
    {
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(footstepClip, footstepVolume);
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;

        // Flip player sprite
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !isFacingRight;
        }

        // Reset WeaponPoint scale to positive so it doesn't mess up local rotations
        if (weapon != null)
        {
            Vector3 localScale = weapon.localScale;
            localScale.x = Mathf.Abs(localScale.x);
            weapon.localScale = localScale;
        }
    }
    private void OnDrawGizmos()
    {
        Vector2 checkPosition = transform.position + Vector3.down * groundCheckOffset;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(checkPosition, groundCheckSize);
    }
    IEnumerator StartParry()
    {
        isParrying = true;
        fallGravity = -1f;
        rb.linearVelocity *= 0.5f;
        IsAbleToParry = false;

        yield return new WaitForSeconds(parryWindow);
        ParryHitbox.SetActive(true);
        Hitbox.SetActive(false);
        animator.SetBool("IsParrying", true);

        yield return new WaitForSeconds(parryDuration);
        ParryHitbox.SetActive(false);
        if (!isParrySuccessful)
        {
            Hitbox.SetActive(true);

        }
        else
        {
            // If parry was successful, you might want to keep the Hitbox disabled for a short duration
            StartCoroutine(ParrySuccess(2f)); // Adjust the duration as needed
        }
        isParrying = false  ;
        animator.SetBool("IsParrying", false);
        fallGravity = 6f;
        rb.linearVelocity *= 1f;

        yield return new WaitForSeconds(parryCooldown);
        IsAbleToParry = true;
        isParrySuccessful = false;
    }

    IEnumerator ParrySuccess(float delay)
    {

        yield return new WaitForSeconds(delay);
        Hitbox.SetActive(true);

    }

    public void Parried()
    {
        if(!isParrySuccessful)
        {
            // This method can be called when the player successfully parries an attack
            Debug.Log("Parry successful!");
            isParrySuccessful = true;
            weaponScript.AmmoCount = weaponScript.MaxAmmoCount; // Increase ammo count by 1

            ImpactFeedbackManager.Instance.TriggerHitstop(0.06f);

            // 2. Full ULTRAKILL parry (hitstop + bright screen flash)
            ImpactFeedbackManager.Instance.TriggerParryFeedback(hitstopDuration: 0.12f, flashDuration: 0.15f);

            // 3. Custom colored flash (e.g., red flash on player damage)
            ImpactFeedbackManager.Instance.TriggerParryFlash(0.2f, new Color(1f, 1f, 1f, 0.7f));
        }
    }
}