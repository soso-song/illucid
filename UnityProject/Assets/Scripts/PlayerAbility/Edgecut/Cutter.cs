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

    // efficient storage
    // private Vector3 AB;

    void Start()
    {
        // use the inputs
        cam = Camera.main;
        edgecut = cam.GetComponent<Edgecut>();
        A = transform.GetChild(0).transform.position;
        B = transform.GetChild(1).transform.position;
        C = cam.transform.position;


        // create the TargetPoint, LeftPoint and RightPoint
        TargetPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        LeftPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        RightPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // disable the collider
        TargetPoint.GetComponent<Collider>().enabled = false;
        LeftPoint.GetComponent<Collider>().enabled = false;
        RightPoint.GetComponent<Collider>().enabled = false;
        TargetPoint.name = "TargetPoint";
        LeftPoint.name = "LeftPoint";
        RightPoint.name = "RightPoint";

        // slope = 0.5f;
        // UpdateLeftRightPoint();
        LeftPoint.transform.position = new Vector3(LeftRightPointOffset,0,0);
        RightPoint.transform.position = new Vector3(-LeftRightPointOffset,0,0);
        LeftPoint.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
        RightPoint.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
        TargetPoint.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
        LeftPoint.transform.parent = TargetPoint.transform;
        RightPoint.transform.parent = TargetPoint.transform;
        TargetPoint.transform.parent = transform;

        // efficient storage
        // AB = B - A;
    }

    Vector2 FindOrthogonalPoint(Vector2 pointA, Vector2 pointB, float d, float slope) {

        // Calculate the slope of line AB
        // float slope = (pointB.y - pointA.y) / (pointB.x - pointA.x);
        
        // Calculate the negative reciprocal of the slope to get the slope of a line perpendicular to AB
        float perpendicularSlope = -1f / slope;
        
        // Calculate the x-coordinate of the point that is d distance away from point A
        float x = (d / Mathf.Sqrt(1f + perpendicularSlope * perpendicularSlope));
        
        // Calculate the y-coordinate of the point using the equation of the line that is perpendicular to AB and passes through point A
        float y = perpendicularSlope * x;
        
        // Return the resulting point as a Vector2
        return new Vector2(x, y);
    }

    void Update()
    {
        A = transform.GetChild(0).transform.position;
        B = transform.GetChild(1).transform.position;
        C = cam.transform.position;

        // check left right intersection
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

    
    // void OnCollisionExit(Collision collision)
    // {
    //     if (collision.gameObject.transform.parent.name == "HoldArea")
    //     {
    //         IntersectObject = false;
    //     }
    // }

    public void UpdateCutterTriangleOnce()
    {
        // 2   3
        //   /
        // 0   1

        // isIntersectObject = false;

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        // Vector3[] normals = mesh.normals;
        vertices[0] = C;
        vertices[1] = B;
        vertices[2] = C + new Vector3(0.1f,0.1f,0.1f);
        vertices[3] = A;
        mesh.vertices = vertices;

        GetComponent<MeshCollider>().sharedMesh = null;
        // isIntersectObject = false;
        GetComponent<MeshCollider>().sharedMesh = mesh;
        // isIntersectObject = false;
        
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
        Vector3 pos = cam.transform.position;
        Vector3 d = B - A;
        float t = Vector3.Dot(n, pos - A) / Vector3.Dot(n, d);

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
