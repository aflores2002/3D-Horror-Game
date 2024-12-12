using UnityEngine;

// Controls the behavior of an interactive locker door, including rotation animations and player interaction
public class LockerController : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField]
    private Transform lockerDoor; // Reference to the door's Transform component for rotation
    [SerializeField]
    [Tooltip("Rotation angle when door is opened (in degrees)")]
    private float doorOpenAngle = -80f; // How many degrees the door rotates when opened
    [SerializeField]
    [Tooltip("How fast the door opens and closes")]
    private float doorSpeed = 5f; // Animation speed multiplier for door movement

    [Header("Audio Settings")]
    [SerializeField]
    private AudioSource audioSource; // Reference to the AudioSource component
    [SerializeField]
    private AudioClip doorSounds; // Combined audio clip for door sounds
    [SerializeField]
    [Tooltip("Adjusts the playback speed of door sounds")]
    private float audioPlaybackSpeed = 2f; // Controls how fast the audio plays
    [SerializeField]
    [Tooltip("Start time in seconds for the open sound")]
    private float openSoundStart = 0f;
    [SerializeField]
    [Tooltip("Length in seconds of the open sound")]
    private float openSoundLength = 1.5f;
    [SerializeField]
    [Tooltip("Start time in seconds for the close sound")]
    private float closeSoundStart = 0.5f;
    [SerializeField]
    [Tooltip("Length in seconds of the close sound")]
    private float closeSoundLength = 1.5f;
    [SerializeField]
    [Tooltip("How long the fade out should take")]
    private float fadeOutDuration = 0.2f; // Duration of the fade out in seconds

    private bool isInRange = false;  // Tracks if player is within interaction distance
    private bool isDoorOpen = false; // Current state of the door (open/closed)
    private Quaternion initialDoorRotation; // Starting rotation of the door
    private Quaternion openDoorRotation;    // Target rotation when door is fully open
    private float initialVolume; // Stores the original volume level

    void Start()
    {
        // Verify required components are assigned
        if (lockerDoor == null)
        {
            Debug.LogError("Locker Door transform not assigned to LockerController!");
            enabled = false; // Disable script if door reference is missing
            return;
        }

        // Auto-assign AudioSource if not set
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (audioSource != null)
        {
            initialVolume = audioSource.volume;
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
                lockerDoor.rotation,       // Current rotation
                targetRotation,            // Target rotation
                Time.deltaTime * doorSpeed // Smooth transition based on speed
            );
        }
    }

    // Switches door between open and closed states
    private void ToggleDoor()
    {
        isDoorOpen = !isDoorOpen;
        PlayDoorSound();
    }

    // Plays the appropriate door sound effect
    private void PlayDoorSound()
    {
        if (audioSource != null && doorSounds != null)
        {
            float startTime = isDoorOpen ? openSoundStart : closeSoundStart;
            float length = isDoorOpen ? openSoundLength : closeSoundLength;

            // Reset volume to initial value
            audioSource.volume = initialVolume;
            audioSource.pitch = audioPlaybackSpeed;
            audioSource.time = startTime;
            audioSource.clip = doorSounds;
            audioSource.Play();

            // Start the fade out process
            float actualPlayTime = length / audioPlaybackSpeed;
            float fadeOutStart = actualPlayTime - fadeOutDuration;
            StartCoroutine(FadeOutAndStop(fadeOutStart, fadeOutDuration));
        }
    }

    // Coroutine to handle the fade out and stopping of audio
    private System.Collections.IEnumerator FadeOutAndStop(float delay, float fadeDuration)
    {
        // Wait until it's time to start fading
        yield return new WaitForSeconds(delay);

        float startVolume = audioSource.volume;
        float timer = 0f;

        // Gradually reduce the volume
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / fadeDuration;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, normalizedTime);
            yield return null;
        }

        // Ensure volume is zero and stop playback
        audioSource.volume = 0f;
        audioSource.Stop();

        // Reset volume to initial value
        audioSource.volume = initialVolume;
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
        if (isDoorOpen != open)
        {
            isDoorOpen = open;
            PlayDoorSound();
        }
    }

    // Public method allowing external scripts to check door state
    public bool IsDoorOpen()
    {
        return isDoorOpen;
    }
}