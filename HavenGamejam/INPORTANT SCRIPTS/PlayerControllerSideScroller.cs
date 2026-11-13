using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    Animator animator;
    private PlayerManager playerManager;



    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;

    [Header("Sprite")]
    [SerializeField] SpriteRenderer spriteRenderer;

    


    [Header("Footstep Sound")]
    [SerializeField] private AudioClip footstepClip;
    [SerializeField][Range(0f, 1f)] private float footstepVolume = 0.7f;
    [SerializeField] private float minPitch = 0.9f;
    [SerializeField] private float maxPitch = 1.1f;
    [SerializeField] private float walkStepInterval = 0.45f;
    [SerializeField] private float runStepInterval = 0.3f;

    private float moveInput;
    private Rigidbody2D rb;
    private AudioSource audioSource;
    private bool isFacingRight = true;
    private bool isRunning = false;

    private float currentSpeed;
    private float stepTimer;

    void Awake()
    {
        playerManager = FindAnyObjectByType<PlayerManager>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;

        currentSpeed = walkSpeed;
    }

    void Update()
    {

        //if (rb.linearVelocity.x != 0)
        //{
        //    animator.SetTrigger("IsRunning");
        //}
        //else
        //{
        //    animator.SetTrigger("IsIdle");
        //}
        moveInput = Input.GetAxisRaw("Horizontal");

        if (isFacingRight && moveInput < 0)
        {
            Flip();
        }
        else if (!isFacingRight && moveInput > 0)
        {
            Flip();
        }

        HandleFootsteps();

        if (!playerManager.CanMove)
        {
            rb.linearVelocity = Vector2.zero;
            walkSpeed = 0f;
        }
        else if (walkSpeed == 0f)
        {
            rb.linearVelocity = Vector2.zero;
            walkSpeed = 5f;
        }
    }

    void FixedUpdate()
    {
        if (playerManager.CanMove)
        {
            rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
        }
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
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }
}