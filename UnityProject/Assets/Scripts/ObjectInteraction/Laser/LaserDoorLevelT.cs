// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public class LaserDoorLevelT : MonoBehaviour
{
    // public int trigLaserNum = 0;
    public Transform laser;
    private LaserLevelT laserScript;
    private float startYpos;
    // Start is called before the first frame update
    void Start()
    {
        // get the laser script from the object   
        laserScript = laser.GetComponent<LaserLevelT>();
        startYpos = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (laserScript.isTriggered)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y - 1, transform.position.z), 2 * Time.deltaTime);
        }else if (transform.position.y < startYpos)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y + 3, transform.position.z), 2 * Time.deltaTime);
        }
    }
}
