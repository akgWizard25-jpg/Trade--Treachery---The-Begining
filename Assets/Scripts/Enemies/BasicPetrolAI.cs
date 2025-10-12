using UnityEngine;
using System.Collections.Generic;

namespace JY
{
    // ----------------- STATE BASE -----------------
    public abstract class PetrolAIBase
    {
        public abstract void EnterState(BasicPetrolAI BAI);
        public abstract void UpdateState(BasicPetrolAI BAI);
        public abstract void ExitState(BasicPetrolAI BAI);
    }

    // ----------------- PATROL STATE -----------------
    public class PetrolState : PetrolAIBase
    {
        private int currentWaypointIndex = 0;
        private float waitTimer = 0f;
        private float currentWaitDuration = 0f;

        public override void EnterState(BasicPetrolAI BAI)
        {
            Debug.Log("Entering Patrol State");
            waitTimer = 0f;
            currentWaitDuration = 0f;
        }

        public override void UpdateState(BasicPetrolAI BAI)
        {
            // Detect player
            if (DetectPlayer(BAI))
            {
                BAI.SwitchState(BAI.chasePlayerState);
                return;
            }

            if (BAI.waypoints.Length == 0) return;

            Transform target = BAI.waypoints[currentWaypointIndex];
            float distance = Vector2.Distance(BAI.transform.position, target.position);

            // If reached waypoint
            if (distance < 0.5f)
            {
                if (currentWaitDuration <= 0f) 
                {
                    currentWaitDuration = BAI.useRandomWaitTime 
                        ? Random.Range(BAI.minWaitTime, BAI.maxWaitTime) 
                        : BAI.waitTime;
                    waitTimer = 0f;
                }

                waitTimer += Time.deltaTime;

                if (waitTimer >= currentWaitDuration)
                {
                    currentWaypointIndex = (currentWaypointIndex + 1) % BAI.waypoints.Length;
                    currentWaitDuration = 0f;
                    waitTimer = 0f;
                }
            }
            else
            {
                BAI.MoveTowards(target.position);
            }
        }

        public override void ExitState(BasicPetrolAI BAI)
        {
            Debug.Log("Exiting Patrol State");
        }

        private bool DetectPlayer(BasicPetrolAI BAI)
        {
            if (BAI.player == null) return false;

            Vector2 dirToPlayer = (BAI.player.position - BAI.transform.position).normalized;
            float distanceToPlayer = Vector2.Distance(BAI.transform.position, BAI.player.position);

            // Auto detect if close
            if (distanceToPlayer < BAI.miniDistanceToAlert)
                return true;

            // Use visual right direction as "forward" in 2D
            if (distanceToPlayer < BAI.detectionRange && BAI.visual != null)
            {
                float angle = Vector2.Angle(BAI.visual.right, dirToPlayer);
                if (angle < BAI.fovAngle * 0.5f)
                {
                    // Line of sight check in 2D
                    RaycastHit2D hit = Physics2D.Raycast(BAI.visual.position, dirToPlayer, distanceToPlayer, BAI.obstacleMask);
                    if (!hit) return true;
                }
            }

            return false;
        }
    }




    // ----------------- CHASE STATE -----------------
    public class ChasePlayerState : PetrolAIBase
    {
        private bool isFireing = false;




        public override void EnterState(BasicPetrolAI BAI)
        {
            Debug.Log("Entering Chase State");
        }

        public override void UpdateState(BasicPetrolAI BAI)
        {
            if (BAI.player == null)
            {
                BAI.SwitchState(BAI.returnToPetrolState);
                return;
            }

            float distanceToPlayer = Vector2.Distance(BAI.transform.position, BAI.player.position);

            // 🔴 Fire if in range
            if (distanceToPlayer <= BAI.fireRadius)
            {
                // Maintain safe distance (don’t get too close)
                if (distanceToPlayer > BAI.safeDistanceFromExplosion && !isFireing)
                {
                    Debug.Log("💥 Firing cannon at player!");
                    BAI.shipCannon.SwtichFireMode(true);
                    isFireing = true;

                }
                else
                {
                    // Back up a little if too close
                    BAI.MoveAwayFrom(BAI.player.position);
                }
            }
            else
            {
                // Keep chasing if outside fire radius
                BAI.MoveTowards(BAI.player.position);
            }

            // If lost player, return to patrol
            if (distanceToPlayer > BAI.lostRange)
            {
                BAI.SwitchState(BAI.returnToPetrolState);
                if (isFireing)
                {
                    BAI.shipCannon.SwtichFireMode(false);
                    isFireing = false;
                }
            }
        }

        public override void ExitState(BasicPetrolAI BAI)
        {
            Debug.Log("Exiting Chase State");
        }
    }


