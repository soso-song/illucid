using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class endfoor : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Time: " + Time.time + ", Level: " + SceneManager.GetActiveScene().buildIndex + ", Player wins");
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            // pause the game
            Time.timeScale = 0;
            //remove camera
            Destroy(collision.gameObject);
        }
    }
}
