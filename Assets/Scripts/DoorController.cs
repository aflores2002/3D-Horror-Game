using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Controls door behavior and interaction with player
public class DoorController : MonoBehaviour
{
    private Animator animator;
    private bool isInRange = false;  // Tracks if player is within interaction range
    private bool isDoorOpen = false;

    [Header("Debug")]
    [SerializeField]
    private bool showDebugMessages = true;  // Toggle debug logging

    [Header("Audio Settings")]
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip doorOpenSound;
    [SerializeField]
    private AudioClip doorUnlockSound;
    [SerializeField]
    private AudioClip doorLockedSound;  // New audio clip for locked door
    [SerializeField]
    [Range(0f, 1f)]
    private float doorOpenVolume = 1f;
    [SerializeField]
    [Range(0f, 1f)]
    private float doorUnlockVolume = 1f;
    [SerializeField]
    [Range(0f, 1f)]
    private float doorLockedVolume = 1f;  // Volume for locked door sound

    private bool hasUnlocked = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Auto-assign AudioSource if not set
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }

    // Handles player input when in range of door
    void Update()
    {
        if (isInRange && Input.GetKeyDown(KeyCode.E) && !isDoorOpen)
        {
            TryOpenDoor();
        }
    }

    // Checks if player has required fragments before opening door
    void TryOpenDoor()
    {
        if (KeyFragmentManager.Instance.HasAllFragments())
        {
            // Play unlock sound if this is the first time unlocking
            if (!hasUnlocked)
            {
                PlaySound(doorUnlockSound, doorUnlockVolume);
                hasUnlocked = true;
            }

            OpenDoor();
            if (showDebugMessages)
            {
                Debug.Log("Door opened successfully!");
            }
        }
        else
        {
            // Play locked door sound when attempting to open without enough fragments
            PlaySound(doorLockedSound, doorLockedVolume);

            if (showDebugMessages)
            {
                Debug.Log("Need all fragments to open door!");
            }
        }
    }

    // Plays the specified sound at the given volume
    private void PlaySound(AudioClip clip, float volume)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.Play();
        }
    }

    // Triggers door opening animation
    void OpenDoor()
    {
        isDoorOpen = true;
        animator.Play("Door_open");
        PlaySound(doorOpenSound, doorOpenVolume);
    }

    // Detects when player enters interaction range
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
            if (showDebugMessages)
            {
                Debug.Log("Player in range of door");
            }
        }
    }

    // Detects when player leaves interaction range
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
        }
    }
}