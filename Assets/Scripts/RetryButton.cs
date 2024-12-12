using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RetryButton : MonoBehaviour

{

    // This method is called when the button is clicked
    public void OnButtonClick()
    {
        // Load the scene named "StartScene"
        SceneManager.LoadScene("StartScene");
    }
}

