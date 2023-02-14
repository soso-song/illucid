// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using Deform;
using float3 = Unity.Mathematics.float3;
// using Assets.Scripts; // to access Slicer

public class Superview : MonoBehaviour
{

    public float smoothFOV = 0.2f; // Adjust this to change the smoothing speed
    [Header("DebugVariables")]
    // public Transform target;            // The target object we picked up for scaling
    public float fieldOfView;
    // public float distanceToTarget;
    // public float frustumHeightAtDistance;
    // public float frustumWidthAtDistance;
    public float distLatticeFront;
    public float distLatticeBack;
    public float CurrLatticeFrontRatio;
    public float CurrLatticeBackRatio;
    public float viewChangePersentage;
    // public bool EnableCameraClippingPlaneShift;
    // public Transform door;

    [Header("Parameters")]
    // public LayerMask targetMask;        // The layer mask used to hit only potential targets with a raycast
      // The layer mask used to ignore the player and target objects while raycasting
    public float holdDistance;          // The offset amount for positioning the object so it doesn't clip into walls
    public float FOVSensitivity;
    public float CPSensitivity;
    public float DFSensitivityFront;
    public float DFSensitivityBack;
    public float DFSensitivityDepth;
    public float minPOV;
    public float maxPOV;
    public bool EnableCameraClippingPlaneShift;

    private Camera playerCamera;

    private Transform LatticeTrans;
    private GameObject LatticeObj;
    private LatticeDeformer latticeDeformer;
    private float minFrustumHeightAtDistLatticeFront;
    private float minFrustumHeightAtDistLatticeBack;

    public float3 FTL;
    public float3 FTR;
    public float3 FBR;
    public float3 FBL;
    public float3 BTL;
    public float3 BTR;
    public float3 BBR;
    public float3 BBL;

    // depth of object Lattice

    float currFrustumHeightAtDistLatticeFront;
    float currFrustumHeightAtDistLatticeBack;
    
    void Start()
    {
        playerCamera = Camera.main;
        fieldOfView = playerCamera.fieldOfView;
    }
    void Update()
    {
        UpdateFOV();
    }

    public void InitDeform(Transform target)
    {
        target.rotation = Quaternion.Euler(0,0,0);

        // Set our target scale to be the same as the original for the time being
        // targetScale = target.localScale;

        LatticeTrans = target.GetChild(0);
        LatticeObj = LatticeTrans.gameObject;
        // // get the Vector3Int resolution of the Lattice
        // Vector3Int resolution = Lattice.GetComponent<Lattice>().resolution;
        // // print the resolution
        // print(Lattice.Resolution);
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
        distLatticeFront = holdDistance - distanceBetweenFTLAndBTL/2;
        // get the distance between the playerCamera and the object's back lattice face
        distLatticeBack = holdDistance + distanceBetweenFTLAndBTL/2;

        minFrustumHeightAtDistLatticeFront = 2.0f * distLatticeFront * Mathf.Tan(minPOV * 0.5f * Mathf.Deg2Rad);
        minFrustumHeightAtDistLatticeBack = 2.0f * distLatticeBack * Mathf.Tan(minPOV * 0.5f * Mathf.Deg2Rad);
    }

    public void DeformTarget(Transform target)
    {
        if(LatticeObj != null)
        {
            // cube_script = cube.GetComponent<CubeScript>();
            latticeDeformer = LatticeObj.GetComponent<LatticeDeformer>();
            // print the resolution from the LatticeDeformer script
            LatticeTrans.LookAt(transform.position);

            // print(latticeDeformer.Resolution);
            // frustumHeightAtDistance = 2.0f * holdDistance * Mathf.Tan(playerCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            // frustumWidthAtDistance = frustumHeightAtDistance * playerCamera.aspect;

            currFrustumHeightAtDistLatticeFront = 2.0f * distLatticeFront * Mathf.Tan(playerCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            currFrustumHeightAtDistLatticeBack = 2.0f * distLatticeBack * Mathf.Tan(playerCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);

            CurrLatticeFrontRatio = currFrustumHeightAtDistLatticeFront/minFrustumHeightAtDistLatticeFront;
            CurrLatticeBackRatio = currFrustumHeightAtDistLatticeBack/minFrustumHeightAtDistLatticeBack;

            // notice the frontRatio and backRatio are the same
            // notice the height and width ratio is also the same:
            //     var frustumWidth = frustumHeight * camera.aspect;
            //     var frustumHeight = frustumWidth / camera.aspect;

            // assign the float3[] ControlPoints to F0, F1, F2, F3, B0, B1, B2, B3

            // FTL = latticeDeformer.ControlPoints[7];
            // FTR = latticeDeformer.ControlPoints[6];
            // FBR = latticeDeformer.ControlPoints[4];
            // FBL = latticeDeformer.ControlPoints[5];

            // BTL = latticeDeformer.ControlPoints[3];
            // BTR = latticeDeformer.ControlPoints[2];
            // BBR = latticeDeformer.ControlPoints[0];
            // BBL = latticeDeformer.ControlPoints[1];

            float frontDeformRatio = 1+DFSensitivityFront*viewChangePersentage; // view 1->0 then DeformRatio 1->2   //1.4
            float backDeformRatio =  1+DFSensitivityBack*viewChangePersentage;//1-DFSensitivityBack*(viewChangePersentage); // view 1->0 then DeformRatio 1->0.5 //0.82
            
            if(target.name != "DeformCube")
            {
                DFSensitivityDepth = DFSensitivityDepth/3;
            }

            float depthDeformRatio = 1+DFSensitivityDepth*viewChangePersentage;
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

        }

        // relation of FOV and distance: https://docs.unity3d.com/Manual/FrustumSizeAtDistance.html
    }

    void UpdateFOV()
    {
        // Use Lerp to smoothly change the FOV value
        fieldOfView = Mathf.Lerp(
            fieldOfView, 
            fieldOfView - FOVSensitivity * Input.mouseScrollDelta.y, 
            smoothFOV
        );


        if (fieldOfView < minPOV)
        {
            fieldOfView = minPOV;
        }else if (fieldOfView > maxPOV)
        {
            fieldOfView = maxPOV;
        }
        playerCamera.fieldOfView = fieldOfView;

        viewChangePersentage = (fieldOfView-minPOV)/(maxPOV-minPOV);
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
