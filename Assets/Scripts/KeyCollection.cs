using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyCollection : MonoBehaviour
{
    //reference to the key object
    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseDown(){
        //print interaction to console
        Debug.Log("Clicked key/n");
        Destroy(gameObject);
    }
}
