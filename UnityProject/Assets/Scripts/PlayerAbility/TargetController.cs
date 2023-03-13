using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    private Outline outline;
    public int pickTime = 0;
    public int maxPickTime = 3;

    [Header("SuperviewParameters")]
    public float LatticeSensitivity=0.005f;
    public float LatticeSensitivityFront=2;
    public float LatticeSensitivityDepth=4;
    public float LatticeSensitivityBack=4;
    // public float currScaleFront=1;
    // public float currScaleBack=1;
    // public float currScaleDepth=1;
    // public float maxScaleFront=2;
    // public float maxScaleBack=4;
    // public float maxScaleDepth=9;
    public float currObjFOV=0;
    public float minObjFOV=-12;
    public float maxObjFOV=80;

    [Header("EdgeCutParameters")]
    
    public bool isStatic = true;
    void Start()
    {
        // add script Outline to the object
        outline = gameObject.AddComponent<Outline>();
        // disable the outline
        // outline.enabled = false;
        outline.OutlineWidth = 0;

        // // check if the object has a mesh collider
        // if (gameObject.GetComponent<MeshCollider>() == null)
        // {
        //     gameObject.AddComponent<MeshCollider>();
        // }
    }

    public void IncrementDrop()
    {
        pickTime += 1;
        // Debug.Log("Object: " + gameObject.name + " has been picked " + pickTime + " times, cur FOV:" + currObjFOV);
        
        // switch (pickTime)
        // {
        //     case 1:
        //         outline.OutlineColor = Color.green;
        //         break;
        //     case 2:
        //         outline.OutlineColor = Color.blue;
        //         break;
        //     case 3:
        //         outline.OutlineColor = Color.grey;
        //         break;
        // }
    }

    public IEnumerator WaitDrop(float seconds){
        // wait one frame for start() to finish
        gameObject.layer = 0; // unable to pickup
        yield return null;
        outline.OutlineColor = Color.yellow;
        outline.OutlineWidth = 8;


        // seconds latter, drop the object
        yield return new WaitForSeconds(seconds);
        GetComponent<Rigidbody>().isKinematic = false; // enable physics
        GetComponent<Rigidbody>().useGravity = true;
        outline.OutlineColor = Color.white;
        outline.OutlineWidth = 0;
        gameObject.layer = 12; // able to pickup
    }

    public IEnumerator EnableColliderAfterFrame(int frameCount = 1){
        // weird bug, after change pos then resize then add collider, player will be pushed by big slides.
        // so wait one frame to really make sure position changed before add collider
        for (int i = 0; i < frameCount; i++)
            yield return null;
        // wait for frames
        gameObject.AddComponent<MeshCollider>().convex = true;
    }

    public IEnumerator ScaleDestroyOverTime(float duration){
        float time = 0.0f;
        Vector3 startScale = transform.localScale;

        while (time < duration) {
            float t = time / duration;
            Vector3 newScale = Vector3.Lerp(startScale, Vector3.zero, t);
            transform.localScale = newScale;
            time += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
