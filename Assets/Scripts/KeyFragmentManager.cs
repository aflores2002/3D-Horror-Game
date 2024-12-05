// KeyFragmentManager.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

// Manages key fragment collection using singleton pattern (Will fully implement later)
public class KeyFragmentManager : MonoBehaviour
{
    
    public static KeyFragmentManager Instance { get; private set; }

    public UnityEvent<KeyFragmentManager> OnKeyCollection;

    [SerializeField]
    private bool testMode = true;  // Bypasses fragment requirement when enabled

    private int collectedFragments = 0;
    public int requiredFragments = 4;

    void Awake()
    {
        // Singleton setup - maintains single instance across scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // Increments fragment count and logs progress
    public void CollectFragment()
    {
        collectedFragments++;
        Debug.Log($"Fragments collected: {collectedFragments}/{requiredFragments}");
    }

    // Returns true if all fragments collected or in test mode
    public bool HasAllFragments()
    {
        return testMode || collectedFragments >= requiredFragments;
    }

    //player action triggers the key collection
    public void OnMouseDown(){
        //print interaction to console
        Debug.Log("Clicked key/n");
        //destroy this key
        Destroy(gameObject);
        CollectFragment();
        OnKeyCollection.Invoke(this);
    }

    //returns keycount for the UI display
    public int keyCount(){
        return collectedFragments;
    }

}