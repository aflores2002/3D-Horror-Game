using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 10f; // Max distance to interact with items
    public LayerMask interactableLayer; // Define which objects are interactable

    private Camera playerCamera;

    void Start()
    {
        playerCamera = Camera.main; // Reference the player's camera
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Press "E" to interact
        {
            TryInteract();
        }
    }

    void TryInteract()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactableLayer))
        {
            PickUpItem item = hit.collider.GetComponent<PickUpItem>();
            if (item != null)
            {
                item.PickUp(); // Call the PickUp function on the item
            }
        }
    }
}

