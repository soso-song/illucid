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

    int nextLevel = 0;

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
        if (SceneManager.GetActiveScene().buildIndex == 0){
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        nextLevel = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextLevel >= SceneManager.sceneCountInBuildSettings){
            nextLevel = 0;
        }
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

    public void _loadLevel(int level)
    {
        if (level < SceneManager.sceneCountInBuildSettings){
            nextLevel = level;
        }else{
            nextLevel = 0;
        }
        LoadLevel();
    }

    void Update()
    {
        if (isloading)
        {
            timeElapsed += Time.deltaTime;
            // if no animation is playing, load next scene
            if (timeElapsed > 1f){
                // if has next scene, load next scene
                // else, load first scene
                SceneManager.LoadScene(nextLevel);
            }
        }
    }

    public void QuitGame(){
        Application.Quit();
    }
}
