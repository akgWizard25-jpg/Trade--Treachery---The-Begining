using UnityEngine;

public class PirateAI : MonoBehaviour
{
    public Transform[] patrolPoints;       // List of patrol points
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3f;
    public float detectionRange = 5f;
    public float fireRange = 3f;
    public float maxChaseDistance = 8f;    // Max distance from current patrol point

    private int currentPatrolIndex = 0;
    private Transform player;
    private Vector3 patrolOrigin;          // Current patrol point origin
    private bool chasingPlayer = false;
    private bool returningToPatrol = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        patrolOrigin = patrolPoints[currentPatrolIndex].position;
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float distanceFromOrigin = Vector2.Distance(transform.position, patrolOrigin);

        if (!returningToPatrol)
        {
            if (distanceToPlayer <= detectionRange && distanceToPlayer > fireRange && distanceFromOrigin <= maxChaseDistance)
            {
                // Chase player
                chasingPlayer = true;
                ChasePlayer();
            }
            else if (distanceToPlayer <= fireRange)
            {
                // Stop chasing, let Cannon.cs handle firing
                chasingPlayer = false;
            }
            else if (chasingPlayer && distanceFromOrigin > maxChaseDistance)
            {
                // Too far -> stop chasing and return
                chasingPlayer = false;
                returningToPatrol = true;
            }
            else
            {
                // Patrol normally
                chasingPlayer = false;
                Patrol();
            }
        }
        else
        {
            // Returning to patrol point
            ReturnToPatrol();
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPatrolIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, patrolSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.2f)
        {
            // Arrived -> update patrol origin to next point
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            patrolOrigin = patrolPoints[currentPatrolIndex].position;
        }
    }

    private void ChasePlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
    }

    private void ReturnToPatrol()
    {
        transform.position = Vector2.MoveTowards(transform.position, patrolOrigin, patrolSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, patrolOrigin) < 0.2f)
        {
            // Arrived -> resume normal patrol
            returningToPatrol = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw detection + fire ranges
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, fireRange);

        // Draw patrol points + chase limits
        if (patrolPoints != null)
        {
            Gizmos.color = Color.cyan;
            foreach (Transform point in patrolPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawSphere(point.position, 0.2f);
                    Gizmos.DrawWireSphere(point.position, maxChaseDistance);
                }
            }
        }
    }
}

