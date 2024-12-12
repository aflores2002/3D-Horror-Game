using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

// Handles end game sequence with fade effect and typewriter text animation
public class EndGameTriggerWithFade : MonoBehaviour
{
    [SerializeField]
    private GameObject endingPanel; // UI panel that appears at game ending

    [SerializeField]
    private TextMeshProUGUI endingText; // Text component for ending message

    [Header("Panel Settings")]
    [SerializeField]
    private float fadeInSpeed = 1f; // Speed of panel fade in effect

    [Header("Text Settings")]
    [SerializeField]
    private string victoryMessage = "Congratulations!\nYou have survived!";

    [SerializeField]
    private float typingSpeed = 0.05f; // Delay between each character appearing

    [SerializeField]
    private AudioSource typingSoundEffect; // Sound played for each character typed

    [Header("Player Settings")]
    [SerializeField]
    private PlayerController playerController; // Reference to the player controller

    private bool hasEnded = false;   // Prevents trigger from firing multiple times
    private CanvasGroup canvasGroup; // Controls panel transparency

    // Initialize UI elements and ensure proper setup
    void Start()
    {
        if (endingPanel != null)
        {
            endingPanel.SetActive(false);
            canvasGroup = endingPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = endingPanel.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 0f;

            if (endingText != null)
            {
                endingText.text = "";
                endingText.richText = false; // Prevents formatting issues
            }
        }

        // Try to find PlayerController if not assigned
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }
    }

    // Triggers victory sequence when player enters trigger zone
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasEnded)
        {
            if (playerController == null)
            {
                playerController = other.GetComponent<PlayerController>();
            }
            StartCoroutine(ShowVictorySequence());
        }
    }

    // Coordinates the victory animations
    IEnumerator ShowVictorySequence()
    {
        hasEnded = true;

        // Disable player movement
        if (playerController != null)
        {
            playerController.SetControlsEnabled(false);

            // Store final camera rotation to prevent snapping
            playerController.StoreCurrentRotation();
        }

        if (endingPanel != null)
        {
            endingPanel.SetActive(true);
            StartCoroutine(FadeInPanel());
            StartCoroutine(TypeText());
        }

        yield break;
    }

    // Gradually fades in the ending panel
    IEnumerator FadeInPanel()
    {
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime * fadeInSpeed;
            yield return null;
        }
    }

    // Creates typewriter effect by showing text character by character
    IEnumerator TypeText()
    {
        if (endingText != null)
        {
            foreach (char c in victoryMessage)
            {
                endingText.text += c;

                if (typingSoundEffect != null)
                {
                    typingSoundEffect.Play();
                }

                yield return new WaitForSeconds(typingSpeed);
            }
        }
    }
}