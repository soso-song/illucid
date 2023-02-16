using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorControl3 : MonoBehaviour
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

    public void OpenRight()
    {
        Door.Play("Door3Open", 0, 0.0f);
    }

    public void CloseRight(){
        Door.Play("Door3Close", 0, 0.0f);
    }
}
