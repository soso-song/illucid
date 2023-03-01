using UnityEngine;

public class Laser : MonoBehaviour
{
    private LineRenderer laser;
    public bool isTriggered = false;

    private void Start()
    {
        laser = GetComponent<LineRenderer>();
        laser.SetPosition(0, transform.position);
    }
    private void Update()
    {
        castLaser(transform.position, transform.forward);
    }
    private void castLaser(Vector3 position , Vector3 direction)
    {
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
                // check if hit object has name "Ground"
                if (hit2.collider.gameObject.name == "Ground")
                {
                    laser.material.color = Color.green;
                    isTriggered = true;
                } else
                {
                    laser.material.color = Color.red;
                    isTriggered = false;
                }
                laser.SetPosition(2, hit2.point);
            }
        }
    }
   
}