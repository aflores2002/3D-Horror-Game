using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [Header("Flicker Settings")]
    [SerializeField]
    private float minIntensity = 0.5f;
    [SerializeField]
    private float maxIntensity = 1.5f;
    [SerializeField]
    [Tooltip("How quickly the light intensity changes")]
    private float flickerSpeed = 0.1f;
    [SerializeField]
    [Tooltip("Smooths out the flickering effect")]
    private float smoothing = 0.1f;

    // Optional settings for more varied effects
    [Header("Advanced Settings")]
    [SerializeField]
    private bool useRandomSeeds = true;
    [SerializeField]
    [Tooltip("Chance per frame that the light will start flickering")]
    [Range(0f, 1f)]
    private float flickerProbability = 0.1f;
    [SerializeField]
    private float flickerDuration = 0.2f;

    private Light lightComponent;
    private float baseIntensity;
    private float randomOffset;
    private float timeUntilNextFlicker;
    private float currentIntensity;
    private bool isFlickering;

    private void Start()
    {
        lightComponent = GetComponent<Light>();
        if (lightComponent == null)
        {
            Debug.LogError("No Light component found on " + gameObject.name);
            enabled = false;
            return;
        }

        baseIntensity = lightComponent.intensity;
        currentIntensity = baseIntensity;
        randomOffset = useRandomSeeds ? Random.Range(0f, 10000f) : 0f;
    }

    private void Update()
    {
        if (!isFlickering)
        {
            // Check if we should start a new flicker
            if (Random.value < flickerProbability * Time.deltaTime)
            {
                StartFlicker();
            }
        }
        else
        {
            timeUntilNextFlicker -= Time.deltaTime;
            if (timeUntilNextFlicker <= 0)
            {
                isFlickering = false;
            }
        }

        UpdateLightIntensity();
    }

    private void StartFlicker()
    {
        isFlickering = true;
        timeUntilNextFlicker = flickerDuration;
    }

    private void UpdateLightIntensity()
    {
        if (isFlickering)
        {
            // Generate noise based on time
            float noise = Mathf.PerlinNoise(Time.time * flickerSpeed + randomOffset, 0f);

            // Map noise to our desired intensity range
            float targetIntensity = Mathf.Lerp(minIntensity, maxIntensity, noise) * baseIntensity;

            // Smoothly transition to the target intensity
            currentIntensity = Mathf.Lerp(currentIntensity, targetIntensity, smoothing);
        }
        else
        {
            // Smoothly return to base intensity when not flickering
            currentIntensity = Mathf.Lerp(currentIntensity, baseIntensity, smoothing);
        }

        lightComponent.intensity = currentIntensity;
    }

    // Public method to manually trigger a flicker
    public void TriggerFlicker()
    {
        StartFlicker();
    }

    // Public method to set the base intensity
    public void SetBaseIntensity(float intensity)
    {
        baseIntensity = intensity;
    }
}