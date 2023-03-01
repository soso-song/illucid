// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using Deform;
using float3 = Unity.Mathematics.float3;
// using Assets.Scripts; // to access Slicer

public class Superview : MonoBehaviour
{
    public float smoothResetFOVAnimationSpeed = 250; // Adjust this to change the smoothing speed
    [Header("DebugVariables")]
    // public float currFOV;
    public float initFOV;
    public float currFOV;
    public float FOVDiff;
    // public float distLatticeFront;
    // public float distLatticeBack;
    public float viewChangePersentage;
    // public bool EnableCameraClippingPlaneShift;
    // public Transform door;

    [Header("Parameters")]
    // public LayerMask targetMask;        // The layer mask used to hit only potential targets with a raycast
      // The layer mask used to ignore the player and target objects while raycasting
    // public float holdDistance=5;          // The offset amount for positioning the object so it doesn't clip into walls
    public float mouseSensitivity=4;
    private float LatticeSensitivity, LatticeSensitivityFront, LatticeSensitivityDepth, LatticeSensitivityBack;
    public float minFOV = 30, maxFOV = 110, midFOV;
    private Camera playerCamera;
    private Transform LatticeTrans;
    private GameObject LatticeObj;
    private Deformable deformable;
    private LatticeDeformer latticeDeformer;
    private bool lockFOV = true;

    public float3 FTL, FTR, FBR, FBL, BTL, BTR, BBR, BBL;

    // private variables from TargetController
    private TargetController targetController;
    public float targetStartScale, targetCurrScale, targetMinScale, targetMaxScale;
    
    // not useful
    // public bool EnableCameraClippingPlaneShift;
    // public float CPSensitivity;
    // private AudioClip zoomLimitSound;
    public AudioSource zoomLimitSound;

    void Start()
    {
        playerCamera = Camera.main;
        currFOV = playerCamera.fieldOfView;
        midFOV = (minFOV + maxFOV) / 2;
        // zoomLimitSound = Resources.Load<AudioClip>("Sounds/ZoomLimit");
    }

    void Update()
    {
        // if not holding object
        if (!lockFOV)
            UpdateFOV();
        else
            ResetFOV(smoothResetFOVAnimationSpeed);
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
        // disable mesh collider
        // target.GetComponent<MeshCollider>().enabled = false;
        targetController = target.GetComponent<TargetController>();
        LatticeSensitivity = targetController.LatticeSensitivity;
        LatticeSensitivityFront = targetController.LatticeSensitivityFront;
        LatticeSensitivityBack = targetController.LatticeSensitivityBack;
        LatticeSensitivityDepth = targetController.LatticeSensitivityDepth;
        targetCurrScale = targetController.currObjFOV;
        targetMinScale = targetController.minObjFOV;
        targetMaxScale = targetController.maxObjFOV;
        targetStartScale = targetCurrScale;

        lockFOV = false;
    }

