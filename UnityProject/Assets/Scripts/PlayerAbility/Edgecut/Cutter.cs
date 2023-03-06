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

    void Start()
    {
        // use the inputs
        cam = Camera.main;
        edgecut = cam.GetComponent<Edgecut>();
        A = transform.GetChild(0).transform.position;
        B = transform.GetChild(1).transform.position;
        C = cam.transform.position;

        // create the mesh
       
        // mesh.vertices = new Vector3[] {A, B, C, C+C};
        // mesh.triangles =  new int[] {0, 1, 2, 2, 3, 0};

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
        LeftPoint.transform.position = new Vector3(LeftRightPointOffset,0,0);
        RightPoint.transform.position = new Vector3(-LeftRightPointOffset,0,0);
        LeftPoint.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
        RightPoint.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
        TargetPoint.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
        LeftPoint.transform.parent = TargetPoint.transform;
        RightPoint.transform.parent = TargetPoint.transform;
        TargetPoint.transform.parent = transform;

        // set the position of TargetPoints
        // TargetPoint.transform.position = (A + B) / 2; // will use cam.transform.forward
        // mesh = new Mesh();
        // CutterCollider = gameObject.GetComponent<MeshCollider>();

        // mesh.vertices = new Vector3[] {A, B, C, C + new Vector3(0,15,0)};
        // mesh.triangles =  new int[] {0, 3, 1, 3, 0, 2};
        // CutterCollider.sharedMesh = mesh;
        
    }

    void Update()
    {
        C = cam.transform.position;

        // UpdateCutterTriangleOnce(); // check left right intersection

        UpdateTargetPoint(); // check up down intersection

        if(isLeftRightIntersectObject && isUpDownIntersectObject){
            isIntersectObject = true;
        } else {
            isIntersectObject = false;
        }

        CheckCutReady(); // check object behind the cutter

        // CheckObjectIntersection();


        // if (isIntersectObjectStay){
        //     isIntersectObject=true;
        //     isIntersectObjectStay=false;
        // } else {
        //     isIntersectObject=false;
        // }
        
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
        
        // mesh.RecalculateBounds();
        // mesh.RecalculateNormals();
        // MeshCollider meshCollider = GetComponent<MeshCollider>();
        // meshCollider.


        // Vector3[] vertices = mesh.vertices;
        // vertices[2] = C;
        // // mesh.vertices = new Vector3[] {A, B, C, C + new Vector3(0,15,0)};
        // // mesh.triangles =  new int[] {0, 3, 1, 3, 0, 2};
        // CutterCollider.sharedMesh.vertices = vertices;
        // // mesh = CutterCollider.sharedMesh;
        // // could only update when release the object
        // CutterCollider.sharedMesh.RecalculateBounds();

        
        // // Vector3[] normals = mesh.normals;

        // // for (var i = 0; i < vertices.Length; i++)
        // // {
        // //     vertices[i] += normals[i] * Mathf.Sin(Time.time);
        // // }

        // // mesh.vertices = vertices;
        // // CutterCollider.sharedMesh = mesh;
    }
    void UpdateTargetPoint()
    {
        Vector3 intersection = GetLinePlaneIntersection(A, B, edgecut.UpDownChecker);
        TargetPoint.transform.position = intersection;
        // TargetPoint.transform.position = GetComponent<MeshCollider>().ClosestPointOnBounds(transform.position);
        TargetPoint.transform.LookAt(C);

        // check if the y position of TargetPoint is between A and B
        if (intersection.y > A.y || intersection.y < B.y)
        {
            isUpDownIntersectObject = false;
        }
        isUpDownIntersectObject = true;
        // Vector3 dir = cam.transform.forward;
    
        // // Calculate the line vector and distance between the line and the ray
        // Vector3 AB = B - A;
        // Vector3 cross = Vector3.Cross(AB, dir);
        // float distance = Vector3.Magnitude(cross) / Vector3.Magnitude(dir);

        // // Calculate the intersection point
        // Vector3 CA = A - C;
        // Vector3 projection = Vector3.Dot(CA, AB) / Vector3.Dot(AB, AB) * AB;
        // Vector3 closestPoint = A + projection;
    }
    public Vector3 GetLinePlaneIntersection(Vector3 A, Vector3 B, GameObject planeObject)
    {
        // Get the plane's normal and distance
        // Plane plane = new Plane(planeObject.transform.up, planeObject.transform.position);
        // Vector3 AB = B - A;
        // float t = - (plane.normal.x * A.x + plane.normal.y * A.y + plane.normal.z * A.z + plane.distance)
        //     / (plane.normal.x * AB.x + plane.normal.y * AB.y + plane.normal.z * AB.z);
        // Vector3 P = A + t * AB;
        // return P;
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
    // void OnTriggerStay(Collider collision)
    // {      
    //     if (collision.gameObject.transform.parent.name == "HoldArea")
    //     {
    //         // Debug.Log("Collider is colliding with MyOtherObject");
    //         isIntersectObject = true;
    //     }
    // }
}
