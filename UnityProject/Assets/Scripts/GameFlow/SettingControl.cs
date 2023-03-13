using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingControl : MonoBehaviour
{
    public Slider musicSlider;
    public Slider soundSlider;
    public Slider mouseSensitivitySlider;

    void Start()
    {
        musicSlider.value = StateControl.MusicVolume;
        soundSlider.value = StateControl.SFXVolume;
        mouseSensitivitySlider.value = StateControl.MouseSensitivity;
    }

    public void changeMusicVolume()
    {
        StateControl.MusicVolume = musicSlider.value;
    }

    public void changeSFXVolume()
    {
        StateControl.SFXVolume = soundSlider.value;
    }

    public void changeMouseSensitivity()
    {
        StateControl.MouseSensitivity = mouseSensitivitySlider.value;
    }
}
