using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class endfoor : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("You Win!");
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            // pause the game
            Time.timeScale = 0;
            //remove camera
            Destroy(collision.gameObject);
        }
    }
}
