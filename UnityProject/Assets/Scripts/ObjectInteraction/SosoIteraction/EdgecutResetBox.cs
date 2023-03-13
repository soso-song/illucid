//
// Created by Soso Song (@sososong) on 3/11/2023
// Copyright (c) 2023 Zhifei(Soso) Song. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgecutResetBox : MonoBehaviour
{
    public float destroyDuration = 3f;
    // Start is called before the first frame update
    Edgecut edgecut;

    public bool keepSlice = false;

    void Start()
    {
        edgecut = Camera.main.GetComponent<Edgecut>();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 12)
        {
            if (other.gameObject.transform.parent == null){ // not holding
                // get the name of the object
                if(other.gameObject.name.Contains("_")){ // is slice
                    // get the name of the object which before the "_"
                    string name = other.gameObject.name.Split('_')[0];
                    // deactive the object
                    other.gameObject.SetActive(false);
                    if(!keepSlice){
                        print("remove every " + name);
                        RemoveObjectWithName(name);
                    }
                    RecoverObjectWithName(name);
                }
                // check if the object name has "_" in it

                // Destroy(other.gameObject);
            }
                // print("no parent, make new");
        }
    }

    void RemoveObjectWithName(string name){
        GameObject[] slices = GameObject.FindGameObjectsWithTag("Slice");
        foreach (GameObject slice in slices) {
            if (slice.name.Contains(name+"_")) {
                // destroy the object during time
                StartCoroutine(slice.GetComponent<TargetController>().ScaleDestroyOverTime(destroyDuration));
            }
        }
        // get child with name "EdgecutBackupBucket"
        // GameObject EdgecutBackupBucket = GameObject.Find("EdgecutBackupBucket");
    }

    void RecoverObjectWithName(string name){
        Transform original = edgecut.CutableBackupBucket.transform.Find(name);
        GameObject dulplicate = Instantiate(original.gameObject);
        dulplicate.transform.position = transform.position;
        dulplicate.gameObject.SetActive(true);
    }
}
