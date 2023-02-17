// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    private Outline outline;
    public int pickTime = 0;
    public int maxPickTime = 3;

    [Header("SuperviewParameters")]
    public float LatticeSensitivity=150;
    public float LatticeSensitivityFront=2;
    public float LatticeSensitivityBack=4;
    public float LatticeSensitivityDepth=9;
    // public float currScaleFront=1;
    // public float currScaleBack=1;
    // public float currScaleDepth=1;
    // public float maxScaleFront=2;
    // public float maxScaleBack=4;
    // public float maxScaleDepth=9;
    public float currScale=1;
    public float minScale=0.1f;
    public float maxScale=4;

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
