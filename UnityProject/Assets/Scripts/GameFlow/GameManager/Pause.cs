// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{   
    [SerializeField] GameObject pause;
    // Start is called before the first frame update

    void Start(){
        pause.SetActive(false);
    }

    void Update(){
        // if (Input.GetMouseButtonDown(0)){
            pause.SetActive(true);
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        // }
    }
    
    public void Resume(){
        pause.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Restart(){
        Time.timeScale = 1f;
        SceneManager.LoadScene("Day1Level");
    }
}
