using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenControl : MonoBehaviour
{

    public Animator Screen;
    private bool isOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        Screen = GetComponent<Animator>();
        isOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        // door.transform.rotation.y += openPersent
        if (isOpen)
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y - 2, transform.position.z), 2 * Time.deltaTime);
    }

    public void Open()
    {
        isOpen = true;
        // Screen.Play("screen_open", 0, 0.0f);
        // move the y position of the current object
        // transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y - 2, transform.position.z), 2 * Time.deltaTime);
    }

}
