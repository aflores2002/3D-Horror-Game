using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    public string itemName = "Key"; // Need to change name for each object (fix)
    public void PickUp()
    {
        Debug.Log($"Picked up {itemName}!");
        
        Destroy(gameObject); // Remove the object from the scene when picked up
    }
}

