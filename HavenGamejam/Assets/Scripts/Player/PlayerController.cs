using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    Animator animator;
    private PlayerManager playerManager;



    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float normalGravity = 3f;
    [SerializeField] private float fallGravity = 6f;
    [SerializeField] private float lowJumpGravity = 8f;
    [SerializeField] private float maxFallSpeed = 15f;
    [SerializeField] private float groundCheckOffset = 0.6f;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.8f, 0.1f);
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool isAbleToFly = false;

    [Header("Physics Materials")]
    public PhysicsMaterial2D frictionlessMat;
    public PhysicsMaterial2D highFrictionMat;

    [Header("Collider Settings")]
    [SerializeField] private Collider2D playerCollider;

    [SerializeField] private bool isGrounded;

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

    [Header("Weapon Settings")]

    [SerializeField] GameObject Weapon;

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
        playerManager = FindAnyObjectByType<PlayerManager>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        playerCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;

        currentSpeed = walkSpeed;

        // Cache the base positive X offset of the weapon
        if (weapon != null)
        {
            initialWeaponX = Mathf.Abs(weapon.localPosition.x);
        }
    }

    void Update()
    {
        FaceCursor();


        animator.SetBool("IsRunning", Mathf.Abs(rb.linearVelocity.x) > 0.1f && isGrounded);
        animator.SetBool("IsJumping", rb.linearVelocity.y > 0.1f && !isGrounded);
        animator.SetBool("IsFalling", rb.linearVelocity.y < -0.1f && !isGrounded);
        animator.SetBool("IsIdle",
        Mathf.Abs(rb.linearVelocity.x) < 0.1f &&
        isGrounded);

        Vector2 checkPosition = transform.position + Vector3.down * groundCheckOffset;

        isGrounded = Physics2D.OverlapBox(checkPosition, groundCheckSize, 0f, groundLayer);
        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && playerManager.CanMove || Input.GetKeyDown(KeyCode.Space) && playerManager.CanMove && isAbleToFly)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
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

            rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
        
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
}