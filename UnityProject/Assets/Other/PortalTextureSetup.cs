using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTextureSetup : MonoBehaviour
{
    public Camera CameraOrth;
    public Material CameraMatOrth;

    // Start is called before the first frame update
    void Start()
    {
        if (CameraOrth.targetTexture != null)
        {
            CameraOrth.targetTexture.Release();
        }
        CameraOrth.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        CameraMatOrth.mainTexture = CameraOrth.targetTexture;
        // make
    }


    // // Update is called once per frame
    // void Update()
    // {
        
    // }
}
