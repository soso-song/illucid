//
// Created by Soso Song (@sososong) on 3/11/2023
// Copyright (c) 2023 Zhifei(Soso) Song. All rights reserved.
//

using UnityEngine.Rendering.PostProcessing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public GameObject GroupA;
    public GoalTrigger GoalATrigger;
    public GoalTrigger GoalBTrigger;
    public Transform Passwall;
    public GameObject Key;
    public float duration = 2.0f;

    public float lengthPasswallY = 9f;

    public PostProcessVolume postProcessingVolume;

    public void Changelevel(){

    }

    public void UseKey(GameObject sliceL, GameObject sliceR){
        // unpickable
        sliceL.layer = 0;
        sliceR.layer = 0;
        // set to kinematic
        sliceL.GetComponent<Rigidbody>().isKinematic = false;
        sliceR.GetComponent<Rigidbody>().isKinematic = false;
        // add force to the slices
        Vector3 forceDir = Camera.main.transform.right;
        sliceL.GetComponent<Rigidbody>().AddForce(forceDir * 60f);
        sliceR.GetComponent<Rigidbody>().AddForce(forceDir * -60f);
        Bloom bloom;
        if (postProcessingVolume.profile.TryGetSettings(out bloom))
        {
            // Debug.Log("Bloom intensity: " + bloom.intensity.value);
            StartCoroutine(setBloomIntensityAndThreshold(bloom, 3f, 40f, 0f));
        }
    }
    public IEnumerator setBloomIntensityAndThreshold(Bloom bloom, float duration, float bloomValue, float thresholdValue){
        float elapsed = 0.0f;
        float startBloomValue = bloom.intensity.value;
        float startThresholdValue = bloom.threshold.value;
        while (elapsed < duration) {
            bloom.intensity.value = Mathf.Lerp(startBloomValue, bloomValue, elapsed / duration);
            bloom.threshold.value = Mathf.Lerp(startThresholdValue, thresholdValue, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Changelevel();
    }

    public bool CheckPassCond(){
        if( GoalATrigger.triggered == true && GoalBTrigger.triggered == true){
            // during transition
            // disable color change
            GoalATrigger.gameObject.SetActive(false); 
            GoalBTrigger.gameObject.SetActive(false);
            // disable triggered object pickup
            GoalATrigger.triggerObj.gameObject.layer = 0;
            GoalBTrigger.triggerObj.gameObject.layer = 0;

            StartCoroutine(TriggerPasswall());
            return true;
        }
        return false;
    }

    public IEnumerator TriggerPasswall(){
        float elapsed = 0.0f;
        float startY = Passwall.position.y;
        float endY = startY + lengthPasswallY;
        // print("startY: " + startY);
        // print("endY: " + endY);
        // move the wall up
        while (elapsed < duration) {
            Passwall.position = new Vector3(
                Passwall.position.x, 
                Mathf.Lerp(startY, endY, elapsed / duration), 
                Passwall.position.z
            );
            elapsed += Time.deltaTime;
            yield return null;
        }

        // remove the objects
        GroupA.SetActive(false);
        Key.SetActive(true);

        // wait for 2 seconds
        yield return new WaitForSeconds(2f);

        // // move the wall down
        elapsed = 0f;
        while (elapsed < duration) {
            Passwall.position = new Vector3(
                Passwall.position.x, 
                Mathf.Lerp(endY, startY, elapsed / duration), 
                Passwall.position.z
            );
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
