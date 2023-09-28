using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingManager : MonoBehaviour
{
    public Light DirectionalLight;
    public LightingPreset Preset;

    public Color StartDirectionalColor;
    public Color StartAmbientColor;
    public Vector3 StartRotation;

    public Material DaySkybox;
    public Material NightSkybox;
    
    public float SkyboxChangeSpeed;


    void Start()
    {
        StartDirectionalColor = DirectionalLight.color;
        StartAmbientColor =  RenderSettings.ambientLight;
        StartRotation = new Vector3(DirectionalLight.transform.eulerAngles.x, DirectionalLight.transform.eulerAngles.y, DirectionalLight.transform.eulerAngles.z);
        
		SetDefaultSkybox();
        
    }

    public void SetDefaultSkybox()
    {
        RenderSettings.skybox = NightSkybox;
    }

    
    
    public void UpdateLighting(float TimePercent)
    {
        RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(TimePercent);
        DirectionalLight.color = Preset.DirectionalColor.Evaluate(TimePercent);
        DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((TimePercent * 360f)-90, 180 ,0));
    }

    public void SetDefaultLighting()
    {
        RenderSettings.ambientLight = StartAmbientColor;
        DirectionalLight.color = StartDirectionalColor;
        DirectionalLight.transform.localRotation = Quaternion.Euler(StartRotation);
    }


    public IEnumerator ChangeSkybox(Material NewSkybox) {
		float t = 0f;
        Debug.Log("Changing skybox");

		while (t < 1.0f) {
			RenderSettings.skybox.Lerp (RenderSettings.skybox, NewSkybox, t);

			t += (SkyboxChangeSpeed * Time.deltaTime) / 50;

			yield return null;
		}
        
        RenderSettings.skybox = NewSkybox;
	}

    void OnApplicationQuit () {
		RenderSettings.skybox = DaySkybox;
	}
}
