using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform[] waypoints;
    public float idleTime = 2f;
    public float walkSpeed = 2f;
    public float chaseSpeed = 4f;
    public float sightDistance = 10f;

    [Header("Audio Settings")]
    public AudioSource heartbeatAudioSource;
    public float heartbeatFadeSpeed = 1f;
    public float maxHeartbeatVolume = 1f; // Maximum volume for the heartbeat

    private NavMeshAgent agent;
    private Animator animator;
    private Transform player;
    private float idleTimer = 0f;
    private int currentWaypointIndex = 0;
    private float currentHeartbeatVolume = 0f;
    private bool shouldFadeOut = false;

    private enum EnemyState { Idle, Walk, Chase }
    private EnemyState currentState = EnemyState.Idle;

    private readonly int animStateHash = Animator.StringToHash("AnimState");

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Initialize heartbeat audio
        if (heartbeatAudioSource != null)
        {
            heartbeatAudioSource.volume = 0f;
            heartbeatAudioSource.loop = true;
        }

        SetDestinationToWaypoint();
    }

    private void Update()
    {
        CheckForPlayerDetection();
        UpdateHeartbeatSound();

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

    private void HandleChaseState()
    {
        idleTimer = 0f;
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);

        if (Vector3.Distance(transform.position, player.position) > sightDistance)
        {
            shouldFadeOut = true;
            currentState = EnemyState.Walk;
            agent.speed = walkSpeed;
        }
    }

    private void UpdateHeartbeatSound()
    {
        if (heartbeatAudioSource == null) return;

        if (currentState == EnemyState.Chase && !shouldFadeOut)
        {
            // Fade in
            if (currentHeartbeatVolume < maxHeartbeatVolume)
            {
                currentHeartbeatVolume += heartbeatFadeSpeed * Time.deltaTime;
                currentHeartbeatVolume = Mathf.Min(currentHeartbeatVolume, maxHeartbeatVolume);
                heartbeatAudioSource.volume = currentHeartbeatVolume;
            }

            if (!heartbeatAudioSource.isPlaying)
            {
                heartbeatAudioSource.Play();
            }
        }
        else if (shouldFadeOut)
        {
            // Fade out
            if (currentHeartbeatVolume > 0)
            {
                currentHeartbeatVolume -= heartbeatFadeSpeed * Time.deltaTime;
                currentHeartbeatVolume = Mathf.Max(currentHeartbeatVolume, 0f);
                heartbeatAudioSource.volume = currentHeartbeatVolume;
            }
            else
            {
                heartbeatAudioSource.Stop();
                shouldFadeOut = false;
            }
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
                if (currentState != EnemyState.Chase)
                {
                    shouldFadeOut = false;
                }
                currentState = EnemyState.Chase;
            }
        }
    }

    private void HandleIdleState()
    {
        idleTimer += Time.deltaTime;
        animator.SetInteger(animStateHash, 0);

        if (idleTimer >= idleTime)
        {
            idleTimer = 0f;
            NextWaypoint();
        }
    }

    private void HandleWalkState()
    {
        animator.SetInteger(animStateHash, 1);

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            currentState = EnemyState.Idle;
        }
    }

    private void NextWaypoint()
    {
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        SetDestinationToWaypoint();
    }

    private void SetDestinationToWaypoint()
    {
        agent.SetDestination(waypoints[currentWaypointIndex].position);
        currentState = EnemyState.Walk;
        agent.speed = walkSpeed;
    }

    private void OnDrawGizmos()
    {
        if (player != null)
        {
            Gizmos.color = currentState == EnemyState.Chase ? Color.red : Color.green;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}