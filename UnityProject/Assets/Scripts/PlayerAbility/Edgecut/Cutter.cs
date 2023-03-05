using UnityEngine;

public class Cutter : MonoBehaviour
{
    [Header("DebugVariables")]
    public float leftDistance;
    public float midDistance;
    public float rightDistance;
    public bool CutReady = false;

    [Header("Parameters")]
    public Vector3 A;
    public Vector3 B;
    public Vector3 C;
    public float LeftRightPointOffset = 2.5f;
    public float MinDistance = 5f;
    public float MaxDistance = 70f;
    public float DistanceTolerance = 0.1f;

    Camera cam;
    GameObject TargetPoint, LeftPoint, RightPoint;
    Mesh mesh;

    void Start()
    {
        // use the inputs
        cam = Camera.main;
        A = transform.GetChild(0).transform.position;
        B = transform.GetChild(1).transform.position;

        // create the mesh
        mesh = new Mesh();
        mesh.vertices = new Vector3[] {A, B, cam.transform.position};
        mesh.triangles =  new int[] {0, 1, 2};

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
    }

    void Update()
    {
        C = cam.transform.position;

        UpdateCutterTriangle();

        UpdateTargetPoint();

        // get the distance between Camera and TargetPoint
        midDistance = Vector3.Distance(C, TargetPoint.transform.position);
        RaycastHit hitL, hitR;
        Physics.Raycast(C, LeftPoint.transform.position - C, out hitL, Mathf.Infinity);
        Physics.Raycast(C, RightPoint.transform.position - C, out hitR, Mathf.Infinity);
        leftDistance = Vector3.Distance(C, hitL.point);
        rightDistance = Vector3.Distance(C, hitR.point);
        // show the raycast
        Debug.DrawRay(C, LeftPoint.transform.position - C, Color.red);
        Debug.DrawRay(C, RightPoint.transform.position - C, Color.red);

        CutReady = false;

        // distance is infinity
        if (leftDistance == 0 || rightDistance == 0)
            return;
        // difference is too small
        if (Mathf.Abs(leftDistance - rightDistance) < MinDistance)
            return;
        if (Mathf.Abs(leftDistance - rightDistance) > MaxDistance)
            return;
        // both side is not close to the targetPoint
        if(Mathf.Abs(leftDistance - midDistance) > DistanceTolerance && Mathf.Abs(rightDistance - midDistance) > DistanceTolerance)
            return;
        
        CutReady = true;
    }

    void UpdateCutterTriangle()
    {
                // could only update when release the object
        mesh.vertices = new Vector3[] {A, B, C};
        mesh.triangles =  new int[] {0, 1, 2};
        gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;
    }
    void UpdateTargetPoint()
    {

        TargetPoint.transform.position = (A + B) / 2;
        TargetPoint.transform.LookAt(C);
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
}
