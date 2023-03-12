//
// Created by Soso Song (@sososong) on 3/11/2023
// Copyright (c) 2023 Zhifei(Soso) Song. All rights reserved.
//

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

    public bool debugMode = true;

    [Header("Parameters")]
    // public LayerMask cutterLayer;
    // public int cutterLayerInt;
    // public float maxEdgeDistance = 50f;
    float originalDistance;             // The original distance between the player playerCamera and the target
    // float originalScale;                // The original scale of the target objects prior to being resized
    // Vector3 targetScale;                // The scale we want our object to be set to each frame
    public float slicePosOffsetPerc = 0.8f;
    // public GameObject UpDownChecker;
    void Start()
    {
        // collect/remember all cutters
        cutters = FindCutters();
        // get child object of cuurent object

        originalDistance = Vector3.Distance(transform.position, transform.GetChild(0).position);
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

        Destroy(target.gameObject);

        // disable the collison of slices to avoid collision with player
        slices[0].GetComponent<Rigidbody>().isKinematic = true;
        slices[1].GetComponent<Rigidbody>().isKinematic = true;
        Destroy(slices[0].GetComponent<MeshCollider>());
        Destroy(slices[1].GetComponent<MeshCollider>());
        
        // Add necessary components to slices
        slices[0].AddComponent<TargetController>();
        slices[1].AddComponent<TargetController>();
        slices[0].layer = 12;//cutable layer
        slices[1].layer = 12;//cutable layer

        // scale & position code
        RepositionTarget(slices[0].transform, hits[0].point);
        RepositionTarget(slices[1].transform, hits[1].point);
        
        ResizeTarget(slices[0].transform);
        ResizeTarget(slices[1].transform);

        RecenterMeshPivotPos(slices[0]);
        RecenterMeshPivotPos(slices[1]);

        // its safe to add collider back (since object is far away from player)
        slices[0].AddComponent<MeshCollider>().convex = true;
        slices[1].AddComponent<MeshCollider>().convex = true;
        // slices[0].AddComponent<MeshCollider>();
        // slices[1].AddComponent<MeshCollider>();

        // LSlice = slices[0];
        // RSlice = slices[1];
    }


    GameObject LSlice;
    GameObject RSlice;

    public void EnableLastSlidesCollider(){
        LSlice.AddComponent<MeshCollider>().convex = true;
        RSlice.AddComponent<MeshCollider>().convex = true;
    }

    void RepositionTarget(Transform slice, Vector3 hitPoint) 
    {
        Vector3 targetPos = Vector3.Project(hitPoint - transform.position, transform.forward);
        // make it shorter by offsetFactor
        // Vector3 toTarget = targetPoint - targetPos;
        // get distance from camera to hitPoint
        float distance = Vector3.Distance(transform.position, hitPoint);
        Vector3 shorter = transform.forward * distance * (1f - slicePosOffsetPerc);

        slice.position = transform.position + targetPos - shorter;
        // target.position = hit.point - transform.forward * offsetFactor;

        // return target.position;
        //return camera.transform.position + projection;
    }

    void ResizeTarget(Transform slice){
        Vector3 targetScale = new Vector3(1, 1, 1);

        float currentDistance = Vector3.Distance(transform.position, slice.position);

        // Calculate the ratio between the current distance and the original distance
        float s = currentDistance / originalDistance;

        // Set the scale Vector3 variable to be the ratio of the distances
        targetScale.x = targetScale.y = targetScale.z = s;

        // Set the scale for the target objectm, multiplied by the original scale
        slice.localScale = targetScale * 1; //originalScale;

        // return s;
    }

    void RecenterMeshPivotPos(GameObject slice){
        // get the local(for vertice shift) and world(for overall shift with scale) shift value
        Vector3 worldBoundShift = slice.GetComponent<Renderer>().bounds.center - slice.transform.position;
        Vector3 boundCenter = slice.GetComponent<Renderer>().localBounds.center;

        // reposition the pivot
        Mesh mesh = slice.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
            vertices[i] -= boundCenter;
        mesh.vertices = vertices;

        // update mesh collider
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        // if(target.GetComponent<MeshCollider>() != null){
        //     target.GetComponent<MeshCollider>().sharedMesh = null;
        //     target.GetComponent<MeshCollider>().sharedMesh = mesh;
        // }
        // target.GetComponent<Renderer>().ResetLocalBounds();

        // reposition the object
        slice.transform.position += worldBoundShift;
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
