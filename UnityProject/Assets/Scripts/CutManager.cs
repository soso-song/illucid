using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutManager : MonoBehaviour
{
    // assume N is cutter which is near the camera
    IllusionManager illusionManager;
    
    public GameObject cutterN;
    public GameObject cutterF;
    GameObject cutterNL;
    GameObject cutterNR;
    GameObject cutterFL;
    GameObject cutterFR;
    // access target variable from IllusionManager.cs
    
    // Start is called before the first frame update
    void Start()
    {
        // get the child of cutterN called "PivotL"
        // access target variable from script IllusionManager.cs
        illusionManager = GetComponent<IllusionManager>();

        // cutterNL = cutterN.transform.Find("PivotL").gameObject.transform.Find("Plane").gameObject;
        // cutterNR = cutterN.transform.Find("PivotR").gameObject.transform.Find("Plane").gameObject;
        // cutterFL = cutterF.transform.Find("PivotL").gameObject.transform.Find("Plane").gameObject;
        // cutterFR = cutterF.transform.Find("PivotR").gameObject.transform.Find("Plane").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(illusionManager.target != null && cutterNL == null)
        {
            cutterNL = cutterN.transform.Find("PivotL").gameObject.transform.Find("Plane").gameObject;
            cutterNR = cutterN.transform.Find("PivotR").gameObject.transform.Find("Plane").gameObject;
            cutterFL = cutterF.transform.Find("PivotL").gameObject.transform.Find("Plane").gameObject;
            cutterFR = cutterF.transform.Find("PivotR").gameObject.transform.Find("Plane").gameObject;
        }

        // if (illusionManager.target != null){
        //     if (cutterNL.GetComponent<Renderer>().bounds.Intersects(illusionManager.target.GetComponent<Renderer>().bounds)){
        //         print("cutterNL");
        //     }
        //     if (cutterNR.GetComponent<Renderer>().bounds.Intersects(illusionManager.target.GetComponent<Renderer>().bounds))
        //     {
        //         print("cutterNR");
        //     }
        //     if (cutterFL.GetComponent<Renderer>().bounds.Intersects(illusionManager.target.GetComponent<Renderer>().bounds))
        //     {
        //         print("cutterFL");
        //     }
        //     if (cutterFR.GetComponent<Renderer>().bounds.Intersects(illusionManager.target.GetComponent<Renderer>().bounds))
        //     {
        //         print("cutterFR");
        //     }

        // }

        if (Input.GetMouseButtonDown(0) && illusionManager.target != null)
        {
            // print true if 
        }
    }
}
