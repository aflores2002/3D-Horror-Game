using UnityEngine;

// Controls flashlight behavior attached to the player camera, including toggle functionality and light settings
public class PlayerFlashlight : MonoBehaviour
{
    [Header("Light References")]
    [SerializeField]
    private Light spotLight; // Reference to the Unity Light component used for the flashlight
    [SerializeField]
    private Camera playerCamera; // Reference to the player's camera for positioning

    [Header("Light Settings")]
    [SerializeField]
    private float lightRange = 15f; // How far the flashlight beam reaches
    [SerializeField]
    private float spotAngle = 45f; // Width of the flashlight beam cone
    [SerializeField]
    private float lightIntensity = 1.5f; // Brightness of the flashlight

    [Header("Controls")]
    [SerializeField]
    private KeyCode toggleKey = KeyCode.F; // Key used to toggle flashlight on/off

    [Header("Optional Audio")]
    [SerializeField]
    private AudioClip toggleSound; // Sound played when toggling flashlight
    private AudioSource audioSource; // Component that plays the toggle sound

    private bool isFlashlightOn = true; // Tracks current state of flashlight

    private void Start()
    {
        // Attempt to find Light component if not manually assigned
        if (spotLight == null)
        {
            spotLight = GetComponentInChildren<Light>();
        }

        // Get main camera reference if not manually assigned
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        // Set up audio system for toggle sound effects
        if (toggleSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = toggleSound;
            audioSource.playOnAwake = false;
            audioSource.volume = 0.5f;
        }

        // Initialize flashlight parameters and verify component exists
        if (spotLight != null)
        {
            spotLight.type = LightType.Spot;      // Ensure light is spot type for flashlight effect
            spotLight.range = lightRange;         // Set how far light reaches
            spotLight.spotAngle = spotAngle;      // Set beam width
            spotLight.intensity = lightIntensity; // Set brightness
            spotLight.enabled = isFlashlightOn;   // Set initial state
        }
        else
        {
            Debug.LogError("No Spot Light assigned to PlayerFlashlight!");
            enabled = false;  // Disable script if no light component found
        }
    }

    private void Update()
    {
        // Check for player input to toggle flashlight
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleFlashlight();
        }
    }

    // Switches flashlight between on and off states
    private void ToggleFlashlight()
    {
        if (spotLight != null)
        {
            isFlashlightOn = !isFlashlightOn; // Flip current state
            spotLight.enabled = isFlashlightOn; // Update light component

            // Play toggle sound if audio is configured
            if (audioSource != null && toggleSound != null)
            {
                audioSource.Play();
            }
        }
    }

    // Public accessor for flashlight state
    public bool IsFlashlightOn() => isFlashlightOn;

    // Allows external scripts to modify flashlight brightness
    public void SetFlashlightIntensity(float intensity)
    {
        if (spotLight != null)
        {
            lightIntensity = intensity;
            spotLight.intensity = intensity;
        }
    }

    // Allows external scripts to modify flashlight range
    public void SetFlashlightRange(float range)
    {
        if (spotLight != null)
        {
            lightRange = range;
            spotLight.range = range;
        }
    }
}