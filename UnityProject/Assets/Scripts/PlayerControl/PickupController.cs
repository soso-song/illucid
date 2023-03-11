// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] Transform holdArea;
    public GameObject target;
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
        // float distance = Vector3.Distance(target.transform.position, holdArea.position);
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

        if (target.gameObject.layer == 12 && target.transform.localScale.x > 1) // LAYER_CUTABLE
        { // change the scale of the target
            target.transform.localScale = Vector3.Lerp(
                target.transform.localScale, 
                new Vector3(transform.localScale.x - 1, transform.localScale.y - 1, transform.localScale.z - 1), 
                3f * Vector3.Distance(target.transform.position, transform.position) * Time.deltaTime
            );
        }else if(target.gameObject.layer == 12 && target.transform.localScale.x < 1){
            target.transform.localScale = Vector3.one;
        }
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
