using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController2D : MonoBehaviour
{
    Rigidbody2D rigidbody2d;
    Animator animator;

    [Header("Movement")]
    [SerializeField] float speed = 2f;

    [Header("Dash")]
    [SerializeField] float dashSpeed = 8f;
    [SerializeField] float dashDuration = 0.3f;
    [SerializeField] float dashCooldown = 0.5f;

    private float slowMultiplier = 1f;
    private float slowTimer = 0f;

    Vector2 motionVector;
    Vector2 lastMoveDirection = Vector2.down;
    Vector2 dashDirection;

    float lastVerticalDirection = -1f;

    bool isDashing;
    float dashTimer;
    float dashCooldownTimer;

    public bool IsDashing => isDashing;

    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        ReadMovementInput();

        UpdateFacingDirection();

        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;

        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame &&
            !isDashing &&
            dashCooldownTimer <= 0f)
        {
            StartDash();
        }

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0f)
            {
                isDashing = false;
                animator.SetBool("IsDashing", false);
            }
        }

        Vector2 animationDirection = isDashing
            ? GetDashAnimationDirection()
            : lastMoveDirection;

        animator.SetFloat("horizontal", animationDirection.x);
        animator.SetFloat("vertical", animationDirection.y);

        animator.SetBool(
            "IsMoving",
            motionVector != Vector2.zero && !isDashing
        );

        if (slowTimer > 0f)
        {
            slowTimer -= Time.deltaTime;

            if (slowTimer <= 0f)
            {
                slowMultiplier = 1f;
                //Debug.Log("Player speed restored!");
            }
        }
    }

    void UpdateFacingDirection()
    {
        if (motionVector == Vector2.zero || isDashing)
            return;

        if (motionVector.y != 0f)
            lastVerticalDirection = motionVector.y;

        if (motionVector.x != 0f && motionVector.y == 0f)
        {
            lastMoveDirection = new Vector2(
                motionVector.x * 0.71f,
                lastVerticalDirection * 0.71f
            );
        }
        else
        {
            lastMoveDirection = motionVector.normalized;
        }
    }

    Vector2 GetDashAnimationDirection()
    {
        if (dashDirection.y == 0f)
        {
            return new Vector2(
                dashDirection.x * 0.71f,
                lastVerticalDirection * 0.71f
            );
        }

        return dashDirection.normalized;
    }

    void StartDash()
    {
        dashDirection = motionVector != Vector2.zero
            ? motionVector.normalized
            : lastMoveDirection.normalized;

        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;

        animator.SetBool("IsMoving", false);
        animator.SetBool("IsDashing", true);

        animator.Play("Dash", 0, 0f);
    }

    void ReadMovementInput()
    {
        motionVector = Vector2.zero;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.aKey.isPressed ||
            Keyboard.current.leftArrowKey.isPressed)
        {
            motionVector.x = -1f;
        }

        if (Keyboard.current.dKey.isPressed ||
            Keyboard.current.rightArrowKey.isPressed)
        {
            motionVector.x = 1f;
        }

        if (Keyboard.current.sKey.isPressed ||
            Keyboard.current.downArrowKey.isPressed)
        {
            motionVector.y = -1f;
        }

        if (Keyboard.current.wKey.isPressed ||
            Keyboard.current.upArrowKey.isPressed)
        {
            motionVector.y = 1f;
        }
    }

    void FixedUpdate()
    {
        rigidbody2d.linearVelocity = isDashing
            ? dashDirection * dashSpeed
            : motionVector.normalized * speed * slowMultiplier;
    }

    public void ApplySlow(float multiplier, float duration)
    {
        slowMultiplier = Mathf.Clamp(multiplier, 0.1f, 1f);
        slowTimer = Mathf.Max(0f, duration);

        //Debug.Log("Player slowed!");
    }
}
