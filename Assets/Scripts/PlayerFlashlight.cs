using UnityEngine;

public class PlayerFlashlight : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField]
    private Light spotLight;
    [SerializeField]
    private KeyCode toggleKey = KeyCode.F;

    [Header("Audio Settings")]
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip turnOnSound;
    [SerializeField]
    private AudioClip turnOffSound;

    private bool isFlashlightOn = false; // Changed to false so flashlight starts off

    void Start()
    {
        if (spotLight == null)
        {
            Debug.LogError("Spotlight reference not set in PlayerFlashlight!");
            enabled = false;
            return;
        }

        // Check if we have audio components
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        spotLight.enabled = isFlashlightOn; // Will start disabled
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleFlashlight();
        }
    }

    private void ToggleFlashlight()
    {
        if (spotLight != null)
        {
            isFlashlightOn = !isFlashlightOn;
            spotLight.enabled = isFlashlightOn;

            // Play appropriate sound effect
            if (audioSource != null)
            {
                AudioClip clipToPlay = isFlashlightOn ? turnOnSound : turnOffSound;
                if (clipToPlay != null)
                {
                    audioSource.clip = clipToPlay;
                    audioSource.Play();
                }
            }
        }
    }
}