// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
// using Deform;
// using float3 = Unity.Mathematics.float3;
// using Assets.Scripts; // to access Slicer

public class IllusionManager : MonoBehaviour
{
    private int LAYER_CUTABLE = 12;
    private int LAYER_ZOOMABLE = 13;
    private int targetMask = 1 << 12 | 1 << 13;

    [Header("DebugVariables")]
    public Transform target;            // The target object we picked up for scaling
    public Outline outline;
    
    [Header("Parameters")]
    // public LayerMask targetMask;        // The layer mask used to hit only potential targets with a raycast
      // The layer mask used to ignore the player and target objects while raycasting
    public float holdDistance;          // The offset amount for positioning the object so it doesn't clip into walls
    public PickupController PickupController;

    [Header("Superview")]
    public Superview Superview;
    public bool deforming = false;

    void Start()
    {
        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        UpdateOutLine();
        HandleInput(); // updates target
        if (target != null)
        {
            // HoldTarget();
            if (target.gameObject.layer == LAYER_ZOOMABLE)
            {   
                if (!deforming)
                {
                    Superview.InitDeform(target); // init lattice deformer
                    deforming = true;
                }else{
                    Superview.UpdateDeform(target); // return viewChangePersentage
                }
            }
        }
        // else if (target != null && target.gameObject.layer == LAYER_CUTABLE)
        // {
        //     CutTarget();
        // }
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0)) // left mouse click
        {
            if (target == null) // If we dont have a target
            {
                target = PickupController.RaycastFromCamera(targetMask);
                if(target){
                    PickupController.PickupTarget(target.transform.gameObject);
                    // target.GetComponent<Rigidbody>().isKinematic = true;
                }
            }
            else // If we DO have a target
            {
                PickupController.DropTarget();
                // target.GetComponent<Rigidbody>().isKinematic = false;

                if(target.gameObject.layer == LAYER_ZOOMABLE)
                {
                    Superview.DetachDeform(target);
                    deforming = false;
                }
                if (target.gameObject.layer == LAYER_CUTABLE)
                {   
                    // CutTarget(); // enable this !
                }
                target = null;
            }
        }

    }

    void UpdateOutLine(){
        if (outline != null) {
            // disable outline
            outline.OutlineWidth = 0;
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
                        outline.OutlineWidth = 8;
                    }
                }
            }
        }
    }

    public void UpdateTarget(Transform newTarget)
    {
        Transform oldTarget = target;
        deforming = false;
        if (newTarget != null)
        {
            target = newTarget;
            // target.GetComponent<Rigidbody>().isKinematic = true;
            PickupController.PickupTarget(target.transform.gameObject);
            
        }
        Destroy(oldTarget.gameObject);
    }




}
