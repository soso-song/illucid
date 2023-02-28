using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameFlowControl : MonoBehaviour
{
    public RawImage mask;
    void Start()
    {
        // play mask animation
        StartCoroutine(playBlinkAnimation(1));
    }

    private IEnumerator playBlinkAnimation(float dir)
    {
        // play mask animation
        Animator anim = mask.GetComponent<Animator>();
        anim.SetFloat("Direction", dir);
        anim.Play("BlinkEyeAnim");
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !anim.IsInTransition(0));
    }

    public void LoadLevel()
    {
        // reverse mask animation
        StartCoroutine(playBlinkAnimation(-1));
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1 );
    }

    public void QuitGame(){
        Application.Quit();
    }
}
