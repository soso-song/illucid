using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;
using System;


public class GameFlowControl : MonoBehaviour
{
    public RawImage mask;
    bool isloading = false;
    float timeElapsed = 0f;
    public GameObject maskPerfab;
    public string curRoom = "room 0";

    void Start()
    {
        // Debug.Log("Game Time: "+Time.time + ", Level: "+SceneManager.GetActiveScene().buildIndex + ", Level Start");
        // play open eye animation-
        if (mask == null){
            if (GameObject.Find("Mask") == null){
                // create a new mask from perfab
                if (maskPerfab != null){
                    GameObject maskObj = Instantiate(maskPerfab) as GameObject;
                    mask = maskObj.GetComponent<RawImage>();
                    // add to canvas
                    maskObj.transform.SetParent(GameObject.Find("Canvas").transform, false);
                }
            } else{
                mask = GameObject.Find("Mask").GetComponent<RawImage>();
            }
        }
        StartCoroutine(playBlinkAnimation("OpenEyeAnim"));
    }

    IEnumerator playBlinkAnimation(string clip)
    {
        if (mask == null){
            yield break;
        }
        Animator anim = mask.GetComponent<Animator>();
        anim.Play(clip);
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
    }

    public void LoadLevel()
    {
        StartCoroutine(playBlinkAnimation("CloseEyeAnim"));
        // Debug.Log("Game Time: "+Time.time + ", Level: "+SceneManager.GetActiveScene().buildIndex + ", Level End");
        isloading = true;
    }

    void Update()
    {
        if (isloading)
        {
            timeElapsed += Time.deltaTime;
            // if no animation is playing, load next scene
            if (timeElapsed > 1f){
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }

    public void QuitGame(){
        Application.Quit();
    }
}
