using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ScreenShake : MonoBehaviour
{

    public static ScreenShake Instance {get; private set;}
    private CinemachineImpulseSource cinemachineImpulseSource; 

    private void Awake() 
    {
        
        if(Instance != null)
        {
            Debug.Log("There is more than one instance of UnitActionSystem" + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();    

    }

    

    public void Shake(float intensity)
    {
        cinemachineImpulseSource.GenerateImpulse(intensity);
    }
}
