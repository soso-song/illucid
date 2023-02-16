using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorControl2 : MonoBehaviour
{
    public Animator Door;

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

    public void OpenLeft()
    {
        Door.Play("Door2Open", 0, 0.0f);
    }

    public void CloseLeft(){
        Door.Play("Door2Close", 0, 0.0f);
    }

}
