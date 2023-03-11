// using System.Collections;
// using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class LaserRef {
    public Transform laser;
    private Laser laserScript;
    public Laser LaserScript { get => laserScript; set => laserScript = value;}

} 

public class LaserDoor : MonoBehaviour
{
    // public int trigLaserNum = 0;
    public LaserRef[] lasers;
    private float startYpos;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < lasers.Length; i++) {
            if (lasers[i].laser != null) {
                // set the laser script from the object

                lasers[i].LaserScript = lasers[i].laser.GetComponent<Laser>();
            }
        }
        startYpos = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        bool allLasersTriggered = true;
        for (int i = 0; i < lasers.Length; i++) {
            if (lasers[i].laser != null) {
                if (!lasers[i].LaserScript.isTriggered) {
                    allLasersTriggered = false;
                    break;
                }
            }
        }
        if (allLasersTriggered)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y - 2, transform.position.z), 2 * Time.deltaTime);
        }else if (transform.position.y < startYpos)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), 2 * Time.deltaTime);
        }
    }
}
