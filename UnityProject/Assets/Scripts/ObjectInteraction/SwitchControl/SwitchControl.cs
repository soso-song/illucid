using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SwitchControl : MonoBehaviour
{
    int switchOn = 0; 
    public IllusionManager IllusionManager;
    public AudioSource switchSound;
    public UnityEvent onSwitch;

    // Start is called before the first frame update
    void Start()
    {
        IllusionManager = GameObject.FindObjectOfType<IllusionManager>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (IllusionManager.target != null)
        {
        }
    }
}
