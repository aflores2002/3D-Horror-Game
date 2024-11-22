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

    void Start()
    {
        animator = GetComponent<Animator>();
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
            OpenDoor();
            if (showDebugMessages)
            {
                Debug.Log("Door opened successfully!");
            }
        }
        else if (showDebugMessages)
        {
            Debug.Log("Need all fragments to open door!");
        }
    }

    // Triggers door opening animation
    void OpenDoor()
    {
        isDoorOpen = true;
        animator.Play("Door_open");
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