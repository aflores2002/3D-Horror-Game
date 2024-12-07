using UnityEngine;

public class LockerController : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField]
    private Transform lockerDoor;
    [SerializeField]
    [Tooltip("Rotation angle when door is opened (in degrees)")]
    private float doorOpenAngle = -80f;
    [SerializeField]
    [Tooltip("How fast the door opens and closes")]
    private float doorSpeed = 5f;

    private bool isInRange = false;
    private bool isDoorOpen = false;
    private Quaternion initialDoorRotation;
    private Quaternion openDoorRotation;

    void Start()
    {
        if (lockerDoor == null)
        {
            Debug.LogError("Locker Door transform not assigned to LockerController!");
            enabled = false;
            return;
        }

        // Store the initial rotation and calculate the open rotation
        initialDoorRotation = lockerDoor.rotation;
        openDoorRotation = initialDoorRotation * Quaternion.Euler(0f, doorOpenAngle, 0f);
    }

    void Update()
    {
        if (isInRange && Input.GetKeyDown(KeyCode.E))
        {
            ToggleDoor();
        }

        // Smoothly animate the door
        if (lockerDoor != null)
        {
            Quaternion targetRotation = isDoorOpen ? openDoorRotation : initialDoorRotation;
            lockerDoor.rotation = Quaternion.Lerp(lockerDoor.rotation, targetRotation, Time.deltaTime * doorSpeed);
        }
    }

    private void ToggleDoor()
    {
        isDoorOpen = !isDoorOpen;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
        }
    }

    // Optional: Add method to force door state
    public void SetDoorOpen(bool open)
    {
        isDoorOpen = open;
    }

    // Optional: Add method to check if door is currently open
    public bool IsDoorOpen()
    {
        return isDoorOpen;
    }
}