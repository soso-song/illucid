using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Deform;
using float3 = Unity.Mathematics.float3;

public class IllusionManager : MonoBehaviour
{
    private int LAYER_CUTABLE = 12;
    private int LAYER_ZOOMABLE = 13;
    int targetMask = 1 << 12 | 1 << 13;

    [Header("DebugVariables")]
    public Transform target;            // The target object we picked up for scaling
    public float fieldOfView;
    // public float distanceToTarget;
    // public float frustumHeightAtDistance;
    // public float frustumWidthAtDistance;
    public float distLatticeFront;
    public float distLatticeBack;
    public float CurrLatticeFrontRatio;
    public float CurrLatticeBackRatio;
    public float viewChangePersentage;
    public bool EnableCameraClippingPlaneShift;
    // public Transform door;

    [Header("Parameters")]
    // public LayerMask targetMask;        // The layer mask used to hit only potential targets with a raycast
    // public LayerMask ignoreTargetMask;  // The layer mask used to ignore the player and target objects while raycasting
    public float holdDistance;          // The offset amount for positioning the object so it doesn't clip into walls
    public float FOVSensitivity;
    public float CPSensitivity;
    public float DFSensitivityFront;
    public float DFSensitivityBack;
    public float DFSensitivityDepth;
    public float minPOV;
    public float maxPOV;
 
    float originalDistance;             // The original distance between the player playerCamera and the target
    float originalScale;                // The original scale of the target objects prior to being resized
    // Vector3 targetScale;                // The scale we want our object to be set to each frame

    public Outline outline;

    private Camera playerCamera;
    void Start()
    {
        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;
        playerCamera = Camera.main;
        fieldOfView = playerCamera.fieldOfView;
    }

    void Update()
    {
        HandleInput();
        UpdateOutLine();
        
        if (target != null)
        {
            target.position = transform.position + transform.forward * holdDistance;
            if (target.gameObject.layer == LAYER_ZOOMABLE)
            {   
                DeformTarget();
            }
        }
        
        // else if (target != null && target.gameObject.layer == LAYER_CUTABLE)
        // {
        //     CutTarget();
        // }
       
    }

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

    // assign the float3[] ControlPoints to F0, F1, F2, F3, B0, B1, B2, B3
    // BBR = latticeDeformer.ControlPoints[0];
    // BBL = latticeDeformer.ControlPoints[1];
    // BTR = latticeDeformer.ControlPoints[2];
    // BTL = latticeDeformer.ControlPoints[3];
    // FBR = latticeDeformer.ControlPoints[4];
    // FBL = latticeDeformer.ControlPoints[5];
    // FTR = latticeDeformer.ControlPoints[6];
    // FTL = latticeDeformer.ControlPoints[7];
    
    // depth of object Lattice
    bool deformInited = false;

    void UpdateOutLine(){
        if (outline != null) {
            // disable outline
            outline.enabled = false;
        }
        RaycastHit hit;
        if (target == null)
        {
            // send raycast on two layers

            if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, targetMask))
            {
                // check if hit object is in layer targetable
                if (hit.collider.gameObject.layer == LAYER_ZOOMABLE || hit.collider.gameObject.layer == LAYER_CUTABLE) {
                    // add outline to the object
                    outline = hit.collider.GetComponent<Outline>();
                    if (outline != null) {
                        outline.enabled = true;
                    }
                }
            }
        }
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0)) // left mouse click
        {
            if (target == null)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, targetMask))
                {
                    target = hit.transform;
                    target.GetComponent<Rigidbody>().isKinematic = true;
                }
            }
            else // If we DO have a target
            {
                if(target.name != "Cube")
                {
                    target.GetComponent<Rigidbody>().isKinematic = false;
                }
                target = null;
                deformInited = false;
            }
        }

        // print the mouse scroll wheel value
        // print(Input.mouseScrollDelta.y);
        // if the mouse scroll wheel value is greater than 0
        // if playerCamera.fieldOfView is between minPOV and maxPOV
        fieldOfView -= FOVSensitivity*Input.mouseScrollDelta.y;
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

    float currFrustumHeightAtDistLatticeFront;
    float currFrustumHeightAtDistLatticeBack;
    

    void DeformTarget()
    {
        if(!deformInited){
            // set the target rotation to be 0,0,0
            target.rotation = Quaternion.Euler(0,0,0);

            // Calculate the distance between the playerCamera and the object
            originalDistance = Vector3.Distance(transform.position, target.position);

            // Save the original scale of the object into our originalScale Vector3 variabble
            originalScale = target.localScale.x;

            // Set our target scale to be the same as the original for the time being
            // targetScale = target.localScale;

            LatticeTrans = target.GetChild(0);
            LatticeObj = LatticeTrans.gameObject;
            // // get the Vector3Int resolution of the Lattice
            // Vector3Int resolution = Lattice.GetComponent<Lattice>().resolution;
            // // print the resolution
            // print(Lattice.Resolution);
            if(LatticeObj != null)
            {
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
            deformInited = true;
        }
        

        // get the child object of the target called "Lattice"
        // get child gameobject of target called "Lattice"
        // LatticeTrans = target.GetChild(0);
        // LatticeObj = LatticeTrans.gameObject;
        // // get the Vector3Int resolution of the Lattice
        // Vector3Int resolution = Lattice.GetComponent<Lattice>().resolution;
        // // print the resolution
        // print(Lattice.Resolution);
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
            
            if(target.name != "Cube")
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
}
