using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountManager : MonoBehaviour
{
    // Start is called before the first frame update
    private TextMeshProUGUI keyCountText;
    void Start()
    {
       
        keyCountText = GetComponent<TextMeshProUGUI>();
        //connect to the collection event
        KeyFragmentManager.Instance.OnKeyCollection.AddListener(UpdateKeyCountText);
        //initializes count
        UpdateKeyCountText();

    }


    // Update is called once per frame
    public void UpdateKeyCountText()
    {
        //will be triggered using unity event
        Debug.Log("trying to update KeyCountText\n");
        keyCountText.text = KeyFragmentManager.Instance.keyCount().ToString();
    }
}
