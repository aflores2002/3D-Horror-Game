using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EndGameTriggerWithFade : MonoBehaviour
{
    [SerializeField]
    private GameObject endingPanel;

    [SerializeField]
    private TextMeshProUGUI endingText;

    [Header("Panel Settings")]
    [SerializeField]
    private float fadeInSpeed = 1f;

    [Header("Text Settings")]
    [SerializeField]
    // Removed special formatting characters that might not be supported
    private string victoryMessage = "Congratulations!\nYou have survived!";

    [SerializeField]
    private float typingSpeed = 0.05f;

    [SerializeField]
    private AudioSource typingSoundEffect;

    private bool hasEnded = false;
    private CanvasGroup canvasGroup;

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

            // Make sure TextMeshPro starts with empty text, no formatting
            if (endingText != null)
            {
                endingText.text = "";
                // Disable rich text to prevent any special character formatting
                endingText.richText = false;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasEnded)
        {
            StartCoroutine(ShowVictorySequence());
        }
    }

    IEnumerator ShowVictorySequence()
    {
        hasEnded = true;

        if (endingPanel != null)
        {
            endingPanel.SetActive(true);
            StartCoroutine(FadeInPanel());
            StartCoroutine(TypeText());
        }

        yield break;
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