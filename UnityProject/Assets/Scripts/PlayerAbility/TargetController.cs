// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    private Outline outline;
    public int pickTime = 0;
    public int maxPickTime = 3;

    [Header("SuperviewParameters")]
    public float LatticeSensitivity=0.005f;
    public float LatticeSensitivityFront=2;
    public float LatticeSensitivityDepth=4;
    public float LatticeSensitivityBack=4;
    // public float currScaleFront=1;
    // public float currScaleBack=1;
    // public float currScaleDepth=1;
    // public float maxScaleFront=2;
    // public float maxScaleBack=4;
    // public float maxScaleDepth=9;
    public float currObjFOV=0;
    public float minObjFOV=-12;
    public float maxObjFOV=80;

    void Start()
    {
        // add script Outline to the object
        outline = gameObject.AddComponent<Outline>();
        // disable the outline
        // outline.enabled = false;
        outline.OutlineWidth = 0;
    }

    public void IncrementDrop()
    {
        pickTime += 1;
        Debug.Log("Object: " + gameObject.name + " has been picked " + pickTime + " times, cur FOV:" + currObjFOV);
        
        // switch (pickTime)
        // {
        //     case 1:
        //         outline.OutlineColor = Color.green;
        //         break;
        //     case 2:
        //         outline.OutlineColor = Color.blue;
        //         break;
        //     case 3:
        //         outline.OutlineColor = Color.grey;
        //         break;
        // }
    }
}
