// KeyFragmentManager.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Manages key fragment collection using singleton pattern (Will fully implement later)
public class KeyFragmentManager : MonoBehaviour
{
    public static KeyFragmentManager Instance { get; private set; }

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
}