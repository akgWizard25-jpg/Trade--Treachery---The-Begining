using UnityEngine;

public class Cannon : MonoBehaviour
{
    [Header("Setup")]
    public Transform[] leftFirePoints;       // Firepoints on left side
    public Transform[] rightFirePoints;      // Firepoints on right side
    public GameObject projectilePrefab;      // Cannonball prefab
    public LayerMask targetLayer;            // What can be shot (e.g., Player)

    [Header("Stats")]
    public float fireRange = 6f;             // Max shooting distance
    public float fireRate = 1f;              // Shots per second

    private float fireCooldown = 0f;
    private Transform target;

    private void Update()
    {
        fireCooldown -= Time.deltaTime;

        FindTarget();

        if (target != null && fireCooldown <= 0f)
        {
            FireFromSide();
            fireCooldown = 1f / fireRate;
        }
    }

    void FindTarget()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, fireRange, targetLayer);
        if (hit != null)
        {
            target = hit.transform;
        }
        else
        {
            target = null;
        }
    }

    void FireFromSide()
    {
        if (projectilePrefab == null || target == null) return;

        // Vector from ship to target
        Vector2 toTarget = (target.position - transform.position).normalized;

        // Check which side target is on (left or right relative to ship forward)
        float side = Vector3.Dot(toTarget, -transform.right);

        // LEFT side (target is on left of ship)
        if (side > 0 && leftFirePoints.Length > 0)
        {
            foreach (Transform firePoint in leftFirePoints)
            {
                FireSingle(firePoint);
            }
        }
        // RIGHT side (target is on right of ship)
        else if (side < 0 && rightFirePoints.Length > 0)
        {
            foreach (Transform firePoint in rightFirePoints)
            {
                FireSingle(firePoint);
            }
        }
    }

    void FireSingle(Transform firePoint)
    {
        if (firePoint == null) return;

        Vector2 direction = (target.position - firePoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        firePoint.rotation = Quaternion.Euler(0, 0, angle);

        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, fireRange);

        Gizmos.color = Color.green;
        foreach (Transform fp in leftFirePoints)
            if (fp != null) Gizmos.DrawSphere(fp.position, 0.1f);

        Gizmos.color = Color.blue;
        foreach (Transform fp in rightFirePoints)
            if (fp != null) Gizmos.DrawSphere(fp.position, 0.1f);
    }
}

