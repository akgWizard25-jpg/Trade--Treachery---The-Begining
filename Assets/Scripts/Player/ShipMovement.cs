using System;
using UnityEngine;
using UnityEngine.InputSystem; // Required for new Input System

[RequireComponent(typeof(Rigidbody2D))]
public class ShipMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Range(1f,6f)]
    [SerializeField] float acceleration = 5f;   // Forward/backward thrust
    public float maxSpeed = 10f;      // Maximum ship speed
    public float turnSpeed = 150f;    // How fast the ship turns
    public float waterDrag = 0.98f;   // Resistance from water


    private Rigidbody2D rb;

    [Space]
    [Header("Input")]
    [SerializeField] Joystick joystick;
    [SerializeField] bool inverseVerticleMovement = false;
    [SerializeField] bool useKeyboardInput = false;
    private Action updateDel;
    Vector2 moveVector; // set this from your joystick script











    void Update()
    {
        updateDel?.Invoke();
    }

    void MoveInput()
    {
        moveVector=new Vector2(joystick.Horizontal, (inverseVerticleMovement) ? joystick.Vertical * -1 : joystick.Vertical);
    }





    // This will be called by PlayerInput (Send Messages) when Move triggers
    public void OnMove(InputAction.CallbackContext context)
    {
        moveVector = context.ReadValue<Vector2>();
        if (inverseVerticleMovement) moveVector = new Vector2(moveVector.x, moveVector.y * -1);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
        
        if (!useKeyboardInput)
        {
            updateDel += MoveInput;

        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
        ApplyDrag();
    }

    private void HandleMovement()
    {
        // If joystick is being used
        if (moveVector.magnitude > 0.1f)
        {
            // 1️⃣ Find target angle based on joystick direction
            float targetAngle = Mathf.Atan2(moveVector.y, moveVector.x) * Mathf.Rad2Deg - 90f;

            // 2️⃣ Smoothly rotate the ship towards joystick direction
            float newAngle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, turnSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(newAngle);

            // 3️⃣ Add forward thrust in facing direction
            Vector2 force = transform.up * acceleration;
            rb.AddForce(force);

            // 4️⃣ Limit max speed
            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }
    }

    private void ApplyDrag()
    {
        rb.linearVelocity *= waterDrag;
    }

    
}