    // ----------------- RETURN TO PATROL STATE -----------------
    public class ReturnToPetrolState : PetrolAIBase
    {
        public override void EnterState(BasicPetrolAI BAI)
        {
            Debug.Log("Returning to Patrol Route");
        }

        public override void UpdateState(BasicPetrolAI BAI)
        {
            if (BAI.waypoints.Length == 0) return;

            Transform nearest = BAI.GetClosestWaypoint();
            BAI.MoveTowards(nearest.position);

            if (Vector3.Distance(BAI.transform.position, nearest.position) < 0.5f)
            {
                BAI.SwitchState(BAI.patrolState);
            }
        }

        public override void ExitState(BasicPetrolAI BAI)
        {
            Debug.Log("Exiting Return State");
        }
    }

    // ----------------- DEAD STATE -----------------
    public class DeadState : PetrolAIBase
    {
        public override void EnterState(BasicPetrolAI BAI)
        {
            Debug.Log("Enemy is Dead!");
            BAI.enabled = false;
            // Play death animation, disable collider etc.
        }

        public override void UpdateState(BasicPetrolAI BAI)
        {
            // Do nothing, dead
        }

        public override void ExitState(BasicPetrolAI BAI)
        {
            // Not needed
        }
    }

    // ----------------- MAIN CONTROLLER -----------------
    public class BasicPetrolAI : MonoBehaviour
    {
        [Header("References")]
        public Transform player;
        public Transform[] waypoints;

        [Header("Settings")]
        public float speed = 3f;
        public float detectionRange = 5f;
        public float lostRange = 7f;
        public float rotationSpeed = 5f;  // smooth turning speed

        [Header("Detection Settings")]
        public float fovAngle = 90f;          // Field of view angle
        public float miniDistanceToAlert = 2f; // Auto detect if closer than this
        public LayerMask obstacleMask;

        [Header("Visual")]
        public Transform visual; // Reference to enemy’s head/eyes

        [Header("Patrol Wait Settings")]
        public bool useRandomWaitTime = false;
        public float waitTime = 2f;
        public float minWaitTime = 1f;
        public float maxWaitTime = 4f;
        [Header("Combat Settings")]
        public float fireRadius = 4f;                // how close to fire
        public float safeDistanceFromExplosion = 2f; // minimum safe distance before firing
        public Cannon shipCannon;


        // State instances
        [HideInInspector] public PetrolState patrolState = new PetrolState();
        [HideInInspector] public ChasePlayerState chasePlayerState = new ChasePlayerState();
        [HideInInspector] public ReturnToPetrolState returnToPetrolState = new ReturnToPetrolState();
        [HideInInspector] public DeadState deadState = new DeadState();

        private PetrolAIBase currentState;







        // Start is called before the first frame update
        void Start()
        {
            currentState = patrolState;
            currentState.EnterState(this);
        }

        // Update is called once per frame
        void Update()
        {
            currentState.UpdateState(this);
        }

        public void SwitchState(PetrolAIBase newState)
        {
            currentState.ExitState(this);
            currentState = newState;
            currentState.EnterState(this);
        }

        #region UTILITY
        public void MoveTowards(Vector3 target)
        {
            Vector2 direction = (target - transform.position).normalized;

            // Smooth rotation towards direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Move forward
            transform.position += transform.right * speed * Time.deltaTime;
        }

        public void MoveAwayFrom(Vector3 target)
        {
            Vector2 direction = (transform.position - target).normalized;

            // Smooth rotate away from target
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Move away
            transform.position += transform.right * speed * Time.deltaTime;
        }



        public Transform GetClosestWaypoint()
        {
            Transform closest = waypoints[0];
            float minDist = Vector3.Distance(transform.position, closest.position);

            foreach (Transform wp in waypoints)
            {
                float dist = Vector3.Distance(transform.position, wp.position);
                if (dist < minDist)
                {
                    closest = wp;
                    minDist = dist;
                }
            }
            return closest;
        }
        #endregion

        // Call this to kill enemy
        public void Die()
        {
            SwitchState(deadState);
        }
        








        private void OnDrawGizmosSelected()
        {
            if (visual == null) return;

            // Detection range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            // Mini alert range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, miniDistanceToAlert);

            // FOV lines (2D)
            Vector3 leftBoundary = Quaternion.Euler(0, 0, -fovAngle * 0.5f) * visual.right;
            Vector3 rightBoundary = Quaternion.Euler(0, 0, fovAngle * 0.5f) * visual.right;

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(visual.position, visual.position + leftBoundary * detectionRange);
            Gizmos.DrawLine(visual.position, visual.position + rightBoundary * detectionRange);
        }


    }
}
