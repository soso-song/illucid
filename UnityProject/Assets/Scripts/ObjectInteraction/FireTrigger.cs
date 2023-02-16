using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTrigger : MonoBehaviour
{
    public IllusionManager IllusionManager;
    private GameObject NewTarget;
    private Vector3 spawnLocation;

    // Start is called before the first frame update
    void Start()
    {
        IllusionManager = GameObject.FindObjectOfType<IllusionManager>();
    }

    private void OnTriggerExit(Collider collider)
    {
        Debug.Log(collider.gameObject.tag);
        spawnLocation = collider.gameObject.transform.position;

        if (collider.gameObject.tag == "CornSeed")
        {
            Debug.Log("CornSeed entered the fire");
            SpawnNewTarget("DeformCube");
            IllusionManager.UpdateTarget(NewTarget.transform);
        }
        else if (collider.gameObject.tag == "Popcorn")
        {
            // Debug.Log("Popcorn entered the fire");
            // SpawnNewTarget("DeformSphere");
            // IllusionManager.UpdateTarget(NewTarget.transform);
            Debug.Log("Popcorn entered the fire");
            IllusionManager.UpdateTarget(null);
        }
    }

    private void SpawnNewTarget(string targetName)
    {
        NewTarget = GameObject.Instantiate(Resources.Load(targetName)) as GameObject;
        NewTarget.transform.position = spawnLocation;
    }

}
