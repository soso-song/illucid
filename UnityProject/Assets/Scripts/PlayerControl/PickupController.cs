// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] Transform holdArea;
    private GameObject target;
    private Rigidbody targetRb;

    [Header("Physics Parameters")]
    [SerializeField] private float pickupRange = 25f;
    [SerializeField] private float pickupForce = 0.2f;


    void Update(){
        if (targetRb != null)
        {
            MoveTarget();
        }
    }

    public Transform RaycastFromCamera(int targetMask)
    {
        // RaycastHit hit;
        // if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, range, mask))
        // {
        //     return hit.transform;
        // }
        // return null;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, pickupRange, targetMask))
        {
            return hit.transform;
        }
        return null;
    }

    public void PickupTarget(GameObject inputTarget){
        target = inputTarget;
        targetRb = inputTarget.GetComponent<Rigidbody>();
        if (targetRb)
        {
            // targetRb.isKinematic = true;
            targetRb.useGravity = false;
            targetRb.drag = 10f;
            // targetRb.constraints = RigidbodyConstraints.FreezeRotationY;
            targetRb.constraints = RigidbodyConstraints.FreezeRotation;
            targetRb.transform.SetParent(holdArea);
        }
    }

    public void MoveTarget(){
        if(Vector3.Distance(target.transform.position, holdArea.position) > 0.1f){
            targetRb.AddForce((holdArea.position - target.transform.position) * pickupForce);
        }
    }

    public void DropTarget(){
        // targetRb.isKinematic = true;
        targetRb.useGravity = true;
        targetRb.drag = 1;
        // targetRb.constraints = RigidbodyConstraints.FreezeRotationY;
        targetRb.constraints = RigidbodyConstraints.None;
        targetRb.transform.SetParent(null);
        targetRb = null;
        target = null;
    }
}
