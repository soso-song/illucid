// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using Deform;
using float3 = Unity.Mathematics.float3;
// using Assets.Scripts; // to access Slicer

public class Superview : MonoBehaviour
{
    public float smoothFOVAnimation = 0.5f; // Adjust this to change the smoothing speed
    [Header("DebugVariables")]

    public float currFOV;
    public float initFOV;
    // public float distLatticeFront;
    // public float distLatticeBack;
    public float viewChangePersentage;
    // public bool EnableCameraClippingPlaneShift;
    // public Transform door;

    [Header("Parameters")]
    // public LayerMask targetMask;        // The layer mask used to hit only potential targets with a raycast
      // The layer mask used to ignore the player and target objects while raycasting
    public float holdDistance=5;          // The offset amount for positioning the object so it doesn't clip into walls
    public float mouseSensitivity=4;
    public float LatticeSensitivity=150, LatticeSensitivityFront=2, LatticeSensitivityBack=4, LatticeSensitivityDepth=9;
    public float minFOV = 30, maxFOV = 110;
    public bool EnableCameraClippingPlaneShift;
    public float CPSensitivity;

    private Camera playerCamera;

    private Transform LatticeTrans;
    private GameObject LatticeObj;
    private LatticeDeformer latticeDeformer;

    public float3 FTL, FTR, FBR, FBL, BTL, BTR, BBR, BBL;
    
    void Start()
    {
        playerCamera = Camera.main;
        currFOV = playerCamera.fieldOfView;
    }

    void Update()
    {
        UpdateFOV();
    }

    public void InitDeform(Transform target)
    {
        initFOV = currFOV;
        target.rotation = Quaternion.Euler(0,0,0);

        // Set our target scale to be the same as the original for the time being
        // targetScale = target.localScale;

        LatticeTrans = target.GetChild(0);
        LatticeObj = LatticeTrans.gameObject;

        if(LatticeObj == null)
        {
            Debug.Log("LatticeObj is null, Superview init failed");
        }
        // cube_script = cube.GetComponent<CubeScript>();
        latticeDeformer = LatticeObj.GetComponent<LatticeDeformer>();
        // assign the float3[] ControlPoints to F0, F1, F2, F3, B0, B1, B2, B3
        FTL = latticeDeformer.ControlPoints[7];
        FTR = latticeDeformer.ControlPoints[6];
        FBR = latticeDeformer.ControlPoints[4];
        FBL = latticeDeformer.ControlPoints[5];

        BTL = latticeDeformer.ControlPoints[3];
        BTR = latticeDeformer.ControlPoints[2];
        BBR = latticeDeformer.ControlPoints[0];
        BBL = latticeDeformer.ControlPoints[1];

        // get the distance between FTL and BTL
        float distanceBetweenFTLAndBTL = Vector3.Distance(FTL, BTL);
        // get the distance between the playerCamera and the object's front lattice face
        // distLatticeFront = holdDistance - distanceBetweenFTLAndBTL/2;
        // get the distance between the playerCamera and the object's back lattice face
        // distLatticeBack = holdDistance + distanceBetweenFTLAndBTL/2;
    }

    public void UpdateDeform(Transform target)
    {
        if(LatticeObj == null)
        {
            Debug.Log("LatticeObj is null, Superview init failed");
        }
        // cube_script = cube.GetComponent<CubeScript>();
        latticeDeformer = LatticeObj.GetComponent<LatticeDeformer>();
        // print the resolution from the LatticeDeformer script
        LatticeTrans.LookAt(transform.position);

        float frontDeformRatio = 1+LatticeSensitivityFront*viewChangePersentage; // view 1->0 then DeformRatio 1->2   //1.4
        float backDeformRatio =  1+LatticeSensitivityBack*viewChangePersentage;//1-LatticeSensitivityBack*(viewChangePersentage); // view 1->0 then DeformRatio 1->0.5 //0.82
        
        if(target.name != "DeformCube")
        {
            LatticeSensitivityDepth = LatticeSensitivityDepth/3;
        }

        float depthDeformRatio = 1+LatticeSensitivityDepth*viewChangePersentage;
        // if the target is cube

        latticeDeformer.ControlPoints[7] = new Vector3(
            FTL.x*(frontDeformRatio),
            FTL.y*(frontDeformRatio),
            FTL.z
        );
        latticeDeformer.ControlPoints[6] = new Vector3(
            FTR.x*(frontDeformRatio),
            FTR.y*(frontDeformRatio),
            FTR.z
        );
        latticeDeformer.ControlPoints[4] = new Vector3(
            FBR.x*(frontDeformRatio),
            FBR.y*(frontDeformRatio),
            FBR.z
        );
        latticeDeformer.ControlPoints[5] = new Vector3(
            FBL.x*(frontDeformRatio),
            FBL.y*(frontDeformRatio),
            FBL.z
        );

        latticeDeformer.ControlPoints[3] = new Vector3(
            BTL.x*(backDeformRatio),
            BTL.y*(backDeformRatio),
            BTL.z*(depthDeformRatio)
        );
        latticeDeformer.ControlPoints[2] = new Vector3(
            BTR.x*(backDeformRatio),
            BTR.y*(backDeformRatio),
            BTR.z*(depthDeformRatio)
        );
        latticeDeformer.ControlPoints[0] = new Vector3(
            BBR.x*(backDeformRatio),
            BBR.y*(backDeformRatio),
            BBR.z*(depthDeformRatio)
        );
        latticeDeformer.ControlPoints[1] = new Vector3(
            BBL.x*(backDeformRatio),
            BBL.y*(backDeformRatio),
            BBL.z*(depthDeformRatio)
        );
        // latticeDeformer.ControlPoints[6] = ;
        // latticeDeformer.ControlPoints[4] = ;
        // latticeDeformer.ControlPoints[5] = ;
        // relation of FOV and distance: https://docs.unity3d.com/Manual/FrustumSizeAtDistance.html
    }
    // public void DetachDeform()
    // {
        
    // }

    void UpdateFOV()
    {
        // Use Lerp to smoothly change the FOV value
        currFOV = Mathf.Lerp(
            currFOV, 
            currFOV - mouseSensitivity * Input.mouseScrollDelta.y, 
            smoothFOVAnimation
        );

        if (currFOV < minFOV)
        {
            currFOV = minFOV;
        }else if (currFOV > maxFOV)
        {
            currFOV = maxFOV;
        }
        playerCamera.fieldOfView = currFOV;

        // float currFOVChangePersentage = (currFOV-minFOV)/(maxFOV-minFOV);
        // float initFOVChangePersentage = (initFOV-minFOV)/(maxFOV-minFOV);
        // viewChangePersentage = Mathf.Abs(currFOVChangePersentage - initFOVChangePersentage);
        // viewChangePersentage = (currFOV-minFOV)/(maxFOV-minFOV);
        // viewChangePersentage = (currFOV-minFOV)/(initFOV-minFOV);
        viewChangePersentage = (currFOV-initFOV)/LatticeSensitivity;

        if(EnableCameraClippingPlaneShift)
        {
           // camera move back
            playerCamera.nearClipPlane = CPSensitivity*(1-viewChangePersentage)+0.01f;
            // shift z position of playerCamera
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x, 
                playerCamera.transform.localPosition.y, 
                -(playerCamera.nearClipPlane)
            );
        }
    }
}
