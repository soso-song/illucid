using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;


public class GameFlowControl : MonoBehaviour
{
    public RawImage mask;
    bool isloading = false;
    float timeElapsed = 0f;
    void Start()
    {
        // play open eye animation-
        if (mask == null){
            if (GameObject.Find("Mask") == null){
                // create a new mask from perfab
                GameObject maskObj = Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Mask.prefab", typeof(GameObject))) as GameObject;
                mask = maskObj.GetComponent<RawImage>();
                // add to canvas
                maskObj.transform.SetParent(GameObject.Find("Canvas").transform, false);



            } else{
                mask = GameObject.Find("Mask").GetComponent<RawImage>();
            }
        }
        StartCoroutine(playBlinkAnimation("OpenEyeAnim"));
    }

    IEnumerator playBlinkAnimation(string clip)
    {
        Animator anim = mask.GetComponent<Animator>();
        anim.Play(clip);
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length+0.5f);
    }

    public void LoadLevel()
    {
        StartCoroutine(playBlinkAnimation("CloseEyeAnim"));
        isloading = true;
    }

    void Update()
    {
        if (isloading)
        {
            timeElapsed += Time.deltaTime;
            // if no animation is playing, load next scene
            if (timeElapsed > 1.5f){
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }

    public void QuitGame(){
        Application.Quit();
    }
}
