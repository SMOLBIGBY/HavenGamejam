using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 7f;

    [Header("Dependencies")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    private AlwaysLoaded _alwaysLoaded;
    private PlayerStatManager _playerStatManager;
    private Rigidbody2D _rb;

    private Vector2 _input;
    private bool _facingRight = true;
    public bool IsRunning = false;
    public bool IsMoving => _input != Vector2.zero;

    private string _lastFacingDirection = "S";

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerStatManager = GetComponent<PlayerStatManager>();
        _alwaysLoaded = GameObject.FindAnyObjectByType<AlwaysLoaded>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        Debug.Log(_lastFacingDirection);
        if (_alwaysLoaded != null && _alwaysLoaded.InCutscene)
        {
            _input = Vector2.zero;
            return;
        }

        _input.x = Input.GetAxisRaw("Horizontal");
        _input.y = Input.GetAxisRaw("Vertical");
        _input = _input.normalized;

        HandleRunning();
        HandleAnimations();
    }

    void FixedUpdate()
    {
        Vector2 targetPosition = _rb.position + _input * moveSpeed * Time.fixedDeltaTime;
        _rb.MovePosition(targetPosition);
    }

    void HandleRunning()
    {
        if (Input.GetKey(KeyCode.LeftShift) && _playerStatManager.CurrentStamina > 1f)
        {
            IsRunning = true;
        }
        else
        {
            IsRunning = false;
        }

        if (IsRunning && IsMoving)
        {
            // actively draining stamina while running
            _playerStatManager.CanRegenStamina = false;
            _playerStatManager.CurrentStamina -= 75f * Time.deltaTime;
            moveSpeed = 10f;
        }
        else
        {
            // not running - use base speed
            moveSpeed = 7f;
        }
        if(Input.GetKeyUp(KeyCode.LeftShift) && _playerStatManager.CurrentStamina > 1f)
        {
            StartCoroutine(_playerStatManager.RecoverStaminaAfterDelay(1f));
        }
    }


    void HandleAnimations()
    {
        if (IsMoving)
        {
            (string animDir, bool faceRight) = GetDirectionAndFlip(_input);
            _lastFacingDirection = animDir;
            _facingRight = faceRight;
            spriteRenderer.flipX = !_facingRight;
            animator.SetTrigger("walk_" + animDir);
        }
        else
        {
            animator.SetTrigger("idle_" + _lastFacingDirection);
        }
    }

    // Returns animation direction (always one of N, NE, E, SE, S) and whether facing right
    (string, bool) GetDirectionAndFlip(Vector2 input)
    {
        float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
        angle = (angle + 360) % 360;

        // EAST quadrant (no flip)
        if (angle >= 337.5f || angle < 22.5f) return ("E", true);
        if (angle < 67.5f) return ("NE", true);
        if (angle < 112.5f) return ("N", true);
        if (angle < 157.5f) return ("NE", false); // NW = NE flipped
        if (angle < 202.5f) return ("E", false);  // W = E flipped
        if (angle < 247.5f) return ("SE", false); // SW = SE flipped
        if (angle < 292.5f) return ("S", true);
        return ("SE", true); // SE quadrant
    }
}
