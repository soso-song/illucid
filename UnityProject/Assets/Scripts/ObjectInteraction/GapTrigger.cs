using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Spawner
{
    public GameObject obj;
    public GameObject prefab;
    public Transform spawnPoint;
}

public class GapTrigger : MonoBehaviour
{
    public Spawner[] spawners;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            foreach (Spawner spawner in spawners)
            {
                SpawnGameObject(spawner);
            }
        } else {
            bool isSpawner = false;
            // check if the object has same name as the prefab in spawners
            foreach (Spawner spawner in spawners)
            {
                if (spawner.obj == other.gameObject)
                {
                    SpawnGameObject(spawner);
                    isSpawner = true;
                }
            }
            if (!isSpawner)
            {
                Destroy(other.gameObject);
            }

        }
    }

    private void SpawnGameObject(Spawner spawner)
    {
        if (spawner.prefab == null || spawner.spawnPoint == null || spawner.obj == null)
        {
            Debug.LogWarning("Missing prefab or spawn point or object in spawner");
            return;
        }
        GameObject tmp = spawner.obj;
        GameObject newObj = Instantiate(spawner.prefab, spawner.spawnPoint.position, spawner.spawnPoint.rotation);
        newObj.name = spawner.obj.name;
        spawner.obj = newObj;
        Destroy(tmp);
    }
}
