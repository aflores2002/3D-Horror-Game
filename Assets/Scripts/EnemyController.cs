using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    // Array of waypoints the enemy will patrol between
    public Transform[] waypoints;
    // Time in seconds the enemy will wait at each waypoint
    public float idleTime = 2f;
    // Movement speed while walking between waypoints
    public float walkSpeed = 2f;
    // Movement speed while chasing the player
    public float chaseSpeed = 4f;
    // How far away the enemy can detect the player
    public float sightDistance = 10f;

    // Heartbeat sound for player detection
    public AudioSource heartbeatAudioSource;

    // Reference to the NavMeshAgent component
    private NavMeshAgent agent;
    // Reference to the Animator component
    private Animator animator;
    // Reference to the player transform
    private Transform player;

    // Timer to track idle time
    private float idleTimer = 0f;
    // Current waypoint index
    private int currentWaypointIndex = 0;

    // Enemy states
    private enum EnemyState { Idle, Walk, Chase }
    private EnemyState currentState = EnemyState.Idle;

    // Animator parameter hash
    private readonly int animStateHash = Animator.StringToHash("AnimState");

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        SetDestinationToWaypoint();
    }

    private void Update()
    {
        // Handle player detection
        CheckForPlayerDetection();

        // Update behavior based on the current state
        switch (currentState)
        {
            case EnemyState.Idle:
                HandleIdleState();
                break;

            case EnemyState.Walk:
                HandleWalkState();
                break;

            case EnemyState.Chase:
                HandleChaseState();
                break;
        }
    }

    private void CheckForPlayerDetection()
    {
        RaycastHit hit;
        Vector3 playerDirection = player.position - transform.position;

        // Raycast to check if the player is visible within sight distance
        if (Physics.Raycast(transform.position, playerDirection.normalized, out hit, sightDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                if (currentState != EnemyState.Chase) // Only transition to Chase if not already chasing
                {
                    StartHeartbeat();
                    SwitchToState(EnemyState.Chase);
                    Debug.Log("Player detected!");
                }
            }
        }
        else
        {
            // Stop chasing if the player is no longer detected
            if (currentState == EnemyState.Chase)
            {
                StopHeartbeat();
                SwitchToState(EnemyState.Walk);
                Debug.Log("Player lost, returning to patrol.");
            }
        }
    }

    private void HandleIdleState()
    {
        animator.SetInteger(animStateHash, 0); // Set animation to idle
        idleTimer += Time.deltaTime;

        // Switch to walking after idle time expires
        if (idleTimer >= idleTime)
        {
            idleTimer = 0f;
            NextWaypoint();
        }
    }

    private void HandleWalkState()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            SwitchToState(EnemyState.Idle); // Transition to Idle when a waypoint is reached
        }

        // Ensure walking animation and speed are set
        if (agent.speed != walkSpeed)
        {
            agent.speed = walkSpeed;
            animator.SetInteger(animStateHash, 1);
        }
    }

    private void HandleChaseState()
    {
        // Set running animation and chase speed
        if (agent.speed != chaseSpeed)
        {
            agent.speed = chaseSpeed;
            animator.SetInteger(animStateHash, 2); // Run animation
        }

        // Continuously update the destination to the player's position
        agent.SetDestination(player.position);

        // Stop chasing if the player is out of sight
        if (Vector3.Distance(transform.position, player.position) > sightDistance)
        {
            StopHeartbeat();
            SwitchToState(EnemyState.Walk);
            SetDestinationToWaypoint();
        }
    }

    private void SwitchToState(EnemyState newState)
    {
        if (currentState != newState)
        {
            currentState = newState; // Update the current state
            Debug.Log($"Switched to state: {newState}");
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
        if (heartbeatAudioSource.isPlaying)
        {
            heartbeatAudioSource.Stop();
        }
    }

    private void NextWaypoint()
    {
        // Move to the next waypoint in the patrol route
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        SetDestinationToWaypoint();
    }

    private void SetDestinationToWaypoint()
    {
        agent.SetDestination(waypoints[currentWaypointIndex].position);
        SwitchToState(EnemyState.Walk); // Transition to walking state
    }

    private void OnDrawGizmos()
    {
        // Draw a line to the player, green if patrolling, red if chasing
        if (player != null)
        {
            Gizmos.color = currentState == EnemyState.Chase ? Color.red : Color.green;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}
