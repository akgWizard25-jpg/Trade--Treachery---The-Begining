using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private Rigidbody2D rb;
    private PlayerInput playerInput;
    private Vector2 moveVector;
    private bool isDashing = false;
    private float dashCooldownTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        dashCooldownTimer -= Time.deltaTime;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveVector = context.ReadValue<Vector2>();
    }

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

        Vector2 dashDirection = moveVector.normalized;
        if (dashDirection == Vector2.zero)
            dashDirection = Vector2.up; // default forward dash if no input

        float startTime = Time.time;

        while (Time.time < startTime + dashDuration)
        {
            rb.linearVelocity = dashDirection * dashSpeed;
            yield return null;
        }

        isDashing = false;
        rb.linearVelocity = Vector2.zero;
    }
}

