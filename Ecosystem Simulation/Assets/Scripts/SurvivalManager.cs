using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivalManager : MonoBehaviour
{
   [Header("Hunger")]
   public float maxHunger = 100;
   public float hungerDepletionRate = 1f;
   public float currentHunger;
   public float hungerPercentageToDamage;
   public float hungerPercentageToHeal;
   public bool isHungry;

   public float hungerPercent => currentHunger / maxHunger;

   [Header("Thurst")]
   public float maxThurst = 100;
   public float thurstDepletionRate = 1f;
   public float currentThurst;
   public float thurstPercentageToDamage;
   public float thurstPercentageToHeal;
   public float thurstPercent => currentThurst / maxThurst;
   public bool isThursty;

    [Header("Stamina")]

    public float DefaultStartingStamina;
   public float maxStamina = 5;
   public float staminaDepletionRate = 1f;
   public float staminaRechargeRate = 1f;

   public float staminaRechargeDelay = 1f;
   public float currentStamina;
   private float currentStaminaDelayCounter;
   public float staminaPercent => currentStamina / maxStamina;

   [Header("ReproductionUrge")]

   public float maxReproductionUrge = 100;
   public float currentReproductionUrge;
   public float ReproductionDepletionRate = 1f; 
   public float ReproductionPercent => currentReproductionUrge / maxReproductionUrge;
   public bool needsReproduce;

   [Header("Health")]
   public float maxHealth;
   public float currentHealth;
   public float healthDepletionRate = 3f;
   public float healthRegenRate = 3f;
   public float healthPercent => currentHealth / maxHealth;


   public float maxStatDifferencePercent;
   private Animal Animal;
   private AgeController Agecontroller;
   private ReproductionManager ReproductionManager;


   void Start()
   {
        Animal = GetComponent<Animal>();
        Agecontroller = GetComponent<AgeController>();
        ReproductionManager = GetComponent<ReproductionManager>();




        currentHunger = maxHunger - maxHunger * Random.Range(0f, maxStatDifferencePercent);
        currentThurst = maxThurst - maxThurst * Random.Range(0f, maxStatDifferencePercent);
        currentStamina = maxStamina - maxStamina * Random.Range(0f, maxStatDifferencePercent * 2);
        currentHealth = maxHealth;
        if(ReproductionManager.sex == Sex.Male)
        {   
            currentReproductionUrge = maxReproductionUrge * Random.Range(0f, maxStatDifferencePercent);
        }
   }


   void Update()
   {
        
        currentHunger -= hungerDepletionRate * Time.deltaTime;
        currentThurst -= thurstDepletionRate * Time.deltaTime;


        if(currentHunger <= maxHunger / 2)
        {
            isHungry = true;
        }
        else
        {
            isHungry = false;
        }

        if(currentThurst <= maxThurst / 2)
        {
            isThursty = true;
        }
        else
        {
             isThursty = false;
        }


        if(Agecontroller.AgeState == Ages.Adult && ReproductionManager.sex == Sex.Male)
        {
            currentReproductionUrge += ReproductionDepletionRate * Time.deltaTime;

            if(currentReproductionUrge >= maxReproductionUrge * 0.7)
            {
                needsReproduce = true;
            }
            else
            {
                needsReproduce = false;
            }
        }

        




        if(hungerPercent < hungerPercentageToDamage || thurstPercent < thurstPercentageToDamage)
        {
            currentHealth -= healthDepletionRate * Time.deltaTime;

            if(currentHealth < 0)
            {
                Debug.Log("Wolf Died");
                currentHealth = 0;
                Animal.OnDead();
            }
        }
        
        else if((hungerPercent > hungerPercentageToHeal && thurstPercent > thurstPercentageToHeal) && currentHealth < maxHealth)
        {
            currentHealth += healthRegenRate * Time.deltaTime;

            if(currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
        }

        if(Animal.state == State.Chase || Animal.state == State.Flee )
        {
            currentStamina -= staminaDepletionRate * Time.deltaTime;
            currentStaminaDelayCounter = 0;
            if(currentStamina <= 0)
            {
                Animal.OnStaminaExpired();
            }
        }

        if(Animal.state != State.Chase &&  Animal.state != State.Flee && currentStamina < maxStamina)
        {  
            if(currentStaminaDelayCounter < staminaRechargeDelay)
            {
                currentStaminaDelayCounter += Time.deltaTime;
            }

             if(currentStaminaDelayCounter >= staminaRechargeDelay)
            {
                currentStamina += staminaRechargeRate * Time.deltaTime;

                if(currentStamina > maxStamina)
                {
                    currentStamina = maxStamina;
                }
            }
        }
   }


   public void RegenHealth()
   {
    currentHunger = maxHunger;
   }

   public void RegenThurst()
   {
    currentThurst = maxThurst;
   }
}
