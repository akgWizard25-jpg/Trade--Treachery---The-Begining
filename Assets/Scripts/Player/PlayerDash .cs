using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashSpeed = 20f;        // Dash speed multiplier
    public float dashDuration = 0.2f;    // How long the dash lasts
    public float dashCooldown = 1f;      // Time before dash can be used again

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 lastMoveDirection;
    private bool isDashing = false;
    private float dashCooldownTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Countdown cooldown timer
        dashCooldownTimer -= Time.deltaTime;
    }

    // Called by Input System when move stick or WASD is used
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        // Remember last movement direction
        if (moveInput.sqrMagnitude > 0.01f)
        {
            lastMoveDirection = moveInput.normalized;
        }
    }

    // Called by Input System when Dash button pressed
    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started && !isDashing && dashCooldownTimer <= 0f)
        {
            StartCoroutine(Dash());
        }
    }

    private System.Collections.IEnumerator Dash()
    {
        isDashing = true;
        dashCooldownTimer = dashCooldown;

        // Determine direction to dash
        Vector2 dashDirection;

        if (moveInput.sqrMagnitude > 0.01f)
        {
            dashDirection = moveInput.normalized; // dash where the player is currently moving
        }
        else if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            dashDirection = rb.linearVelocity.normalized; // dash in current movement direction
        }
        else
        {
            dashDirection = lastMoveDirection; // last known input direction
        }

        float startTime = Time.time;

        while (Time.time < startTime + dashDuration)
        {
            rb.linearVelocity = dashDirection * dashSpeed;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        isDashing = false;
    }
}




