using UnityEngine;

public class CutterEdge : MonoBehaviour
{
    Camera cam;
    
    public Vector3 A,B;

    Mesh mesh;
    MeshFilter mf;
    
    // Start is called before the first frame update
    void Start()
    {
        // mesh = GetComponent<MeshFilter>().mesh; // get the plane mesh
        // Vector3[] vertices = mesh.vertices; // get the vertices of the plane mesh

        // // Remove the second triangle by adjusting the vertices
        // vertices[2] = vertices[3];
        // vertices[3] = vertices[0];
        // vertices[0] = vertices[1];

        // // Update the mesh with the new vertices
        // mesh.vertices = vertices;
        // mesh.triangles =  new int[] {0, 1, 2};

        // // Recalculate the normals and bounds of the mesh
        // mesh.RecalculateNormals();
        // mesh.RecalculateBounds();


        cam = Camera.main;
        A = transform.GetChild(0).transform.position - transform.position;
        B = transform.GetChild(1).transform.position - transform.position;

        mesh = new Mesh();
        mesh.vertices = new Vector3[] {A, B, cam.transform.position - transform.position};
        mesh.triangles =  new int[] {0, 1, 2};

        // gameObject.AddComponent<MeshFilter>();
        // GetComponent<MeshFilter>().mesh = mesh;

        // gameObject.AddComponent<MeshCollider>();
        // gameObject.GetComponent<MeshCollider>().isTrigger = true;
    }

    // Update is called once per frame
    // float camX, camY;
    void Update()
    {
        // gameObject.GetComponent<MeshCollider>().sharedMesh.vertices[0] = cam.transform.position;
        
        mesh.vertices = new Vector3[] {A, B, cam.transform.position - transform.position};
        mesh.triangles =  new int[] {0, 1, 2};
        // mesh.RecalculateBounds();
        // mesh.RecalculateNormals();
        gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;

        // mesh.vertices[2] = cam.transform.position;
        // mesh.RecalculateBounds();
        // mesh.RecalculateNormals();
        
        // gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;
        // let cutterL and cutterR facing sideway of the camera
        // camX = cam.transform.position.x;
        // camY = cam.transform.position.y;
        
        // pivotL.transform.LookAt(cam.transform.position);
        // pivotR.transform.LookAt(cam.transform.position);
        // // // // let cutterL and cutterR always face the GameObject
        // pivotL.transform.Rotate(0, 90, 0);
        // pivotR.transform.Rotate(0, 90, 180);
        // // let cutterL and cutterR's width is the same as the distance from the GameObject to the camera
        // pivotL.transform.localScale = new Vector3(Vector3.Distance(cam.transform.position, pivotL.transform.position)/10, 1, 1);
        // pivotR.transform.localScale = new Vector3(Vector3.Distance(cam.transform.position, pivotR.transform.position)/10, 1, 1);
        // let cutterL rotate around its center
        // let cutterL only between its center and the position of the cam
        // if (Vector3.Distance(cutterL.transform.position, transform.position) > Vector3.Distance(cam.transform.position, transform.position))
        // {
        //     cutterL.transform.RotateAround(transform.position, Vector3.up, -1);
        // }
    }
}
