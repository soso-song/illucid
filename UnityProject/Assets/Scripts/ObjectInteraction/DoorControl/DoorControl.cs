using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorControl : MonoBehaviour
{
    public Animator Door;
    public Transform DoorLight;

    // Start is called before the first frame update
    void Start()
    {
        Door = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // door.transform.rotation.y += openPersent
    }

    public void Open()
    {
        Door.Play("Door1Open", 0, 0.0f);
        DoorLight.gameObject.SetActive(true);
    }

    public void Close(){
        Door.Play("Door1Close", 0, 0.0f);
        DoorLight.gameObject.SetActive(false);
    }
}

//public DoorController DoorController;
//DoorController = GameObject.FindObjectOfType<DoorController>();
//DoorController.Open();
//DoorController.Close();
