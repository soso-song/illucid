using UnityEngine;

public class Cutter : MonoBehaviour
{
    [Header("DebugVariables")]
    public float leftDistance;
    public float midDistance;
    public float rightDistance;
    public bool isCutReady = false;
    public bool isIntersectObject = false;
    public bool isUpDownIntersectObject = false;
    public bool isLeftRightIntersectObject = false;
    // public bool isIntersectObjectStay = false;
    public float slope = 0.5f;

    [Header("Parameters")]
    public Vector3 A;
    public Vector3 B;
    public Vector3 C;
    public float LeftRightPointOffset = 0.2f;
    public float MinDiff = 5f;
    public float MaxDiff = 70f;
    public float DiffTolerance = 2f;
    public LayerMask cutterLayer;

    Camera cam;
    GameObject TargetPoint, LeftPoint, RightPoint;
    Mesh mesh;
    MeshCollider CutterCollider;
    // GameObject UpDownChecker;
    Edgecut edgecut;
    Transform ATransform, BTransform;

    // efficient storage
    // private Vector3 AB;
    GameObject pivot;
    GameObject IntersectPlane;

    void Start()
    {
        // use the inputs
        cam = Camera.main;
        edgecut = cam.GetComponent<Edgecut>();
        ATransform = transform.Find("A").transform;
        // A = transform.GetChild(0).transform.position;
        BTransform = transform.Find("B").transform;

        A = ATransform.position;
        B = BTransform.position;
        C = cam.transform.position;

        // create the TargetPoint, LeftPoint and RightPoint
        // check isUpDownIntersectObject, isCutReady
        TargetPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        LeftPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        RightPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        TargetPoint.GetComponent<Collider>().enabled = false;
        LeftPoint.GetComponent<Collider>().enabled = false;
        RightPoint.GetComponent<Collider>().enabled = false;
        TargetPoint.name = "TargetPoint";
        LeftPoint.name = "LeftPoint";
        RightPoint.name = "RightPoint";
        LeftPoint.transform.position = new Vector3(LeftRightPointOffset,0,0);
        RightPoint.transform.position = new Vector3(-LeftRightPointOffset,0,0);
        LeftPoint.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
        RightPoint.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
        TargetPoint.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
        LeftPoint.transform.parent = TargetPoint.transform;
        RightPoint.transform.parent = TargetPoint.transform;
        TargetPoint.transform.parent = transform;

        // check isLeftRightIntersectObject
        pivot = new GameObject("Pivot");
        pivot.transform.parent = transform;
        IntersectPlane = GameObject.CreatePrimitive(PrimitiveType.Quad);
        // IntersectPlane.GetComponent<Renderer>().enabled = false;
        Destroy(IntersectPlane.GetComponent<MeshCollider>());
        IntersectPlane.AddComponent<BoxCollider>();
        IntersectPlane.GetComponent<BoxCollider>().isTrigger = true;

        IntersectPlane.transform.parent = pivot.transform;
        float height = Vector3.Distance(A, B);
        pivot.transform.position = transform.position;
        IntersectPlane.transform.localPosition = new Vector3(-0.5f, 0, 0);
        IntersectPlane.transform.localScale = new Vector3(1, height, 0.01f); // 0.01 is the z scale for collider
        // attach the IntersectPlane script to the IntersectPlane
        // change the layer to "MathObj"
        IntersectPlane.layer = 20;
        IntersectPlane.AddComponent<IntersectPlane>();
        // IntersectPlane.transform.Rotate(90, 0, 0);
        
        // efficient storage
        // AB = B - A;
    }

    void Update()
    {
        A = ATransform.position;
        B = BTransform.position;
        C = cam.transform.position;

        // check left right intersection (supports rotation)
        UpdateCutterTriangleOnce();

        // check up down intersection (supports rotation)
        // update CutReady Point position (supports rotation)
        UpdateTargetPoint(); 

        if(isLeftRightIntersectObject && isUpDownIntersectObject){
            isIntersectObject = true;
        } else {
            isIntersectObject = false;
        }

        // check object behind the cutter
        CheckCutReady(); 
    }

