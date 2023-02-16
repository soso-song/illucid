using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenControl : MonoBehaviour
{

   public Animator Screen;

    // Start is called before the first frame update
    void Start()
    {
        Screen = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // door.transform.rotation.y += openPersent
    }

    public void Open()
    {
        Screen.Play("screen_open", 0, 0.0f);
    }

}
