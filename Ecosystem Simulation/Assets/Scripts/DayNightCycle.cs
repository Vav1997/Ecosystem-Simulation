using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
	[Header("Lights")]
	//SUN
	public AnimationCurve sunIntensityCurve;
	public float minSunLightIntensity = 0.1f;
	public float maxSunLightIntensity = 1.5f;
	public Light sunLight;

	//MOON
	public AnimationCurve moonIntensityCurve;
	public float minMoonLightIntensity = 0.3f;
	public float maxMoonLightIntensity = 0.6f;
	public Light moonLight;

	[Header("Skybox")]
	public Gradient skyTopColorGradient;
	public Gradient skyMiddleColorGradient;

	[Header("Time")]
	public float dayLengthInSeconds = 30;
	public float timeScale = 1;

	[Header("DEBUG")]
	public float simulatedTime = 0;
	static public float SIMDT = 0;
	public float timeOfDay = 0;

	public float clockTime = 0;
	public string clockTimeString = "00:00";
	public TextMeshProUGUI clockText;

	[Header("Fog")]
	public Gradient fogColorGradient;
	public AnimationCurve fogDensityCurve;
	public float fogDensityMin = 0.005f;
	public float fogDensityMax = 0.01f;
	Color fogColor;

	[Header("Reflections")]
	public AnimationCurve reflectionIntensityCurve;
	public float minReflectionIntensity = 0.1f;
	public float maxReflectionIntensity = 1f;
	public ReflectionProbe[] reflectionProbes;
	float refPower;

	[Header("Env Ambient Light")]
	public Gradient ambientLightColorGradient;
	public float ambientIntensityScalar = 0.25f;

	public Gradient[] ambientLightColorGradients;

	//time of day is in the range 0f-1f
	public Color GetAmbientLightColor(float timeOfDay)
	{
		//get the index of the gradient
		int index = Mathf.FloorToInt(timeOfDay * (ambientLightColorGradients.Length - 1));

		//get gradient by index
		Gradient gradient = ambientLightColorGradients[index];

		//get time of day adjusted by index
		float timeOfDayAdjusted = timeOfDay - (index / (float)(ambientLightColorGradients.Length - 1));

		//get color from gradient
		return gradient.Evaluate(timeOfDayAdjusted);
	}

	//TODO: Change sun/moon lights into fill lights when they are occluded/facing upside down
	//change their colors based on gradient
	//TODO: Get hours represented by time of day!
	//create a function that rotates the sun around the x axis, 360 degrees per minute

	void Start()
	{
		//get probes if no probes
		if (reflectionProbes == null || reflectionProbes.Length == 0)
		{
			reflectionProbes = FindObjectsOfType<ReflectionProbe>();
		}
	}

	void LateUpdate()
	{
		SetupTimeOfDay();

		//set sun/moon brightness
		SetCelestialBrightness();
		SetSkyboxColors();
		SetFogColors();
		SetReflectionIntensity();
		SetAmbientLightColor();

		//set day cycle system rotation
		transform.localRotation = Quaternion.Euler(timeOfDay * 360, 0, 0);
	}

	//set the sun and moon light based on their dot product and vector3.up
	public void SetCelestialBrightness()
	{
		sunLight.intensity = Mathf.Lerp(minSunLightIntensity, maxSunLightIntensity, sunIntensityCurve.Evaluate(timeOfDay));
		moonLight.intensity = Mathf.Lerp(minMoonLightIntensity, maxMoonLightIntensity, moonIntensityCurve.Evaluate(timeOfDay));
		/*
		Vector3 sunDirection = sunLight.transform.forward;
		Vector3 moonDirection = moonLight.transform.forward;

		float sunDot = Vector3.Dot(sunDirection, Vector3.up);
		float moonDot = Vector3.Dot(moonDirection, Vector3.up);

		if (sunDot < 0)
		{
			sunLight.renderMode = LightRenderMode.ForcePixel;
		}
		else
		{
			sunLight.renderMode = LightRenderMode.ForceVertex;
		}
		if (moonDot < 0)
		{
			moonLight.renderMode = LightRenderMode.ForcePixel;
		}
		else
		{
			moonLight.renderMode = LightRenderMode.ForceVertex;
		}*/

	}

	//use rendersettings to change skybox colors top middle bottom to a gradient based on time of day
	void SetSkyboxColors()
	{
		RenderSettings.skybox.SetColor("_TopColor", skyTopColorGradient.Evaluate(timeOfDay));
		RenderSettings.skybox.SetColor("_HorizonColor", skyMiddleColorGradient.Evaluate(timeOfDay));
		//RenderSettings.skybox.SetColor("_BottomColor", fogColorGradient.Evaluate(timeOfDay));
	}

	//set fog color and density
	void SetFogColors()
	{
		//set fog color based on time of day
		fogColor = fogColorGradient.Evaluate(timeOfDay);
		RenderSettings.fogColor = fogColor;

		//remap animationcurve between fog density min and max
		RenderSettings.fogDensity = Mathf.Lerp(fogDensityMin, fogDensityMax, fogDensityCurve.Evaluate(timeOfDay));
	}

	//sets ambient light color
	//FIXME: set a 3 layer gradient instead of a single color
	void SetAmbientLightColor()
	{
		RenderSettings.ambientLight = ambientLightColorGradient.Evaluate(timeOfDay) * ambientIntensityScalar;
	}

	//sets reflection intensity
	public void SetReflectionIntensity()
	{
		refPower = Mathf.Lerp(minReflectionIntensity, maxReflectionIntensity, reflectionIntensityCurve.Evaluate(timeOfDay));
		RenderSettings.reflectionIntensity = refPower;
		foreach (ReflectionProbe probe in reflectionProbes)
		{
			probe.intensity = refPower;
		}

	}

	//setup time of day
	void SetupTimeOfDay()
	{
		//step time
		//FIXME: set up timestamps, and pauseing the time. timescale too?
		simulatedTime += Time.deltaTime;

		SIMDT = Time.deltaTime / dayLengthInSeconds;
		//float timeOfDay = simulatedTime / (dayLengthInMinutes * 60);
		timeOfDay = simulatedTime / dayLengthInSeconds;
		timeOfDay = Mathf.Repeat(timeOfDay, 1); //loop it / modulo 1

		clockTime = timeOfDay * 2400;

		if (clockTime <= 1200)
		{
			//set am stuff
			clockTimeString = clockTime.ToString("00:00") + " AM";
		}
		else
		{
			//clockTime -= 1200; //subtract 12 hours temp fix
			//set pm stuff
			clockTimeString = clockTime.ToString("00:00") + " PM";
		}

		clockTimeString += "\n" + "sun:" + sunLight.intensity.ToString("0.00");
		clockTimeString += "\n" + "moon:" + moonLight.intensity.ToString("0.00");
		clockTimeString += "\n" + "ref:" + refPower.ToString("0.00");

//		if (clockText.text != clockTimeString)
		//{
            //het berel

			//clockText.text = clockTimeString;
		//}

	}

} //eoc