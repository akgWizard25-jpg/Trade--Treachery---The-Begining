using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PirateAIPath : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 3f;
    public float pathUpdateInterval = 1f;
    public float avoidRadius = 3f;          // Distance to detect other ships
    public float avoidForce = 5f;           // Strength of avoidance
    public LayerMask avoidanceMask;         // Layer for other ships

    private Rigidbody2D rb;
    private List<PathNode> currentPath;
    private int currentPathIndex = 0;
    private float pathUpdateTimer = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        pathUpdateTimer -= Time.deltaTime;

        // Update A* path every few seconds
        if (pathUpdateTimer <= 0f)
        {
            pathUpdateTimer = pathUpdateInterval;
            currentPath = AStarPathfinder.FindPath(transform.position, player.position);
            currentPathIndex = 0;
        }

        MoveAlongPath();
    }

    private void MoveAlongPath()
    {
        if (currentPath == null || currentPathIndex >= currentPath.Count)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 targetPos = GridManager.Instance.GetWorldPosition(currentPath[currentPathIndex].gridPosition);
        Vector2 desiredDirection = (targetPos - (Vector2)transform.position).normalized;

        // ✅ Apply local avoidance
        Vector2 avoidanceDir = GetAvoidanceDirection();
        Vector2 finalDir = (desiredDirection + avoidanceDir).normalized;

        rb.linearVelocity = finalDir * moveSpeed;

        // Move to next node when close enough
        if (Vector2.Distance(transform.position, targetPos) < 0.3f)
        {
            currentPathIndex++;
        }
    }

    private Vector2 GetAvoidanceDirection()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, avoidRadius, avoidanceMask);
        Vector2 avoidance = Vector2.zero;

        foreach (var hit in hits)
        {
            if (hit.attachedRigidbody == rb) continue; // Skip self

            Vector2 away = (Vector2)transform.position - (Vector2)hit.transform.position;
            float distance = Mathf.Max(away.magnitude, 0.1f);
            avoidance += away.normalized / distance; // Closer objects push more
        }

        return avoidance.normalized * avoidForce;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, avoidRadius);
    }
}

