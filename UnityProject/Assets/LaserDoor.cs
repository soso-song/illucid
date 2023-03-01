// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public class LaserDoor : MonoBehaviour
{
    // public int trigLaserNum = 0;
    public Transform laser1;
    public Transform laser2;
    public Transform laser3;
    public Transform laser4;
    private Laser laser1Script;
    private Laser laser2Script;
    private Laser laser3Script;
    private Laser laser4Script;
    private float startYpos;
    // Start is called before the first frame update
    void Start()
    {
        // get the laser script from the object   
        laser1Script = laser1.GetComponent<Laser>();
        laser2Script = laser2.GetComponent<Laser>();
        laser3Script = laser3.GetComponent<Laser>();
        laser4Script = laser4.GetComponent<Laser>();
        startYpos = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (laser1Script.isTriggered && laser2Script.isTriggered && laser3Script.isTriggered && laser4Script.isTriggered)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y - 2, transform.position.z), 2 * Time.deltaTime);
        }else if (transform.position.y < startYpos)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), 2 * Time.deltaTime);
        }
    }
}
