//
// Created by Soso Song (@sososong) on 3/11/2023
// Copyright (c) 2023 Zhifei(Soso) Song. All rights reserved.
//

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
            // let partical child of the Key enable
            Key.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
    // void OnTriggerExit(Collider other)
    // {
    //     if(other.gameObject.name == "Player"){
    //         Key.layer = 0;
    //     }
    // }
}
