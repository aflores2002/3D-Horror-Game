using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyCollection : MonoBehaviour
{
    //reference to the manager script
    private KeyFragmentManager keyManager;
    

    void Start(){
        ///connect to teh KeyFragmentManagerScript
        keyManager = KeyFragmentManager.Instance;

        
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseDown(){
        //print interaction to console
        Debug.Log("Clicked key\n");
         //make use of key manager collection method
        keyManager.CollectFragment();
        //update UI using count manager method
        //countManager.UpdateKeyCountText();
        Destroy(gameObject);
        //make use of key manager collection method
        // keyManager.CollectFragment();
        //update UI using count manager method
        //countManager.UpdateKeyCountText();

    }

    //call key fragment manager

}
