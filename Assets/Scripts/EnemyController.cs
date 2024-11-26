using UnityEngine;
using UnityEngine.AI;

// Controls enemy AI patrol behavior using waypoints and NavMesh for navigation
public class EnemyController : MonoBehaviour
{
    // Array of waypoints the enemy will patrol between
    public Transform[] waypoints;
    // Time in seconds the enemy will wait at each waypoint
    public float idleTime = 2f;
    // Movement speed while walking between waypoints
    public float walkSpeed = 2f;

    // Reference to the NavMeshAgent component for pathfinding
    private NavMeshAgent agent;
    // Reference to the Animator component for controlling animations
    private Animator animator;
    // Timer to track how long the enemy has been idle at a waypoint
    private float idleTimer = 0f;
    // Index of the current waypoint in the waypoints array
    private int currentWaypointIndex = 0;

    // States for the enemy AI
    private enum EnemyState { Idle, Walk }
    // Current state of the enemy
    private EnemyState currentState = EnemyState.Idle;

    // Cached hash ID for the animator parameter to improve performance
    private readonly int animStateHash = Animator.StringToHash("AnimState");

    // Initialize components and start patrolling
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        SetDestinationToWaypoint();
    }

    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                HandleIdleState();
                break;

            case EnemyState.Walk:
                HandleWalkState();
                break;
        }
    }

    // Handles behavior while in idle state at a waypoint
    private void HandleIdleState()
    {
        // Increment the idle timer
        idleTimer += Time.deltaTime;
        // Set animation to idle state (0)
        animator.SetInteger(animStateHash, 0);

        // If we've waited long enough, move to next waypoint
        if (idleTimer >= idleTime)
        {
            idleTimer = 0f;
            NextWaypoint();
        }
    }

    // Handles behavior while walking between waypoints
    private void HandleWalkState()
    {
        // Set animation to walking state (1)
        animator.SetInteger(animStateHash, 1);

        // If we've reached the current waypoint, switch to idle
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            currentState = EnemyState.Idle;
        }
    }

    // Advances to the next waypoint in the patrol sequence
    private void NextWaypoint()
    {
        // Increment waypoint index, wrapping around to 0 if we exceed array bounds
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        SetDestinationToWaypoint();
    }

    // Sets the NavMeshAgent's destination to the current waypoint
    private void SetDestinationToWaypoint()
    {
        // Tell the NavMeshAgent to move to the current waypoint
        agent.SetDestination(waypoints[currentWaypointIndex].position);
        // Change state to walking
        currentState = EnemyState.Walk;
        // Set the movement speed
        agent.speed = walkSpeed;
    }

    // Draws debug visualization of the patrol path in the Unity editor
    private void OnDrawGizmos()
    {
        // Only draw if we have waypoints set up
        if (waypoints != null && waypoints.Length > 0)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] != null)
                {
                    // Draw a sphere at each waypoint
                    Gizmos.DrawWireSphere(waypoints[i].position, 0.5f);

                    // Draw lines between consecutive waypoints
                    if (i + 1 < waypoints.Length && waypoints[i + 1] != null)
                    {
                        Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                    }
                    // Draw line from last waypoint back to first to complete the patrol circuit
                    else if (i == waypoints.Length - 1 && waypoints[0] != null)
                    {
                        Gizmos.DrawLine(waypoints[i].position, waypoints[0].position);
                    }
                }
            }
        }
    }
}