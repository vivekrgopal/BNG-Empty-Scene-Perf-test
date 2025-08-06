using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRateLocker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 90;

        OVRManager.display.displayFrequency = 90;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
