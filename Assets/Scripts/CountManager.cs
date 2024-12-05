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
    }

    // Update is called once per frame
    public void UpdateKeyCountText(KeyFragmentManager fragmentManager)
    {
        //will be triggered using unity event
        keyCountText.text = fragmentManager.keyCount().ToString();
    }
}
