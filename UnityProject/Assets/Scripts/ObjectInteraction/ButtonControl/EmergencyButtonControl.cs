// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EmergencyButtonControl : MonoBehaviour
{
    public Rigidbody buttonTopRigid;
    public Transform buttonTop;
    public Transform buttonLowerLimit;
    public Transform buttonUpperLimit;
    public float threshHold;
    public float force = 10;
    private float upperLowerDiff;
    public bool isPressed;
    private bool prevPressedState;
    public AudioSource pressedSound;
    public AudioSource releasedSound;
    public Collider[] CollidersToIgnore;
    public UnityEvent onPressed;
    public UnityEvent onReleased;
    public DoorControl DoorControl;
    public DoorControl2 DoorControl2;
    public DoorControl3 DoorControl3;
    public WaterControl WaterControl;

    // Start is called before the first frame update
    void Start()
    {   

        DoorControl = GameObject.FindObjectOfType<DoorControl>();
        DoorControl2 = GameObject.FindObjectOfType<DoorControl2>();
        DoorControl3 = GameObject.FindObjectOfType<DoorControl3>();
        WaterControl = GameObject.FindObjectOfType<WaterControl>();
        Collider localCollider = GetComponent<Collider>();
        if (localCollider != null)
        {
            Physics.IgnoreCollision(localCollider, buttonTop.GetComponentInChildren<Collider>());

            foreach (Collider singleCollider in CollidersToIgnore)
            {
                Physics.IgnoreCollision(localCollider, singleCollider);
            }
        }
        
        if (transform.eulerAngles != Vector3.zero){
            Vector3 savedAngle = transform.eulerAngles;
            transform.eulerAngles = Vector3.zero;
            upperLowerDiff = buttonUpperLimit.position.y - buttonLowerLimit.position.y;
            transform.eulerAngles = savedAngle;
        }
        else
            upperLowerDiff = buttonUpperLimit.position.y - buttonLowerLimit.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        buttonTop.transform.localPosition = new Vector3(0, buttonTop.transform.localPosition.y, 0);
        buttonTop.transform.localEulerAngles = new Vector3(0, 0, 0);
        if (buttonTop.localPosition.y >= 0)
            buttonTop.transform.position = new Vector3(buttonUpperLimit.position.x, buttonUpperLimit.position.y, buttonUpperLimit.position.z);
        else
            buttonTopRigid.AddForce(buttonTop.transform.up * force * Time.deltaTime);

        if (buttonTop.localPosition.y <= buttonLowerLimit.localPosition.y)
            buttonTop.transform.position = new Vector3(buttonLowerLimit.position.x, buttonLowerLimit.position.y, buttonLowerLimit.position.z);


        if (Vector3.Distance(buttonTop.position, buttonLowerLimit.position) < upperLowerDiff * threshHold)
            isPressed = true;
        else
            isPressed = false;

        if(isPressed && prevPressedState != isPressed)
            Pressed();
        if(!isPressed && prevPressedState != isPressed)
            Released();
    }

    // void FixedUpdate(){
    //     Vector3 localVelocity = transform.InverseTransformDirection(buttonTop.GetComponent<Rigidbody>().velocity);
    //     Rigidbody rb = buttonTop.GetComponent<Rigidbody>();
    //     localVelocity.x = 0;
    //     localVelocity.z = 0;
    //     rb.velocity = transform.TransformDirection(localVelocity);
    // }

    void Pressed(){
        prevPressedState = isPressed;
        pressedSound.pitch = 1;
        pressedSound.Play();
        onPressed.Invoke();
        DoorControl.Open();
        DoorControl2.OpenLeft();
        DoorControl3.OpenRight();
        WaterControl.Show();
        // door.gameObject.SetActive(false);
    }

    void Released(){
        prevPressedState = isPressed;
        releasedSound.pitch = Random.Range(1.1f, 1.2f);
        releasedSound.Play();
        onReleased.Invoke();
        DoorControl.Close();
        DoorControl2.CloseLeft();
        DoorControl3.CloseRight();
        WaterControl.Hide();
        // door.gameObject.SetActive(true);
    }
}
