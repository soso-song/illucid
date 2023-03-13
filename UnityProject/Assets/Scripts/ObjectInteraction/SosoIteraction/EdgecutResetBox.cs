using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgecutResetBox : MonoBehaviour
{
    public Transform SlicesBucket;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 12)
        {
            // print("collider is colliding with MyOtherObject");
            // print(other.gameObject.transform.parent);
            // check if the gameobject has a parent
            if (other.gameObject.transform.parent == null){
                // get the name of the object
                if(other.gameObject.name.Contains("_")){
                    // get the name of the object which before the "_"
                    string name = other.gameObject.name.Split('_')[0];
                    print(name);
                }
                // check if the object name has "_" in it

                // Destroy(other.gameObject);
            }
                // print("no parent, make new");
        }
    }

    void RemoveObjectWithName(string name){
        foreach (GameObject obj in SlicesBucket.FindObjectsOfType<GameObject>()) {
            if (obj.name.Contains(name)) {
                Destroy(obj);
            }
        }
        // find the object with the name
        // get the object
        // loop through the object
        // check if the object has a parent

        // destroy the object
    }
}
