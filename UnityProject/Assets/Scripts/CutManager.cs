using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
// using Thirdparty;

public class CutManager : MonoBehaviour
{
    // assume N is cutter which is near the camera
    private int LAYER_CUTABLE = 12;
    private int LAYER_ZOOMABLE = 13;
    IllusionManager illusionManager;
    
    public GameObject cutterN;
    public GameObject cutterF;
    GameObject cutterNL;
    GameObject cutterNR;
    GameObject cutterFL;
    GameObject cutterFR;
    // create 4 Plane variables
    // assign them to the 4 planes

    // access target variable from IllusionManager.cs
    
    // Start is called before the first frame update
    void Start()
    {
        // get the child of cutterN called "PivotL"
        // access target variable from script IllusionManager.cs
        illusionManager = GetComponent<IllusionManager>();

        // cutterNL = null;
        // cutterNR = null;
        // cutterFL = null;
        // cutterFR = null;
        // cutterNL = cutterN.transform.Find("PivotL").gameObject.transform.Find("Plane").Plane;
        // cutterNR = cutterN.transform.Find("PivotR").gameObject.transform.Find("Plane").Plane;
        // cutterFL = cutterF.transform.Find("PivotL").gameObject.transform.Find("Plane").Plane;
        // cutterFR = cutterF.transform.Find("PivotR").gameObject.transform.Find("Plane").Plane;
    }

    // Update is called once per frame
    void Update()
    {
        // check if cutterNL has been assigned

        if(illusionManager.target != null && cutterNR == null)
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

        if (Input.GetMouseButtonDown(0) && illusionManager.target != null && illusionManager.target.gameObject.layer == LAYER_CUTABLE)
        {
            Plane cutterNLPlane = new Plane(cutterNL.transform.up, cutterN.transform.position);
            GameObject[] slices = Slicer.Slice(cutterNLPlane, illusionManager.target.gameObject);
            Destroy(illusionManager.target.gameObject);
            // Rigidbody rigidbody = slices[1].GetComponent<Rigidbody>();
        }
    }
}
