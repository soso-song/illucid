//
// Created by Soso Song (@sososong) on 3/11/2023
// Copyright (c) 2023 Zhifei(Soso) Song. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    public bool wantSlice = false;
    // Start is called before the first frame update
    Color ColorA;
    Color ColorB;

    void Start()
    {
        if(wantSlice){
            ColorA = Color.red;
            ColorB = Color.green;
        }else{
            ColorA = Color.green;
            ColorB = Color.red;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 12)
        {
            // if tag is Untagged
            if(other.gameObject.tag == "Untagged"){
                // change the color of the parent object
                transform.parent.GetComponent<Renderer>().material.color = ColorA;
            }else{
                transform.parent.GetComponent<Renderer>().material.color = ColorB;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        transform.parent.GetComponent<Renderer>().material.color = Color.white;
    }
}
