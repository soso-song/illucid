using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowControl : MonoBehaviour
{
    void Start()
    {
        // load "open-eye" animation
    }

    public void LoadLevel()
    {
        //load "close-eye" animation
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1 );
    }

    public void QuitGame(){
        Application.Quit();
    }
}
