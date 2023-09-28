using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUI : MonoBehaviour
{
    public string[] shadowDistanceText;
    public TextMeshProUGUI shadowDistanceTextObj;
    [SerializeField] private int _currentShadowDistanceIndex;
    public Toggle Grass;
    public Toggle AmbientOcclusion;
    public Settings Settings;
    void Start()
    {
        if (PlayerPrefs.HasKey("Grass"))
        {
            SetGrassUIState(PlayerPrefs.GetInt("Grass"));
        }
        else
        {
            SetGrassUIState(1);
        }

        if (PlayerPrefs.HasKey("AmbientOcclusion"))
        {
            SetAmbientOcclusionUIState(PlayerPrefs.GetInt("AmbientOcclusion"));
        }
        else
        {
            SetAmbientOcclusionUIState(1);
        }
        
        if(PlayerPrefs.HasKey("AmbientOcclusion"))
        {
            shadowDistanceTextObj.text = Settings.instance.UpdateShadowDistance(PlayerPrefs.GetInt("currentShadowDistanceIndex"));
        }
        else
        {
            shadowDistanceTextObj.text = Settings.instance.UpdateShadowDistance(2);
        }
        
        
        
    }
    public void SetGrassUIState(int state)
    {
        if(state == 1)
        {
            Grass.isOn = true;
        }
        else
        {
            Grass.isOn = false;
        }
    }

    public void ChangeGrassState()
    {
        if(Grass.isOn)
        {
            PlayerPrefs.SetInt("Grass", 1);
            if(Settings != null)
            {
                Settings.SetGrassState(1);
            }
        }
        else
        {
            PlayerPrefs.SetInt("Grass", 0);
            if(Settings != null)
            {
                Settings.SetGrassState(0);
            }
        }

        Debug.Log("Miban");
        Debug.Log(PlayerPrefs.GetInt("Grass"));

        
    }

    public void SetAmbientOcclusionUIState(int state)
    {
        if(state == 1)
        {

            AmbientOcclusion.isOn = true;
        }
        else
        {
            AmbientOcclusion.isOn = false;
        }
    }

    public void ChangeAmbientOcclusionState()
    {
        if(AmbientOcclusion.isOn)
        {
            PlayerPrefs.SetInt("AmbientOcclusion", 1);
            if(Settings != null)
            {
                Settings.SetAmbientOcclusionState(1);
            }
        }
        else
        {
            PlayerPrefs.SetInt("AmbientOcclusion", 0);
            if(Settings != null)
            {
                Settings.SetAmbientOcclusionState(0);
            }
        } 
    }

    public void SwitchShadowDistance(bool toRight)
    {
        if(toRight && _currentShadowDistanceIndex < (System.Enum.GetValues(typeof(ShadowDistance)).Length - 1))
        {
            _currentShadowDistanceIndex++;
            
        }
        else if (toRight)
        {
            _currentShadowDistanceIndex = 0;
        }
        else if(!toRight && _currentShadowDistanceIndex > 0)
        {
            _currentShadowDistanceIndex--;
        }
        else if(!toRight && _currentShadowDistanceIndex <= 0)
        {
            _currentShadowDistanceIndex =  (System.Enum.GetValues(typeof(ShadowDistance)).Length - 1);
        }
        
        shadowDistanceTextObj.text = Settings.instance.UpdateShadowDistance(_currentShadowDistanceIndex);
        PlayerPrefs.SetInt("currentShadowDistanceIndex", _currentShadowDistanceIndex);
        
    }
}
