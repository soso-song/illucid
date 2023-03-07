using UnityEngine;

public class IntersectPlane : MonoBehaviour
{
    Cutter Cutter;
    void Start()
    {
        // access parent object Pivot -> Parent
        Cutter = transform.parent.parent.gameObject.GetComponent<Cutter>();
    }

    void OnTriggerEnter(Collider collision)
    {      
        // Debug.Log("--enter-- is colliding with MyOtherObject");
        // Debug.Log(collision.gameObject.transform.parent);

        if (collision.gameObject.transform.parent == null)
            return;
        if (collision.gameObject.transform.parent.name == "HoldArea")
        {
            // Debug.Log("Collider is colliding with MyOtherObject");
            Cutter.isLeftRightIntersectObject = true;
        }
    }
    void OnTriggerStay(Collider collision)
    {      
        // Debug.Log("--stay-- is colliding with MyOtherObject");
        // check if the gameobject has a parent
        if (collision.gameObject.transform.parent == null)
            return;
        if (collision.gameObject.transform.parent.name == "HoldArea")
        {
            // Debug.Log("Collider is colliding with MyOtherObject");
            Cutter.isLeftRightIntersectObject = true;
        }
    }
    void OnTriggerExit(Collider collision)
    {    
        // Debug.Log("--no-- is colliding with MyOtherObject");
        // check if the gameobject has a parent
        if (collision.gameObject.transform.parent == null)
            return;
        if (collision.gameObject.transform.parent.name == "HoldArea")
        {
            // Debug.Log("Collider is colliding with MyOtherObject");
            Cutter.isLeftRightIntersectObject = false;
        }
    }
}
