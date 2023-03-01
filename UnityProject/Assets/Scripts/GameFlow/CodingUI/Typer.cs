using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Typer : MonoBehaviour
{
    public string dirPath = string.Empty;
    public RawImage img;
    public GameObject GameManager;
    GameFlowControl GameFlowControl;
    int count = 0;

    public AudioSource[] SoundFX;

    Texture2D tex = null;
    byte[] fileData;

    void Start()
    {
        dirPath = "Code/"+SceneManager.GetActiveScene().name;
        GameFlowControl = GameManager.GetComponent<GameFlowControl>();
        LoadImage();
    }


    // Update is called once per frame
    void LoadImage()
    {
        string filePath = dirPath+"/"+count.ToString();
        Debug.Log(filePath);
        // check if resource exists
        tex = Resources.Load<Texture2D>(filePath);

        if (tex != null){
            tex = Resources.Load<Texture2D>(filePath);
            img.texture = tex;
        }else{
            Debug.Log("next level");
            // load animation here
            GameFlowControl.LoadLevel();
        }
    }

    private void Update()
    {
        if(Input.anyKeyDown){
            int index = Random.Range(0, SoundFX.Length);
            SoundFX[index].Play();
            count++;
            LoadImage();
        }
    }
}