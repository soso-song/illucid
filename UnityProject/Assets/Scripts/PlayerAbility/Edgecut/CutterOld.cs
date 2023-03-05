using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutterOld : MonoBehaviour
{
    public Camera cam;
    GameObject cutterL;
    GameObject cutterR;
    GameObject pivotL;
    GameObject pivotR;
    float width;
    
    // Start is called before the first frame update
    void Start()
    {
        pivotL = new GameObject("PivotL");
        pivotR = new GameObject("PivotR");
        cutterL = GameObject.CreatePrimitive(PrimitiveType.Quad);
        cutterR = GameObject.CreatePrimitive(PrimitiveType.Quad);
        cutterL.GetComponent<Collider>().enabled = false;
        cutterR.GetComponent<Collider>().enabled = false;
        // not render the cutter
        cutterL.GetComponent<Renderer>().enabled = false;
        cutterR.GetComponent<Renderer>().enabled = false;

        pivotL.transform.parent = transform;
        pivotR.transform.parent = transform;

        cutterL.transform.parent = pivotL.transform;
        cutterR.transform.parent = pivotR.transform;


        width = GetComponent<Renderer>().bounds.size.x;
        pivotL.transform.position = new Vector3(transform.position.x - width/2, transform.position.y, transform.position.z);
        pivotR.transform.position = new Vector3(transform.position.x + width/2, transform.position.y, transform.position.z);
        cutterL.transform.position = new Vector3(transform.position.x - width, transform.position.y, transform.position.z);
        cutterR.transform.position = new Vector3(transform.position.x + width, transform.position.y, transform.position.z);

        cutterL.transform.Rotate(90, 0, 0);
        cutterR.transform.Rotate(90, 0, 0);

        // add 0.1f to the width of gameObject
        transform.localScale += new Vector3(0.01f, 0, 0);
        // increase the width of mesh collider
    }

    // Update is called once per frame
    // float camX, camY;
    void Update()
    {
        // let cutterL and cutterR facing sideway of the camera
        // camX = cam.transform.position.x;
        // camY = cam.transform.position.y;
        
        pivotL.transform.LookAt(cam.transform.position);
        pivotR.transform.LookAt(cam.transform.position);
        // // // let cutterL and cutterR always face the GameObject
        pivotL.transform.Rotate(0, 90, 0);
        pivotR.transform.Rotate(0, 90, 180);
        // let cutterL and cutterR's width is the same as the distance from the GameObject to the camera
        pivotL.transform.localScale = new Vector3(Vector3.Distance(cam.transform.position, pivotL.transform.position)/10, 1, 1);
        pivotR.transform.localScale = new Vector3(Vector3.Distance(cam.transform.position, pivotR.transform.position)/10, 1, 1);
        // let cutterL rotate around its center
        // let cutterL only between its center and the position of the cam
        // if (Vector3.Distance(cutterL.transform.position, transform.position) > Vector3.Distance(cam.transform.position, transform.position))
        // {
        //     cutterL.transform.RotateAround(transform.position, Vector3.up, -1);
        // }
    }
}
