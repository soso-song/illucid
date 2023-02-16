using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTrigger : MonoBehaviour
{
    public IllusionManager IllusionManager;
    public GameObject PopcornPrefab;
    private GameObject NewTarget;
    private Vector3 SpawnLocation;

    // Start is called before the first frame update
    void Start()
    {
        IllusionManager = GameObject.FindObjectOfType<IllusionManager>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (IllusionManager.target != null)
        {
             SpawnLocation = IllusionManager.target.gameObject.transform.position;
            
            if (IllusionManager.target.gameObject.tag == "CornSeed")
            {
                SpawnNewTarget(PopcornPrefab);
                IllusionManager.UpdateTarget(NewTarget.transform);
            }
            else if (IllusionManager.target.gameObject.tag == "Popcorn")
            {
                IllusionManager.UpdateTarget(null);
            }
        }
    }

    private void SpawnNewTarget(GameObject target)
    {
        NewTarget = GameObject.Instantiate(target);
        NewTarget.transform.position = SpawnLocation;
    }

}
