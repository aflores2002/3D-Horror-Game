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
    //chase speed once seen the player 
    public float chaseSpeed = 4f;
    //how far away he will see the player 
    public float sightDistance = 10f;

    //heartbeat for when player is detected
    public AudioSource heartbeatAudioSource; // Reference to the heartbeat Audio Source

    public float heartbeatFadeSpeed = 1f; // Speed for fading in/out the heartbeat



    // Reference to the NavMeshAgent component for pathfinding
    private NavMeshAgent agent;
    // Reference to the Animator component for controlling animations
    private Animator animator;
    //reference player 
    private Transform player;

    // Timer to track how long the enemy has been idle at a waypoint
    private float idleTimer = 0f;
    // Index of the current waypoint in the waypoints array
    private int currentWaypointIndex = 0;

    // States for the enemy AI
    private enum EnemyState { Idle, Walk, Chase }
    // Current state of the enemy
    private EnemyState currentState = EnemyState.Idle;

    //private bool isChasingAnimation = false;


    // Cached hash ID for the animator parameter to improve performance
    private readonly int animStateHash = Animator.StringToHash("AnimState");

    // Initialize components and start patrolling
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        SetDestinationToWaypoint();
    }

    private void Update()
    {
        //check if player is within detection range 
        CheckForPlayerDetection();

        switch (currentState)
        {
            case EnemyState.Idle:
                HandleIdleState();
                break;

            case EnemyState.Walk:
                HandleWalkState();
                break;

            case EnemyState.Chase:
                idleTimer = 0f;
                agent.speed = chaseSpeed; // Set the chase speed.
                agent.SetDestination(player.position);
                //animator.SetBool("IsChasing", true); // Set IsChasing to true in chase state.

                

                // Check if the player is out of sight and go back to the walk state.
                if (Vector3.Distance(transform.position, player.position) > sightDistance)
                {
                    currentState = EnemyState.Walk;
                    agent.speed = walkSpeed; // Restore walking speed.
                }
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

    private void CheckForPlayerDetection()
    {
        RaycastHit hit;
        Vector3 playerDirection = player.position - transform.position;

        if (Physics.Raycast(transform.position, playerDirection.normalized, out hit, sightDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                if (currentState != EnemyState.Chase) // Start heartbeat if entering Chase state
                {
                    StartHeartbeat();
                }
                currentState = EnemyState.Chase;
                Debug.Log("Player detected!");
            }
        }
        else
        {
            if (currentState == EnemyState.Chase) // Stop heartbeat if leaving Chase state
            {
                StopHeartbeat();
            }
        }
    }

    private void StartHeartbeat()
    {
        if (!heartbeatAudioSource.isPlaying)
        {
            heartbeatAudioSource.Play();
        }
    }

    private void StopHeartbeat()
    {
        if (heartbeatAudioSource != null && heartbeatAudioSource.isPlaying)
        {
            heartbeatAudioSource.Stop();
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



    // Draw a green raycast line at all times and switch to red when the player is detected.
    private void OnDrawGizmos()
    {
        // Ensure the player reference is valid before drawing the line
        if (player != null)
        {
            Gizmos.color = currentState == EnemyState.Chase ? Color.red : Color.green;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}