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
    [Tooltip("Time between each character (in seconds)")]
    private float typingSpeed = 0.05f; // Delay between each character appearing

    [Header("Audio Settings")]
    [SerializeField]
    private AudioSource audioSource; // Main audio source for sounds
    [SerializeField]
    private AudioClip typingSound; // Sound for typing effect
    [SerializeField]
    private AudioClip victorySound; // Sound played when victory sequence starts
    [SerializeField]
    [Range(0f, 1f)]
    private float typingSoundVolume = 0.5f; // Volume for typing sounds
    [SerializeField]
    [Range(0f, 1f)]
    private float victorySoundVolume = 1f; // Volume for victory sound
    [SerializeField]
    [Tooltip("Delay before starting victory sequence")]
    private float startDelay = 0.5f; // Delay before sequence starts

    [Header("Player Settings")]
    [SerializeField]
    private PlayerController playerController; // Reference to the player controller

    private bool hasEnded = false;   // Prevents trigger from firing multiple times
    private CanvasGroup canvasGroup; // Controls panel transparency

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

        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }

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

    private void PlaySound(AudioClip clip, float volume)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip, volume); // Using PlayOneShot instead of Play
        }
    }

    IEnumerator ShowVictorySequence()
    {
        hasEnded = true;

        // Disable player movement immediately
        if (playerController != null)
        {
            playerController.SetControlsEnabled(false);
            playerController.StoreCurrentRotation();
        }

        // Wait before starting sequence
        yield return new WaitForSeconds(startDelay);

        // Play victory sound
        PlaySound(victorySound, victorySoundVolume);

        if (endingPanel != null)
        {
            endingPanel.SetActive(true);
            StartCoroutine(FadeInPanel());
            StartCoroutine(TypeText());
        }
    }

    IEnumerator FadeInPanel()
    {
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime * fadeInSpeed;
            yield return null;
        }
    }

    IEnumerator TypeText()
    {
        if (endingText != null)
        {
            // Short delay before starting typing
            yield return new WaitForSeconds(0.2f);

            foreach (char c in victoryMessage)
            {
                endingText.text += c;

                if (c != ' ' && c != '\n') // Don't play sound for spaces or newlines
                {
                    PlaySound(typingSound, typingSoundVolume);
                }

                // Use unscaled time to ensure consistent typing speed
                yield return new WaitForSecondsRealtime(typingSpeed);
            }
        }
    }
}