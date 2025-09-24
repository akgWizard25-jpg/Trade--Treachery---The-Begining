using UnityEngine;

public class PirateAI : MonoBehaviour
{
    public Transform[] patrolPoints;    // Waypoints to patrol
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float detectionRadius = 5f;
    public LayerMask playerLayer;       // Assign "Player" layer in Inspector

    private int currentPoint = 0;
    private Transform player;
    private bool chasing = false;

    private void Update()
    {
        if (chasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
            DetectPlayer();
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPoint];
        Vector3 direction = (targetPoint.position - transform.position).normalized;
        transform.position += direction * patrolSpeed * Time.deltaTime;

        // Face movement direction
        if (direction != Vector3.zero)
            transform.up = direction;

        // Switch to next point when close enough
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.2f)
        {
            currentPoint = (currentPoint + 1) % patrolPoints.Length;
        }
    }

    void DetectPlayer()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
        if (hit != null)
        {
            player = hit.transform;
            chasing = true;
        }
    }

    void ChasePlayer()
    {
        if (player == null) { chasing = false; return; }

        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * chaseSpeed * Time.deltaTime;

        if (direction != Vector3.zero)
            transform.up = direction;

        // If player escapes detection radius, return to patrol
        if (Vector3.Distance(transform.position, player.position) > detectionRadius * 1.5f)
        {
            chasing = false;
            player = null;
        }
    }

    // Draw detection radius in Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}

