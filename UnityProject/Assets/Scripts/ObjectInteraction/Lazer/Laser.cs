using UnityEngine;

public class Laser : MonoBehaviour
{
    private int targetMask = 1 << 12 | 1 << 13;
    // private int _maxBounce = 20;
    // public Transform startPoint;
    // private int _count ;
    private LineRenderer laser;

    // [SerializeField]
    // private Vector3 _offSet;

    private void Start()
    {
        laser = GetComponent<LineRenderer>();
        
    }
    private void Update()
    {
        castLaser(transform.position, transform.forward);
    }
    private void castLaser(Vector3 position , Vector3 direction)
    {
        laser.SetPosition(0, position);
       
        Ray ray = new Ray(position, direction);
        RaycastHit hit;
        
        if(Physics.Raycast(ray , out hit , 300))
        {
            laser.SetPosition(1, hit.point);
            Ray ray2 = new Ray(hit.point, Vector3.Reflect(direction, hit.normal));
            RaycastHit hit2;
            laser.SetPosition(2, hit.point);
            if(Physics.Raycast(ray2 , out hit2 , 300))
            {
                laser.SetPosition(2, hit2.point);
            }
        }
    }
   
}