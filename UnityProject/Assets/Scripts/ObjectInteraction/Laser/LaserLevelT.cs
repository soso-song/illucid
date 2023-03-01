using UnityEngine;

public class LaserLevelT : MonoBehaviour
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
            if (hit.collider.gameObject.name == "DeformTriggerBall")
            {
                laser.material.color = Color.green;
                isTriggered = true;
            } else
            {
                laser.material.color = Color.red;
                isTriggered = false;
            }
            laser.SetPosition(1, hit.point);
        }
    }
   
}