    public void UpdateDeform(Transform target)
    {
        LatticeTrans.LookAt(transform.position);

        // set the rotation of the lattice to be zero
        // LatticeTrans.rotation = Quaternion.Euler(0,0,0);

        // print the resolution from the LatticeDeformer script
        FOVDiff = currFOV - initFOV;
        targetCurrScale = targetStartScale+FOVDiff;

        float frontDeformRatio = LatticeSensitivityFront*LatticeSensitivity*targetCurrScale; // view 1->0 then DeformRatio 1->2   //1.4
        float backDeformRatio  = LatticeSensitivityBack *LatticeSensitivity*targetCurrScale;//1-LatticeSensitivityBack*(viewChangePersentage); // view 1->0 then DeformRatio 1->0.5 //0.82
        float depthDeformRatio = LatticeSensitivityDepth*LatticeSensitivity*targetCurrScale*2; // depth is oneway expend, so *2

        // print("frontDeformRatio: "+frontDeformRatio);
        // if the target is cube
        if(targetCurrScale < targetMinScale && frontDeformRatio < 1)
        {
            targetCurrScale = targetMinScale;
            return;
        }
        if(targetCurrScale > targetMaxScale && frontDeformRatio > 1)
        {
            targetCurrScale = targetMaxScale;
            return;
        }
        if(LatticeObj == null)
        {
            Debug.Log("LatticeObj is null, Superview init failed");
            return;
        }
        // cube_script = cube.GetComponent<CubeScript>();
        latticeDeformer = LatticeObj.GetComponent<LatticeDeformer>();

        latticeDeformer.ControlPoints[7] = new Vector3(
            0.5f+(frontDeformRatio),
            0.5f+(frontDeformRatio),
            FTL.z
        );
        latticeDeformer.ControlPoints[6] = new Vector3(
            -0.5f-(frontDeformRatio),
            0.5f+(frontDeformRatio),
            FTR.z
        );
        latticeDeformer.ControlPoints[4] = new Vector3(
            -0.5f-(frontDeformRatio),
            -0.5f-(frontDeformRatio),
            FBR.z
        );
        latticeDeformer.ControlPoints[5] = new Vector3(
            0.5f+(frontDeformRatio),
            -0.5f-(frontDeformRatio),
            FBL.z
        );

        latticeDeformer.ControlPoints[3] = new Vector3(
            0.5f+(backDeformRatio),
            0.5f+(backDeformRatio),
            -0.5f-(depthDeformRatio)
        );
        latticeDeformer.ControlPoints[2] = new Vector3(
            -0.5f-(backDeformRatio),
            0.5f+(backDeformRatio),
            -0.5f-(depthDeformRatio)
        );
        latticeDeformer.ControlPoints[0] = new Vector3(
            -0.5f-(backDeformRatio),
            -0.5f-(backDeformRatio),
            -0.5f-(depthDeformRatio)
        );
        latticeDeformer.ControlPoints[1] = new Vector3(
            0.5f+(backDeformRatio),
            -0.5f-(backDeformRatio),
            -0.5f-(depthDeformRatio)
        );

        FTL = latticeDeformer.ControlPoints[7];
        FTR = latticeDeformer.ControlPoints[6];
        FBR = latticeDeformer.ControlPoints[4];
        FBL = latticeDeformer.ControlPoints[5];

        BTL = latticeDeformer.ControlPoints[3];
        BTR = latticeDeformer.ControlPoints[2];
        BBR = latticeDeformer.ControlPoints[0];
        BBL = latticeDeformer.ControlPoints[1];
        // latticeDeformer.ControlPoints[6] = ;
        // latticeDeformer.ControlPoints[4] = ;
        // latticeDeformer.ControlPoints[5] = ;
        // relation of FOV and distance: https://docs.unity3d.com/Manual/FrustumSizeAtDistance.html
        // return viewChangePersentage;
    }
    public void DetachDeform(Transform target)
    {
        // enable mesh collider
        // target.GetComponent<MeshCollider>().enabled = true;
        target.GetComponent<TargetController>().currObjFOV = targetCurrScale;
        deformable = target.GetComponent<Deformable>();
        deformable.RecalculateMeshCollider();
        // deformable.ColliderRecalculation = ColliderRecalculation.Auto;
        // deformable.ColliderRecalculation = ColliderRecalculation.None;
        lockFOV = true;
    }

    void UpdateFOV()
    {
        float fovChange = mouseSensitivity * Input.mouseScrollDelta.y;
        
        // MacOS touch mouse scroll is continuous
        // windows mouse scroll is discrete, and below code is not working
        // Use Lerp to smoothly change the FOV value
        // currFOV = Mathf.Lerp(
        //     currFOV, 
        //     currFOV - mouseSensitivity * Input.mouseScrollDelta.y, 
        //     smoothFOVAnimation
        // );

        // bound between object size
        if(targetCurrScale == targetMinScale && fovChange > 0){
            zoomLimitSound.Play();
            return;
        }
            
        if(targetCurrScale == targetMaxScale && fovChange < 0){
            zoomLimitSound.Play();
            return;
        }

        currFOV = currFOV - fovChange;
        // bound between max and min FOV
        if (currFOV < minFOV)
            currFOV = minFOV;
        else if (currFOV > maxFOV)
            currFOV = maxFOV;

        playerCamera.fieldOfView = currFOV;
        viewChangePersentage = (currFOV-initFOV)/LatticeSensitivity;

        // // not going to run
        // if(EnableCameraClippingPlaneShift)
        // {
        //    // camera move back
        //     playerCamera.nearClipPlane = CPSensitivity*(1-viewChangePersentage)+0.01f;
        //     // shift z position of playerCamera
        //     playerCamera.transform.localPosition = new Vector3(
        //         playerCamera.transform.localPosition.x, 
        //         playerCamera.transform.localPosition.y, 
        //         -(playerCamera.nearClipPlane)
        //     );
        // }
    }
    void ResetFOV(float speed)
    {
        // playerCamera.fieldOfView = Mathf.MoveTowards(playerCamera.fieldOfView, midFOV, 2 * Time.deltaTime);
        // playerCamera.fieldOfView = midFOV;
        currFOV =  Mathf.MoveTowards(playerCamera.fieldOfView, midFOV, speed * Time.deltaTime);
        playerCamera.fieldOfView = currFOV;
    }
}
