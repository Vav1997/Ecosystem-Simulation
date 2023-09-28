using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderValue : MonoBehaviour
{
    public Slider EnvSoundsSlider;
    public Slider AnimalSoundsSlider;
    void Start()
    {
        EnvSoundsSlider.value = PlayerPrefs.GetFloat("EnvironmentSoundVolume");
        AnimalSoundsSlider.value = PlayerPrefs.GetFloat("AnimalSoundVolume");
    }
}
