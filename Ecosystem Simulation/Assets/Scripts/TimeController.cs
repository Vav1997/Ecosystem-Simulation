using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TimeController : MonoBehaviour
{   

    
    
    public GameController GameController;
    private float timeMultiplier = 1000;
    [SerializeField] private float startHour;
    [SerializeField] private TextMeshProUGUI timeText;

    [SerializeField] private Light sunLight;

    [SerializeField] private float sunriseHour = 7;
    [SerializeField] private float sunsetHour = 20.5f;
    [SerializeField] private float dayChangeHour = 23.88f;

    [SerializeField] private Color dayAmbientLight;
    [SerializeField] private Color nightAmbientLight;

    [SerializeField] private AnimationCurve lightChangeCurve;

    [SerializeField] private float maxSunLightIntensity;

    [SerializeField] private Light MoonLight;

    [SerializeField] private float maxMoonLightIntensity;

    public DateTime currentTime;

    private TimeSpan sunriseTime;
    private TimeSpan sunsetTime;

    private TimeSpan daychangeTime;

    public int currentDay;

    private bool canChangeDay = true;


    public Material DaySkybox;
    public Material NightSkybox;
    public Material TargetSkybox;

    public float lerpTime = 2f; // Duration of the lerp in seconds
    
    private bool isLerping = false;

    private bool skyboxChanged;

    public SimulationInfoUI SimulationInfoUI;

    private bool DayModeActivated;
    private bool NightModeActivated;

    public delegate void ChangeToDay();
    public static event ChangeToDay OnChangedToDay; 


    public MushroomController[] MushroomControllers;
    public FirefliesController[] FirefliesControllers;

   
    
    void Start()
    {
        GameController = GetComponent<GameController>();
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(startHour);
        
        

        sunriseTime = TimeSpan.FromHours(sunriseHour);
        sunsetTime = TimeSpan.FromHours(sunsetHour);
        daychangeTime = TimeSpan.FromHours(dayChangeHour);
        SimulationInfoUI.DayText.SetText("Day " + currentDay.ToString());

        StartCoroutine(insertdata());

        
        RenderSettings.skybox = TargetSkybox;

        DayModeActivated = true;
        
    }

    public IEnumerator insertdata()
    {
        yield return new WaitForSeconds(2);
        GameController.InsertDailyCoyoteData(currentDay);
        GameController.InsertDailyTreeData(currentDay);
        GameController.InsertDailyDeerData(currentDay);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimeOfDay();
        RotateSun();
        UpdateLightSettings();
    }

    public void UpdateTimeOfDay()
    {
        currentTime = currentTime.AddSeconds(Time.deltaTime * timeMultiplier);

        //Day change
        if(currentTime.TimeOfDay > daychangeTime && canChangeDay)
        {
            canChangeDay = false;
            StartCoroutine(CanChangeDay());  
            currentDay++;
            GameController.InsertDailyCoyoteData(currentDay);
            GameController.InsertDailyTreeData(currentDay);
            GameController.InsertDailyDeerData(currentDay);
            SimulationInfoUI.DayText.SetText("Day " + currentDay.ToString());
            
        }

        if(currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime)
        {
            if(!DayModeActivated)
            {
                //Switch all objects to day mode
                DayModeActivated = true;
                NightModeActivated = false;
                StartLerp(DaySkybox);
                
                for (int i = 0; i < MushroomControllers.Length; i++)
                {
                    MushroomControllers[i].ChangeMode(false);
                }

                for (int i = 0; i < FirefliesControllers.Length; i++)
                {
                    FirefliesControllers[i].SwitchMode(true);
                }


            }    
        }
        else
        {
            if(!NightModeActivated)
            {
                //switch all objects to night mode
                NightModeActivated = true;
                DayModeActivated = false;
                StartLerp(NightSkybox);
                
                
                for (int i = 0; i < MushroomControllers.Length; i++)
                {
                    MushroomControllers[i].ChangeMode(true);
                }

                for (int i = 0; i < FirefliesControllers.Length; i++)
                {
                    FirefliesControllers[i].SwitchMode(false);
                }
                
            }
        }
        
    }

    public IEnumerator CanChangeDay()
    {
        yield return new WaitForSeconds(2);
        canChangeDay = true;
    }

    private void RotateSun()
    {
        float sunLightRotation;
        if(currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime)
        {
            TimeSpan sunriseToSunsetDuration = CalculateTimeDifference(sunriseTime, sunsetTime);
            TimeSpan timeSinceSunrise = CalculateTimeDifference(sunriseTime, currentTime.TimeOfDay);

            double percentage = timeSinceSunrise.TotalMinutes / sunriseToSunsetDuration.TotalMinutes;
            sunLightRotation = Mathf.Lerp(0, 180, (float)percentage);

            //StartLerp(NightSkybox, DaySkybox);
        }
        else
        {
            TimeSpan sunsetToSunriseDuration = CalculateTimeDifference(sunsetTime, sunriseTime);
            TimeSpan timeSinceSunset = CalculateTimeDifference(sunsetTime, currentTime.TimeOfDay);

            double percentage = timeSinceSunset.TotalMinutes / sunsetToSunriseDuration.TotalMinutes;

            sunLightRotation = Mathf.Lerp(180, 360, (float)percentage);

           

        }

        sunLight.transform.rotation = Quaternion.AngleAxis(sunLightRotation, Vector3.right);
    }


    private void UpdateLightSettings()
    {
        float dotProduct = Vector3.Dot(sunLight.transform.forward, Vector3.down);
        sunLight.intensity = Mathf.Lerp(0, maxSunLightIntensity, lightChangeCurve.Evaluate(dotProduct));
        MoonLight.intensity = Mathf.Lerp(maxMoonLightIntensity, 0, lightChangeCurve.Evaluate(dotProduct));
        RenderSettings.ambientLight = Color.Lerp(nightAmbientLight, dayAmbientLight, lightChangeCurve.Evaluate(dotProduct));

    }

    private TimeSpan CalculateTimeDifference(TimeSpan fromTime, TimeSpan toTime)
    {
        TimeSpan difference = toTime - fromTime;

        if(difference.TotalSeconds <0)
        {
            difference += TimeSpan.FromHours(24);
        }

        return difference;
    }


    IEnumerator LerpSkybox(Material start, Material end) {
        
        float currentLerpTime = 0f;
        while (currentLerpTime < lerpTime) {
            currentLerpTime += Time.deltaTime;
            
            float t = currentLerpTime / lerpTime;
            RenderSettings.skybox.Lerp(start, end, t);
            yield return null;
        }
        isLerping = false;
    }

    public void StartLerp (Material SkyboxToLerpTo) {
        if (!isLerping) {
            isLerping = true;
            StartCoroutine(LerpSkybox(TargetSkybox, SkyboxToLerpTo));
        }
    }


}
