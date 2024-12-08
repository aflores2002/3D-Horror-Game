using UnityEngine;

public class PlayerFlashlight : MonoBehaviour
{
    [SerializeField]
    private Light spotLight;

    [SerializeField]
    private KeyCode toggleKey = KeyCode.F;

    private bool isFlashlightOn = true;

    void Start()
    {
        if (spotLight == null)
        {
            Debug.LogError("Spotlight reference not set in PlayerFlashlight!");
            enabled = false;
            return;
        }

        spotLight.enabled = isFlashlightOn;
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
        }
    }
}