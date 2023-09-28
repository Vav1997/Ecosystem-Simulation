using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomController : MonoBehaviour
{
    //public TimeController TimeController;
    public MeshRenderer HeadRenderer;
    public Light MushroomLight;
    public float DayTimeIntensity;
    public float NightTimeIntensity;

    public float DayLightIntensity;
    public float NightLightIntensity;

    public float lerpTime;

    public bool DoLerp;
    public bool NightMode;

    public float minChangeTime;
    public float maxCHangeTime;

    void enabled()
    {
        
    }
    void Start()
    {
        
        MushroomLight.enabled = false;
        MushroomLight.intensity = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(DoLerp)
        {
            DoLerp = false;
            ChangeMode(NightMode);
        }
    }

    public void ChangeMode(bool NightMode)
    {
        
        // if(NightMode)
        // {
        //     float WaitThisTime = Random.Range(minChangeTime, maxCHangeTime);
        //     StartCoroutine(ChangeMushrooms(WaitThisTime));
        // }
        // else
        // {
            StartCoroutine(LerpColor(NightMode));
            StartCoroutine(LerpLightIntensity(NightMode));
        // }
        
    }

    public IEnumerator ChangeMushrooms(float WaitThisTime)
    {
        yield return new WaitForSeconds(WaitThisTime);
        StartCoroutine(LerpColor(NightMode));
        StartCoroutine(LerpLightIntensity(NightMode));
    }

     

    public IEnumerator LerpColor(bool MakeNight) {

        float oldValue;
        float newValue;
        if(MakeNight)
        {
            oldValue = DayTimeIntensity;
            newValue = NightTimeIntensity;
        }
        else
        {
            oldValue = NightTimeIntensity;
            newValue = DayTimeIntensity;
        }
        float currentLerpTime = 0f;
        while (currentLerpTime < lerpTime) {
            currentLerpTime += Time.deltaTime;
            float t = currentLerpTime / lerpTime;

            float currentValue = Mathf.Lerp(oldValue, newValue, t);
            HeadRenderer.material.SetFloat("_Emmition_str", currentValue);
            
            yield return null;
        }
    }

    public IEnumerator LerpLightIntensity(bool MakeNight) {
        
        
        float oldValue;
        float newValue;
        if(MakeNight)
        {
            oldValue = DayLightIntensity;
            newValue = NightLightIntensity;
            MushroomLight.enabled = true;
        }
        else
        {
            oldValue = NightLightIntensity;
            newValue = DayLightIntensity;
        }
        float currentLerpTime = 0f;
        while (currentLerpTime < lerpTime) {
            currentLerpTime += Time.deltaTime;
            float t = currentLerpTime / lerpTime;

            MushroomLight.intensity = Mathf.Lerp(oldValue, newValue, t);            
            yield return null;
        }

        if(!MakeNight)
        {
            MushroomLight.enabled = false;
        }

    }
}
