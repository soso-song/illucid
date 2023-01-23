using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Deform;
using float3 = Unity.Mathematics.float3;

public class IllusionManager : MonoBehaviour
{
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
    public Transform door;


 
    [Header("Parameters")]
    public LayerMask targetMask;        // The layer mask used to hit only potential targets with a raycast
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
        if (outline != null) {
            // disable outline
            outline.enabled = false;
        }
        RaycastHit hit;
        if (target == null)
        {
            if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, targetMask))
            {
                // check if hit object is in layer targetable
                if (hit.collider.gameObject.layer == 10) {
                    // add outline to the object
                    outline = hit.collider.GetComponent<Outline>();
                    if (outline != null) {
                        outline.enabled = true;
                    }
                }
            }
        }
        HandleInput();
        if (target != null)
        {
            // holding the object in front of the player 

            // target.position = transform.position + transform.forward * holdDistance; //* targetScale.x;

            if(EnableCameraClippingPlaneShift)
            {
                target.position = transform.parent.transform.position +  transform.parent.transform.up * 3 + 
                                transform.forward * holdDistance; //* targetScale.x;
            }
            else
            {
                target.position = transform.position + transform.forward * holdDistance;
            }
           
            // get the parent's transform position

            // originalScale = target.localScale.x;originalScale * CurrLatticeFrontRatio;

            // target.localScale = new Vector3(
            //     originalScale * CurrLatticeFrontRatio, 
            //     originalScale * CurrLatticeFrontRatio, 
            //     originalScale * CurrLatticeFrontRatio);

            // making the object always facing the player
            // target.LookAt(transform.position);
            DeformTarget();
            // change the target's scale
            // target.localScale *= CurrLatticeBackRatio;
        }
        // slide door to right during 1 second
        door.position = Vector3.Lerp(door.position, new Vector3(0, 0, 0), Time.deltaTime);
        // slide door to left during 1 second
        door.position = Vector3.Lerp(door.position, new Vector3(0, 0, 0), Time.deltaTime);

        // Vector3 p = playerCamera.ViewportToWorldPoint(new Vector3(1, 1, playerCamera.nearClipPlane));
        // Gizmos.color = Color.yellow;
        // Gizmos.DrawSphere(p, 0.1F);
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

    void HandleInput()
    {
        // Check for left mouse click
        if (Input.GetMouseButtonDown(0))
        {
            // If we do not currently have a target
            if (target == null)
            {
                // Fire a raycast with the layer mask that only hits potential targets
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, targetMask))
                {
                    // Set our target variable to be the Transform object we hit with our raycast
                    target = hit.transform;
 
                    // Disable physics for the object
                    target.GetComponent<Rigidbody>().isKinematic = true;

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
                }
            }
            // If we DO have a target
            else
            {
                // Reactivate physics for the target object
                // check if the target is cube
                if(target.name != "Cube")
                {
                    target.GetComponent<Rigidbody>().isKinematic = false;
                }
 
                // Set our target variable to null
                
                // OrigfrustumHeightAtDistLatticeFront = 0;
                // OrigfrustumHeightAtDistLatticeBack = 0;

                // target.GetComponent<MeshCollider>().mesh = null;
                // get the mesh of mesh collider of the target
                // MeshFilter meshFilter = target.GetComponent<MeshFilter>();
                // get the mesh from mesh renderer of the target
                // Mesh mesh = target.GetComponent<MeshFilter>().mesh;
                // assign the mesh of mesh collider of the target to be the mesh of the target
                // target.GetComponent<MeshCollider>().sharedMesh = meshFilter.mesh;
                // target.GetComponent<MeshCollider>().sharedMesh = meshFilter.mesh;

                target = null;
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
        // get the child object of the target called "Lattice"
        // get child gameobject of target called "Lattice"
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
