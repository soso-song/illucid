//
// Created by Soso Song (@sososong) on 3/11/2023
// Copyright (c) 2023 Zhifei(Soso) Song. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    public L2Room1Manager room;
    public bool wantSlice = false;
    public bool triggered = false;
    // Start is called before the first frame update
    Color ColorA;
    Color ColorB;

    public Transform triggerObj;


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

    void OnTriggerEnter(Collider other)
    {   
        // if its cuable that is not holding
        if (other.gameObject.layer == 12 && other.gameObject.transform.parent == null){
            // if tag is Untagged
            if(other.gameObject.tag == "Untagged"){
                // change the color of the parent object
                transform.parent.GetComponent<Renderer>().material.color = ColorA;
                if (!wantSlice){
                    triggered = true;
                    triggerObj = other.gameObject.transform;
                    room.CheckPassCond(); // efficiency way without Update()
                }

            }else{
                transform.parent.GetComponent<Renderer>().material.color = ColorB;
                if (wantSlice){
                    triggered = true;
                    triggerObj = other.gameObject.transform;
                    room.CheckPassCond();
                }
            }
            other.gameObject.transform.parent = transform.parent; // remove with parent
        }
    }

    void OnTriggerExit(Collider other)
    {
        transform.parent.GetComponent<Renderer>().material.color = Color.white;
        triggered = false;
    }

}
