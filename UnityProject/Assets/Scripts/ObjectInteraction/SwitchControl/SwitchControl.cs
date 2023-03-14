using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SwitchControl : MonoBehaviour
{
    public int state = 1; 
    public float distance = 5f;
    public GameObject player;
    public IllusionManager IllusionManager;
    public AudioSource switchSound;
    public UnityEvent onSwitchOn;
    public UnityEvent onSwitchOff;

    // Start is called before the first frame update
    void Start()
    {
        IllusionManager = GameObject.FindObjectOfType<IllusionManager>();
        switchSound = GetComponent<AudioSource>();
        // hold distance
        distance = IllusionManager.holdDistance;
    }

    void Update()
    {
        // if player is in distance
        if (Vector3.Distance(player.transform.position, transform.position) < distance){
            // if player press E
            if (Input.GetMouseButtonDown(0) && IllusionManager.target == null && IllusionManager.outline == null){
                switchSound.Play();
                if (state == 0){
                    switchOn();
                }else{
                    switchOff();
                }
                state = 1 - state;
            }
        }
    }

    // one trigger stay
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag=="Player"){
            Debug.Log("OnTriggerEnter");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag=="Player"){
            Debug.Log("OnTriggerStay");
            if (Input.GetMouseButtonDown(0)){
                switchSound.Play();
                if (state == 0){
                    switchOn();
                }else{
                    switchOff();
                }
            }
        }
    }

    private void switchOn(){
        onSwitchOn.Invoke();
    }

    private void switchOff(){
        onSwitchOff.Invoke();
    }
}
