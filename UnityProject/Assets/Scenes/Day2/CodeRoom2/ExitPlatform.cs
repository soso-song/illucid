using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPlatform : MonoBehaviour
{
    public GameObject Key;
    // Start is called before the first frame update
    void Start()
    {
        Key.layer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        // if player is entering the platform
        if(other.gameObject.name == "Player"){
            Key.layer = 12;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.name == "Player"){
            Key.layer = 0;
        }
    }
}
