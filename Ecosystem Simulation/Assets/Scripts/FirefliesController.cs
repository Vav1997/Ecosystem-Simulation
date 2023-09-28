using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirefliesController : MonoBehaviour
{
    public ParticleSystem FireflyParticle;
    public Light AreaLight;

    public float DayLightIntensity;
    public float NightLightIntensity;
    public float LightlerpTime;
    void Start()
    {
        FireflyParticle = GetComponent<ParticleSystem>();
        AreaLight = GetComponentInChildren<Light>();
    }

    
    void Update()
    {
        
    }

    public void SwitchMode(bool toDay)
    {
        if(toDay)
        {
            FireflyParticle.loop = false;
            StartCoroutine(LerpLightIntensity(true));
        }
        else
        {
            FireflyParticle.Play();
            FireflyParticle.loop = true;
            StartCoroutine(LerpLightIntensity(false));
        }
    }


    public IEnumerator LerpLightIntensity(bool ToDay) {
        
        
        float oldValue;
        float newValue;

        if(ToDay)
        {
            oldValue = NightLightIntensity;
            newValue = DayLightIntensity;
        }
        else
        {
            oldValue = DayLightIntensity;
            newValue = NightLightIntensity;
            AreaLight.enabled = true;
        }

        float currentLerpTime = 0f;
        while (currentLerpTime < LightlerpTime) {
            currentLerpTime += Time.deltaTime;
            float t = currentLerpTime / LightlerpTime;

            AreaLight.intensity = Mathf.Lerp(oldValue, newValue, t);            
            yield return null;
        }

        if(ToDay)
        {
            AreaLight.enabled = false;
        }

    }

}
