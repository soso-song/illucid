using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SwitchControl : MonoBehaviour
{
    int state = 0; 
    public IllusionManager IllusionManager;
    public AudioSource switchSound;
    public UnityEvent onSwitchOn;
    public UnityEvent onSwitchOff;
    public Animator switchAnim;
    Outline outline;

    // Start is called before the first frame update
    void Start()
    {
        IllusionManager = GameObject.FindObjectOfType<IllusionManager>();
        outline = gameObject.GetComponent<Outline>();
        outline.OutlineWidth = 0;
        switchSound = gameObject.GetComponent<AudioSource>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag=="Player"){
            outline.OutlineWidth = 5;
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
        // switchAnim.Play("switchOn", 0, 0.0f);
        onSwitchOn.Invoke();
    }

    private void switchOff(){
        // switchAnim.Play("switchOff", 0, 0.0f);
        onSwitchOff.Invoke();
    }
}
