// KeyFragmentManager.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;



// Manages key fragment collection using singleton pattern (Will fully implement later)
public class KeyFragmentManager : MonoBehaviour
{
    //for spawning the keys
    public GameObject keyPrefab;

    public static KeyFragmentManager Instance { get; private set; }

    public UnityEvent OnKeyCollection;

    [SerializeField]
    //private bool testMode = true;  // Bypasses fragment requirement when enabled

    public int collectedFragments = 0;
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

    //spawn key locations at start
    void staticKeySpawn(GameObject key){
        //set positions
        Vector3 pos1 = new Vector3(-32.877f, 2.517f, 31.312f);
        Vector3 pos2 = new Vector3(1.22f, 1.657f, -1.82f);
        Vector3 pos3 = new Vector3(21.5f, 2.508f, -21.9f);
        Vector3 pos4 = new Vector3(-9.7f, 2.26f, -17.6f);

        //instantiate keys at specific positions
        GameObject key1 = Instantiate(key, pos1, Quaternion.identity);
        key1.transform.parent = null;
        GameObject key2 = Instantiate(key, pos2, Quaternion.identity);
        key2.transform.parent = null;
        GameObject key3 = Instantiate(key, pos3, Quaternion.identity);
        key3.transform.parent = null;
        GameObject key4 = Instantiate(key, pos4, Quaternion.identity);
        key4.transform.parent = null;

    }

    void Start(){
        //spawn keys
        staticKeySpawn(keyPrefab);

    }

    // Increments fragment count and logs progress
    public void CollectFragment()
    {
        Debug.Log("CollectFragment called\n");
        collectedFragments++;
        Debug.Log($"Fragments collected: {collectedFragments}/{requiredFragments}");
        //trigger event
        OnKeyCollection?.Invoke();

    }

    // Returns true if all fragments collected or in test mode
    public bool HasAllFragments()
    {
        //return testMode || collectedFragments >= requiredFragments;
        //getting rid of test mode
        return collectedFragments >= requiredFragments;
    }

    //returns keycount for the UI display
    public int keyCount(){
        return collectedFragments;
    }

}