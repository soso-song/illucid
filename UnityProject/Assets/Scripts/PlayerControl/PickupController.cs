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
    [SerializeField] private float holdAreaForce = 150f;
    // pickupDrag 10f = funny spring effect and shaking when against object, 30f = not much movement
    [SerializeField] private float pickupDrag = 30f; 

    void LateUpdate(){
        if (targetRb != null)
        {
            MoveTarget();
        }
    }

    public Transform RaycastFromCamera(int targetMask)
    {
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
            targetRb.isKinematic = false; // for poster

            targetRb.useGravity = false;
            targetRb.drag = pickupDrag;
            
            // fix the target shacking after deform
            targetRb.constraints = RigidbodyConstraints.FreezeRotation;
            // targetRb.constraints = RigidbodyConstraints.FreezeRotationY;

            targetRb.transform.SetParent(holdArea);
        }
    }
    public void MoveTarget(){
        if(Vector3.Distance(target.transform.position, holdArea.position) > 0.01f){
            targetRb.AddForce(
                (holdArea.position - target.transform.position) * 
                holdAreaForce  //* targetRb.mass
            );
        }
        // else{
        //     targetRb.velocity = Vector3.zero;
        //     targetRb.transform.position = holdArea.position;
        // }
        // targetRb.transform.LookAt(transform);
        
        // let x rotation be free
        targetRb.transform.rotation = Quaternion.Euler(0, holdArea.rotation.eulerAngles.y, 0);
    }

    public void DropTarget(){
        // this trick removes the cummelative drag force when releasing the object
        targetRb.isKinematic = true;
        targetRb.isKinematic = false;

        targetRb.useGravity = true;
        targetRb.drag = 1;
        // targetRb.constraints = RigidbodyConstraints.FreezeRotationY;
        targetRb.constraints = RigidbodyConstraints.None;
        targetRb.transform.SetParent(null);
        
        target.GetComponent<TargetController>().IncrementDrop();
        targetRb = null;
        target = null;
    }
}
