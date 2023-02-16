using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterControl : MonoBehaviour
{
    public GameObject water;
    // Start is called before the first frame update
    void Start()
    { 
        water.transform.position = new Vector3(-20, -20, -20);
    }

    public void Show()
    {   
        water.transform.position = new Vector3(-16, 9, 0);
        water.SetActive(true);
    }

    // Update is called once per frame
    public void Hide()
    {
         water.SetActive(false);
    }
}
