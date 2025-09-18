using UnityEngine;
using UnityEngine.InputSystem; // Required for new Input System

[RequireComponent(typeof(Rigidbody2D))]
public class ShipMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float acceleration = 5f;   // Forward/backward thrust
    public float maxSpeed = 10f;      // Maximum ship speed
    public float turnSpeed = 150f;    // How fast the ship turns
    public float waterDrag = 0.98f;   // Resistance from water

    private Rigidbody2D rb;

    // Input values
    private float moveInput;
    private float turnInput;

    // This will be called by PlayerInput (Send Messages) when Move triggers
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 v = context.ReadValue<Vector2>();
        // convention: y = forward/back, x = left/right for turning
        moveInput = v.y;
        turnInput = v.x;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
    }

    private void FixedUpdate()
    {
        HandleMovement();
        ApplyDrag();
    }

       private void HandleMovement()
    {
        // Apply forward/backward thrust
        if (Mathf.Abs(moveInput) > 0.01f)
        {
            Vector2 force = transform.up * (moveInput * acceleration);
            rb.AddForce(force);
        }

        // Cap max speed
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        // Apply turning
        if (Mathf.Abs(turnInput) > 0.01f)
        {
            float rotation = -turnInput * turnSpeed * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation + rotation);
        }
    }

    private void ApplyDrag()
    {
        rb.linearVelocity *= waterDrag;
    }
}
