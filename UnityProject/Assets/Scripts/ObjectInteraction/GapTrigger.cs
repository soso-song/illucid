using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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
    GameObject gameFlowControlObj;
    GameFlowControl gameFlowControl;

    void Start()
    {
        gameFlowControlObj = GameObject.Find("GameManager");
        gameFlowControl = gameFlowControlObj.GetComponent<GameFlowControl>();
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // Debug.Log("Time: " + Time.time + ", Level: " + SceneManager.GetActiveScene().buildIndex + ", Room: " + gameFlowControl.curRoom + ", Player Fall");
            foreach (Spawner spawner in spawners)
            {
                SpawnGameObject(spawner);
            }
        } else {
            // Debug.Log("Time: " + Time.time + ", Level: " + SceneManager.GetActiveScene().buildIndex + ", Room: " + gameFlowControl.curRoom + ", " + other.gameObject.name + " Fall");
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
