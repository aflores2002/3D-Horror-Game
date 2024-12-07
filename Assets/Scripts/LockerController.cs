using UnityEngine;

// Controls the behavior of an interactive locker door, including rotation animations and player interaction
public class LockerController : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField]
    private Transform lockerDoor;     // Reference to the door's Transform component for rotation
    [SerializeField]
    [Tooltip("Rotation angle when door is opened (in degrees)")]
    private float doorOpenAngle = -80f;    // How many degrees the door rotates when opened
    [SerializeField]
    [Tooltip("How fast the door opens and closes")]
    private float doorSpeed = 5f;     // Animation speed multiplier for door movement

    private bool isInRange = false;   // Tracks if player is within interaction distance
    private bool isDoorOpen = false;  // Current state of the door (open/closed)
    private Quaternion initialDoorRotation;    // Starting rotation of the door
    private Quaternion openDoorRotation;       // Target rotation when door is fully open

    void Start()
    {
        // Verify required components are assigned
        if (lockerDoor == null)
        {
            Debug.LogError("Locker Door transform not assigned to LockerController!");
            enabled = false;  // Disable script if door reference is missing
            return;
        }

        // Cache initial door rotation and calculate target open position
        initialDoorRotation = lockerDoor.rotation;
        openDoorRotation = initialDoorRotation * Quaternion.Euler(0f, doorOpenAngle, 0f);
    }

    void Update()
    {
        // Check for player interaction input when in range
        if (isInRange && Input.GetKeyDown(KeyCode.E))
        {
            ToggleDoor();
        }

        // Handle door animation if door reference exists
        if (lockerDoor != null)
        {
            // Determine target rotation based on door state
            Quaternion targetRotation = isDoorOpen ? openDoorRotation : initialDoorRotation;

            // Smoothly interpolate door rotation using Lerp
            lockerDoor.rotation = Quaternion.Lerp(
                lockerDoor.rotation,      // Current rotation
                targetRotation,           // Target rotation
                Time.deltaTime * doorSpeed // Smooth transition based on speed
            );
        }
    }

    // Switches door between open and closed states
    private void ToggleDoor()
    {
        isDoorOpen = !isDoorOpen;
    }

    // Triggered when player enters interaction zone
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
        }
    }

    // Triggered when player leaves interaction zone
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
        }
    }

    // Public method allowing external scripts to control door state
    public void SetDoorOpen(bool open)
    {
        isDoorOpen = open;
    }

    // Public method allowing external scripts to check door state
    public bool IsDoorOpen()
    {
        return isDoorOpen;
    }
}