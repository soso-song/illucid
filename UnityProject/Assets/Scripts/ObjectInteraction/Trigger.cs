using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[Serializable]
public class TriggerEvent
{
    // tag
    public string tag;
    // trigger event
    public UnityEvent triggerEvent;
}

public class Trigger : MonoBehaviour
{
    public TriggerEvent[] onTriggerEnterEvents;
    public TriggerEvent[] onTriggerExitEvents;
    public TriggerEvent[] onTriggerStayEvents;
    

    void OnTriggerEnter(Collider other)
    {
        foreach (TriggerEvent triggerEvent in onTriggerEnterEvents)
        {
            if (other.gameObject.tag == triggerEvent.tag || triggerEvent.tag=="")
            {
                triggerEvent.triggerEvent.Invoke();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        foreach (TriggerEvent triggerEvent in onTriggerExitEvents)
        {
            if (other.gameObject.tag == triggerEvent.tag || triggerEvent.tag=="")
            {
                triggerEvent.triggerEvent.Invoke();
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        foreach (TriggerEvent triggerEvent in onTriggerStayEvents)
        {
            if (other.gameObject.tag == triggerEvent.tag || triggerEvent.tag=="")
            {
                triggerEvent.triggerEvent.Invoke();
            }
        }
    }
}
