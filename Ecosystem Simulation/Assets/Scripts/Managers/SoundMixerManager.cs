using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{

    

    [SerializeField] private AudioMixer _audioMixer;

    public void Start()
    {
        if (!PlayerPrefs.HasKey("EnvironmentSoundVolume")) // slider value was not set
        {
            SetEnvironmentVolume(0.5f);
            SetAnimalVolume(0.5f);
        }
       
    }
    
    public void SetEnvironmentVolume(float level)
    {
        float correctLevel = Mathf.Log10(level) * 20f;
        _audioMixer.SetFloat("EnvironmentSoundVolume", correctLevel);
        PlayerPrefs.SetFloat("EnvironmentSoundVolume", level);
    }

    public void SetAnimalVolume(float level)
    {
        float correctLevel = Mathf.Log10(level) * 20f;
        _audioMixer.SetFloat("AnimalSoundVolume", correctLevel);
        PlayerPrefs.SetFloat("AnimalSoundVolume", level);
    }
}
