using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Room : MonoBehaviour
{
    public GameObject GameManager;
    GameFlowControl gameFlowControl;
    public string roomName = string.Empty;

    void Start()
    {
        if (GameManager == null)
        {
            GameManager = GameObject.Find("GameManager");
        }
        gameFlowControl = GameManager.GetComponent<GameFlowControl>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            gameFlowControl.curRoom = roomName;
            // Debug.Log("Game Time: "+Time.time + ", Level: "+SceneManager.GetActiveScene().buildIndex + ", Room: "+gameFlowControl.curRoom);
        }
    }
}
