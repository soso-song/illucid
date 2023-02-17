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
