using UnityEngine;

public class ShowDescription : MonoBehaviour
{
    public GameObject descriptionPanel; // Reference to the panel

    // This method is called when the button is clicked
    public void OnButtonClick()
    {
        // Toggle the panel's visibility
        descriptionPanel.SetActive(!descriptionPanel.activeSelf);
    }
}