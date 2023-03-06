using UnityEngine;
using Assets.Scripts; // to access Slicer
using System.Collections.Generic;

public class Edgecut : MonoBehaviour
{
    [Header("DebugVariables")]
    // public Transform target;            // The target object we picked up for scaling
    // public Transform door;
    public Cutter[] cutters;

    [Header("Parameters")]
    public LayerMask cutterLayer;
    // public int cutterLayerInt;
    float originalDistance;             // The original distance between the player playerCamera and the target
    float originalScale;                // The original scale of the target objects prior to being resized
    Vector3 targetScale;                // The scale we want our object to be set to each frame
    
    void Start()
    {
        // collect/remember all cutters
        cutters = FindCutters();

        // print cutterLayer.value
        // print cutters.Length;
        // print((int)cutterLayer.value);
    }

    public void CutTarget(Transform target){
        Cutter cutter = cutters[0];

        // long cut method
        Vector3 pos1 = cutter.A; //cutterN.transform.Find("PivotR").gameObject.transform.position;
        Vector3 pos2 = cutter.B; //cutterN.transform.Find("PivotR").gameObject.transform.position + new Vector3(0, 3, 0);
        Vector3 pos3 = transform.position; //camera location
        Vector3 edge1 = pos2 - pos1;
        Vector3 edge2 = pos3 - pos1;
        Vector3 normal = Vector3.Cross(edge1, edge2).normalized;
        Vector3 transformedNormal = ((Vector3)(target.transform.worldToLocalMatrix * normal)).normalized;
        Vector3 transformedStartingPoint = target.transform.InverseTransformPoint(pos1);
        Plane cutterNRPlane = new Plane();
        cutterNRPlane.SetNormalAndPosition(
            transformedNormal, 
            transformedStartingPoint
        );
        if(transformedNormal.x<0 || transformedNormal.y<0){
            cutterNRPlane = cutterNRPlane.flipped;
        }


        // cutter.UpdateCutterTriangleOnce();
        // print(cutter.isIntersectObject);
        // if(cutter.isIntersectObject){
            
        // }
        // GameObject[] slices = Slicer.Slice(cutterNRPlane, target.gameObject);

        // // if(slices.Length != 2){ // didnt cut
        // //     return;
        // // }

        // Destroy(target.gameObject);
        // // Rigidbody rigidbody = slices[1].GetComponent<Rigidbody>();
        // slices[0].GetComponent<Rigidbody>().isKinematic = true;
        // slices[1].GetComponent<Rigidbody>().isKinematic = true;

        // // resizeTarget
        // // Vector3 nearEdgeDirection = cutterN.transform.Find("PivotR").gameObject.transform.position - cutterN.transform.Find("PivotL").gameObject.transform.position;
        // // Vector3 farEdgeDirection = cutterF.transform.Find("PivotR").gameObject.transform.position - cutterF.transform.Find("PivotL").gameObject.transform.position;
        // // cutterN.transform.localScale += new Vector3(0.1f, 0, 0);
        // originalDistance = 3; // holdDistance
        // originalScale = 1;
        // targetScale = new Vector3(1, 1, 1);
        // // ResizeTarget(slices[0].transform, originalDistance, originalScale, targetScale, 1); // right side
        // // // cutterN.transform.localScale += new Vector3(0.2f, 0, 0);
        // // ResizeTarget(slices[1].transform, originalDistance, originalScale, targetScale, 0); // left side
        // // cutterN.transform.localScale -= new Vector3(0.1f, 0, 0);
    }

    float offsetFactor = 2f;

    public void ResizeTarget(Transform target, float originalDistance, float originalScale, Vector3 targetScale, int hitIndex = 0){
        // RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, cutterLayer);
        // send raycastall from transform.position to transform.forward
        // if (Physics.RaycastAll(ray, out hits, Mathf.Infinity, cutterWalls))
        // {
        // Set the new position of the target by getting the hit point and moving it back a bit
        // depending on the scale and offset factor
        target.position = hits[hitIndex].point - transform.forward * offsetFactor * targetScale.x;

        // Calculate the current distance between the camera and the target object
        float currentDistance = Vector3.Distance(transform.position, target.position);

        // Calculate the ratio between the current distance and the original distance
        float s = currentDistance / originalDistance;

        // Set the scale Vector3 variable to be the ratio of the distances
        targetScale.x = targetScale.y = targetScale.z = s;

        // Set the scale for the target objectm, multiplied by the original scale
        target.localScale = targetScale * originalScale;
        // }
        // if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, cutterWalls))
        // {
        //     // Set the new position of the target by getting the hit point and moving it back a bit
        //     // depending on the scale and offset factor
        //     target.position = hit.point - transform.forward * offsetFactor * targetScale.x;
 
        //     // Calculate the current distance between the camera and the target object
        //     float currentDistance = Vector3.Distance(transform.position, target.position);
 
        //     // Calculate the ratio between the current distance and the original distance
        //     float s = currentDistance / originalDistance;
 
        //     // Set the scale Vector3 variable to be the ratio of the distances
        //     targetScale.x = targetScale.y = targetScale.z = s;
 
        //     // Set the scale for the target objectm, multiplied by the original scale
        //     target.localScale = targetScale * originalScale;
        // }
    }

    public Cutter[] FindCutters()
    {
        Cutter[] goArray = FindObjectsOfType<Cutter>();
        List<Cutter> goList = new List<Cutter>();
        for (int i = 0; i < goArray.Length; i++)
        {
            goList.Add(goArray[i]);
        }
        if (goList.Count == 0)
        {
            return null;
        }
        return goList.ToArray();
    }

}