    public void UpdateCutterTriangleOnce()
    {
        pivot.transform.LookAt(cam.transform.position, A-B);
        pivot.transform.Rotate(0, 90, 0);
        pivot.transform.localScale = new Vector3(Vector3.Distance(cam.transform.position, pivot.transform.position), 1, 1);
    }
    void UpdateTargetPoint()
    {
        Vector3 intersection = GetLinePlaneIntersection(A, B);
        TargetPoint.transform.position = intersection;
        TargetPoint.transform.LookAt(C, A-B); // A-B is up direction

        // check if the y position of TargetPoint is between A and B
        isUpDownIntersectObject = IsIntersectionBetweenAB(intersection);
    }


    public Vector3 GetLinePlaneIntersection(Vector3 A, Vector3 B)
    {
        Vector3 n = cam.transform.up;
        // Vector3 n = Vector3.Cross(B-A, cam.transform.up).normalized;
        // Vector3 pos = cam.transform.position;
        Vector3 d = B - A;
        float t = Vector3.Dot(n, C - A) / Vector3.Dot(n, d);

        if (float.IsNaN(t) || float.IsInfinity(t))
        {
            // The line AB is parallel to the plane, return a default value.
            return Vector3.zero;
        }

        Vector3 P = A + t * d;

        return P; 
    }
    public bool IsIntersectionBetweenAB(Vector3 intersection)
    {
        Vector3 AB = B - A;
        Vector3 AC = intersection - A;
        Vector3 BC = intersection - B;

        float dotABAC = Vector3.Dot(AB, AC);
        float dotABBC = Vector3.Dot(AB, BC);

        if (dotABAC >= 0f && dotABBC <= 0f)
        {
            // C is between A and B.
            return true;
        }
        else
        {
            // C is not between A and B.
            return false;
        }
    }

    public RaycastHit[] CheckCutReady(){
        // get the distance between Camera and TargetPoint
        midDistance = Vector3.Distance(C, TargetPoint.transform.position);
        RaycastHit hitL, hitR;
        Physics.Raycast(C, LeftPoint.transform.position - C, out hitL, Mathf.Infinity, cutterLayer);
        Physics.Raycast(C, RightPoint.transform.position - C, out hitR, Mathf.Infinity, cutterLayer);
        leftDistance = Vector3.Distance(C, hitL.point);
        rightDistance = Vector3.Distance(C, hitR.point);
        // show the raycast
        Debug.DrawRay(C, LeftPoint.transform.position - C, Color.red);
        Debug.DrawRay(C, RightPoint.transform.position - C, Color.red);

        isCutReady = false;

        // distance is infinity
        if (leftDistance == 0 || rightDistance == 0)
            return null;
        // difference is too small
        if (Mathf.Abs(leftDistance - rightDistance) < MinDiff)
            return null;
        if (Mathf.Abs(leftDistance - rightDistance) > MaxDiff)
            return null;
        // both side is not close to the targetPoint
        if(Mathf.Abs(leftDistance - midDistance) > DiffTolerance && Mathf.Abs(rightDistance - midDistance) > DiffTolerance)
            return null;
        
        isCutReady = true;

        return new RaycastHit[] {
            hitL,
            hitR
        };
    }

    void OnTriggerEnter(Collider collision)
    {      
        // if (collision.gameObject.transform.parent.name == "HoldArea")
        // {
            // Debug.Log("Collider is colliding with MyOtherObject");
            isIntersectObject = true;
            // get the intersection point
            // Vector3 intersectionPoint = collision.ClosestPointOnBounds(transform.position);
            // TargetPoint.transform.position = intersectionPoint;
        // }
    }
    void OnTriggerExit(Collider collision)
    {      
        // if (collision.gameObject.transform.parent.name == "HoldArea")
        // {
            // Debug.Log("Collider is colliding with MyOtherObject");
            isIntersectObject = false;
        // }
    }
}
