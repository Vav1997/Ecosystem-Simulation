using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Ages {Baby, Middle, Adult}


[System.Serializable]
public class Age {
    public string AgeName;
    public float StatMultiplier;
    public float ScaleMultiplier;
    public float TimeForNextStage;
}
public class AgeController : MonoBehaviour
{
    public Ages AgeState;
    public Age[] age;
    
    public float AgeTimer;
    public bool AgeChanged;
    public Animal Animal;

    public float GrowSpeed;
    public float ScaleGrowSpeed;
    public bool SizeChanged;

    public GameObject Model;

    private float StartingScaleX;
    private float StartingScaleY;
    private float StartingScaleZ;

    void Start()
    {
        AgeTimer = 0;
        StartingScaleX = Model.transform.localScale.x;
        StartingScaleY = Model.transform.localScale.y;
        StartingScaleZ = Model.transform.localScale.z;

        Animal = GetComponent<Animal>();
        AgeState = Ages.Baby;
        GrowStats(age[0]);

        //Model.transform.localScale = new Vector3(Model.transform.localScale.x * age[0].ScaleMultiplier, Model.transform.localScale.y * age[0].ScaleMultiplier, Model.transform.localScale.z * age[0].ScaleMultiplier);
        Model.transform.localScale = Vector3.zero;
        
    }

    // Update is called once per frame
    void Update()
    {
        AgeTimer += Time.deltaTime * GrowSpeed;

        if(AgeTimer >= age[0].TimeForNextStage && AgeState == Ages.Baby)
        {
            AgeState = Ages.Middle;
           
            GrowStats(age[1]);;
            AgeTimer = 0;
        }

         if(AgeTimer >= age[1].TimeForNextStage && AgeState == Ages.Middle)
        {
            AgeState = Ages.Adult;
            
            GrowStats(age[2]);
        }

        ChangeSize();
        
    }


    public void GrowStats(Age age)
    {

        
        //set run speed
        Animal.runSpeed = Animal.DefaultRunSpeed * age.ScaleMultiplier;

        //set stamina
        float staminaPercent = Animal.SurvivalManager.staminaPercent;
        Animal.SurvivalManager.maxStamina = Animal.SurvivalManager.DefaultStartingStamina * age.ScaleMultiplier;
        Animal.SurvivalManager.currentStamina = Animal.SurvivalManager.maxStamina * staminaPercent;

        //set Radius
        Animal.FieldOfview.radius = Animal.FieldOfview.DefaultRadius * age.ScaleMultiplier;
        
    }

    public void ChangeSize()
    {
        if(AgeState == Ages.Baby)
        {
            Model.transform.localScale = Vector3.Lerp(Model.transform.localScale, new Vector3(StartingScaleX * age[0].ScaleMultiplier,StartingScaleY * age[0].ScaleMultiplier, StartingScaleZ * age[0].ScaleMultiplier), ScaleGrowSpeed * Time.deltaTime);
        }
        else if(AgeState == Ages.Middle)
        {
            Model.transform.localScale = Vector3.Lerp(Model.transform.localScale, new Vector3(StartingScaleX * age[1].ScaleMultiplier,StartingScaleY * age[1].ScaleMultiplier, StartingScaleZ * age[1].ScaleMultiplier), ScaleGrowSpeed * Time.deltaTime);
        }
        else if(AgeState == Ages.Adult)
        {
            Model.transform.localScale = Vector3.Lerp(Model.transform.localScale, new Vector3(StartingScaleX * age[2].ScaleMultiplier,StartingScaleY * age[2].ScaleMultiplier, StartingScaleZ * age[2].ScaleMultiplier), ScaleGrowSpeed * Time.deltaTime);
        }
        
    }
}
