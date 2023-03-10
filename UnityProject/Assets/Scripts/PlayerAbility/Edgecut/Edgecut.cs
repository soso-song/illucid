using UnityEngine;
using Assets.Scripts; // to access Slicer
using System.Collections.Generic;
using System.Collections;

public class Edgecut : MonoBehaviour
{
    [Header("DebugVariables")]
    // public Transform target;            // The target object we picked up for scaling
    // public Transform door;
    public Cutter[] cutters;

    [Header("Parameters")]
    // public LayerMask cutterLayer;
    // public int cutterLayerInt;
    // public float maxEdgeDistance = 50f;
    float originalDistance;             // The original distance between the player playerCamera and the target
    float originalScale;                // The original scale of the target objects prior to being resized
    Vector3 targetScale;                // The scale we want our object to be set to each frame
    
    // public GameObject UpDownChecker;
    void Start()
    {
        // collect/remember all cutters
        cutters = FindCutters();
    }

    Cutter findBestCutter(){
        // create a list of cutters
        Cutter bestCutter = null;
        foreach(Cutter cutter in cutters){
            if(cutter.isIntersectObject && cutter.isCutReady){
                if(bestCutter == null)
                    bestCutter = cutter;
                else if(cutter.distance < bestCutter.distance)
                    bestCutter = cutter;
            }
        }
        return bestCutter;
    }

    public void CutTarget(Transform target){
        Cutter cutter = findBestCutter();
        if(cutter == null)
            return;
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
        // if(transformedNormal.x<0 || transformedNormal.y<0){
        //     cutterNRPlane = cutterNRPlane.flipped;
        // }

    
        // check target is intersect with cutter
        // cutter.UpdateCutterTriangleOnce();
        // StartCoroutine(PauseOneFrame()); // wait one frame for cutter to run ontrigger functions since they reset collider.sharedMesh makes them weird
        // Debug.Log("-------------------");
        // Debug.Log("isIntersectObject:" + cutter.isIntersectObject);
        if(!cutter.isIntersectObject){
            return;
        }

        // check two sides of cutter
        RaycastHit[] hits = cutter.CheckCutReady();
        // Debug.Log("isCutReady:" + cutter.isCutReady);
        if(!cutter.isCutReady || hits ==  null || hits.Length != 2){
            return;
        }
    
        // cut the target
        GameObject[] slices = Slicer.Slice(cutterNRPlane, target.gameObject);
        if(slices.Length != 2){ // didnt cut, error happens
            // error message
            Debug.Log("error: didnt cut to two pieces");
            return;
        }

        Debug.Log("cut success");

        Destroy(target.gameObject);
        // Rigidbody rigidbody = slices[1].GetComponent<Rigidbody>();
        slices[0].GetComponent<Rigidbody>().isKinematic = true;
        slices[1].GetComponent<Rigidbody>().isKinematic = true;
        // make slices to cutable layer
        slices[0].layer = 12;
        slices[1].layer = 12;
        // add targetController to slices
        slices[0].AddComponent<TargetController>();
        slices[1].AddComponent<TargetController>();

        // resizeTarget
        // Vector3 nearEdgeDirection = cutterN.transform.Find("PivotR").gameObject.transform.position - cutterN.transform.Find("PivotL").gameObject.transform.position;
        // Vector3 farEdgeDirection = cutterF.transform.Find("PivotR").gameObject.transform.position - cutterF.transform.Find("PivotL").gameObject.transform.position;
        // cutterN.transform.localScale += new Vector3(0.1f, 0, 0);
        originalDistance = 3; // holdDistance
        originalScale = 1;
        targetScale = new Vector3(1, 1, 1);

        // if(!cutter.isVertical){
        //     ResizeTarget(slices[1].transform, hits[1], originalDistance, originalScale, targetScale); // right side
        // // // cutterN.transform.localScale += new Vector3(0.2f, 0, 0);
        //     ResizeTarget(slices[0].transform, hits[0], originalDistance, originalScale, targetScale); // left side
        // }else{
        // cutter.GetLeftRightCutter();
        ResizeTarget(slices[1].transform, hits[1], originalDistance, originalScale, targetScale); // right side
        // // cutterN.transform.localScale += new Vector3(0.2f, 0, 0);
        ResizeTarget(slices[0].transform, hits[0], originalDistance, originalScale, targetScale); // left side
        // }
       
        // cutterN.transform.localScale -= new Vector3(0.1f, 0, 0);
    }

    float offsetFactor = 2f;

    public void ResizeTarget(Transform target, RaycastHit hit, float originalDistance, float originalScale, Vector3 targetScale){
        // RaycastHit hit;
        // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, cutterLayer);
        // send raycastall from transform.position to transform.forward
        // if (Physics.RaycastAll(ray, out hits, Mathf.Infinity, cutterWalls))
        // {
        // Set the new position of the target by getting the hit point and moving it back a bit
        // depending on the scale and offset factor
        target.position = hit.point - transform.forward * offsetFactor * targetScale.x;

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

    // IEnumerator PauseOneFrame()
    // {
    //     // Pause for one frame
    //     yield return null;

    //     // Resume execution after one frame
    //     Debug.Log("Resumed after one frame");
    // }

}
