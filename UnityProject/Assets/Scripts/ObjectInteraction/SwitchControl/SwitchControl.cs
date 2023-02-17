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
    public Animation switchOnAnime;
    public Animation switchOffAnime;
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
        switchOnAnime.Play();
        onSwitchOn.Invoke();
    }

    private void switchOff(){
        switchOffAnime.Play();
        onSwitchOff.Invoke();
    }
}
