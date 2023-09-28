using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public enum ShadowDistance {LOW, MEDIUM, HIGH, ULTRA}
public class Settings : MonoBehaviour
{
    public static Settings instance;
    public ShadowDistance shadowDistance;
    
    public GameObject Grass;
    public GameObject GrassSecond;
    public UniversalRendererData rendererData;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        if (PlayerPrefs.HasKey("Grass"))
        {
            SetGrassState(PlayerPrefs.GetInt("Grass"));
        }
        else
        {
            SetGrassState(1);
        }

        if (PlayerPrefs.HasKey("AmbientOcclusion"))
        {
            SetAmbientOcclusionState(PlayerPrefs.GetInt("AmbientOcclusion"));
        }
        else
        {
            SetAmbientOcclusionState(1);
        }


        if(PlayerPrefs.HasKey("currentShadowDistanceIndex"))
        {
            UpdateShadowDistance(PlayerPrefs.GetInt("currentShadowDistanceIndex"));
        }
        else
        {
            UpdateShadowDistance(2);
        }

        
    }
    public void SetGrassState(int state)
    {
        if(state == 1)
        {
            Grass.SetActive(true);
            GrassSecond.SetActive(true);
        }
        else
        {
            Grass.SetActive(false);
            GrassSecond.SetActive(false);
        }
    }

    public void SetAmbientOcclusionState(int state)
    {
        if(state == 1)
        {
            if(rendererData.rendererFeatures[0].name == "ScreenSpaceAmbientOcclusion")
            {
                rendererData.rendererFeatures[0].SetActive(true);
            }
            
        }
        else
        {
            if(rendererData.rendererFeatures[0].name == "ScreenSpaceAmbientOcclusion")
            {
                rendererData.rendererFeatures[0].SetActive(false);
            }
        }
    }

    public string UpdateShadowDistance(int index)
    {
        UniversalRenderPipelineAsset urp = (UniversalRenderPipelineAsset)UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline;
        switch (index)
        {
            case 0:
            shadowDistance = ShadowDistance.LOW;
            QualitySettings.shadowDistance = 50;
            urp = (UniversalRenderPipelineAsset)UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline;
            urp.shadowDistance = 50;
            return "LOW";
            case 1:
            shadowDistance = ShadowDistance.MEDIUM;
            QualitySettings.shadowDistance = 130;
            urp = (UniversalRenderPipelineAsset)UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline;
            urp.shadowDistance = 130;
            return "MEDIUM";
            case 2:
            shadowDistance = ShadowDistance.HIGH;
            QualitySettings.shadowDistance = 200;
            urp = (UniversalRenderPipelineAsset)UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline;
            urp.shadowDistance = 200;
            return "HIGH";
            case 3:
            shadowDistance = ShadowDistance.ULTRA;
            QualitySettings.shadowDistance = 400;
            urp = (UniversalRenderPipelineAsset)UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline;
            urp.shadowDistance = 400;
            return "ULTRA";
            default:
            shadowDistance = ShadowDistance.HIGH;
            QualitySettings.shadowDistance = 200;
            urp = (UniversalRenderPipelineAsset)UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline;
            urp.shadowDistance = 200;
            return "HIGH";
        }
    }
}
