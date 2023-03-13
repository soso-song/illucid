// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{   
    [SerializeField] GameObject pause;
    // Start is called before the first frame update
    // script to disbale
    public IllusionManager illusionManager;

    void Start(){
        pause.SetActive(false);
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.Escape)){
            pause.SetActive(true);
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            if (illusionManager != null)
                illusionManager.enabled = false;
        }
    }
    
    public void Resume(){
        pause.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (illusionManager != null)
            illusionManager.enabled = true;
    }

    public void Restart(){
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
