using UnityEngine;
using UnityEngine.SceneManagement;

public class KillPlayer : MonoBehaviour
{
    public string nextSceneName; // Name of the next scene to load

    private bool playerInsideTrigger = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision detected with: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the trigger!");
            playerInsideTrigger = true;

            Invoke("LoadNextScene", 0);
        }
    }



    private void LoadNextScene()
    {
        if (playerInsideTrigger)
        {
            // Load the next scene by name
            SceneManager.LoadScene(nextSceneName);
        }
    }
}