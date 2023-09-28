using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Sex {Male, Female};




public class ReproductionManager : MonoBehaviour
{
    public float MateTime;
    public bool isPregnant;
    public Sex sex;
    public SurvivalManager SurvivalManager;
    public Animal Animal;
    public float PregnancyDuration;
    private float PregnancyTimer;
    public float maxMutationAmount;

    public float AcceptedDeviation;

    public PrefabRef MaleChildObj;
    public PrefabRef FemaleChildObj;

    public Animal MaleController;
    
    public List<GameObject> RegectedFemalesList = new List<GameObject>();
   
   public PrefabRef HeartsPrefab;
   public PrefabRef BrokenHeartsPrefab;
   public float BrokenHeartOffset;

   public float TwoChildrenSpawnChance;

   private float _baseNeckHeight;


   [SerializeField] private AudioClip _rejectSoundEffect;

    void Start()
    {
        Animal = GetComponent<Animal>();
        SurvivalManager = GetComponent<SurvivalManager>();
        _baseNeckHeight = GetComponent<DeerController>().Neck.transform.localPosition.y;
        
    }

    void Update()
    {
        if(isPregnant)
        {
            PregnancyTimer += Time.deltaTime;

            if(PregnancyTimer >= PregnancyDuration)
            {
                if(gameObject.GetComponent<CoyoteController>())
                {
                    GiveCoyoteBirth(MaleController, Animal);
                }
                else if(gameObject.GetComponent<DeerController>())
                {
                    GiveDeerBirth(MaleController, Animal);
                }
                
                PregnancyTimer = 0;
            }
        }
    }

    public bool RequestReproduction(Animal _MaleController)
    {
        if(_MaleController.runSpeed >= Animal.runSpeed * (1 - AcceptedDeviation) && _MaleController.FieldOfview.radius >= Animal.FieldOfview.radius * (1 - AcceptedDeviation) &&
         _MaleController.SurvivalManager.maxStamina >= Animal.SurvivalManager.maxStamina * (1 - AcceptedDeviation))
        {
            if(_MaleController.gameObject.GetComponent<DeerController>() && _MaleController.gameObject.GetComponent<DeerController>().NeckHeight >= gameObject.GetComponent<DeerController>().NeckHeight * (1 - AcceptedDeviation) &&
            _MaleController.gameObject.GetComponent<DeerController>().startFleeDistance >= gameObject.GetComponent<DeerController>().startFleeDistance * (1 - AcceptedDeviation * 1.5f))
            {
                //Accept request
                _MaleController.state = State.Mating;
                Animal.state = State.Mating;
                StartCoroutine(RotateTowards(_MaleController.gameObject.transform));
                isPregnant = true;
                //StartCoroutine(SetPregnant());
                MaleController = _MaleController;
                return true; 
            }
            else if (!_MaleController.gameObject.GetComponent<DeerController>())
            {
                //Accept request
                _MaleController.state = State.Mating;
                Animal.state = State.Mating;
                StartCoroutine(RotateTowards(_MaleController.gameObject.transform));
                isPregnant = true;
                //StartCoroutine(SetPregnant());
                MaleController = _MaleController;
                return true; 
            }

            else 
            {
                //Deni request
                SoundFXManager.instance.PlaySoundFXClip(_rejectSoundEffect, this.transform, 0.75f);
                _MaleController.ReproductionManager.RegectedFemalesList.Add(this.gameObject);
                Vector3 BrokenHeartPos = new Vector3(_MaleController.transform.position.x, _MaleController.transform.position.y + BrokenHeartOffset, _MaleController.transform.position.z);
                Instantiate(BrokenHeartsPrefab.prefab, BrokenHeartPos, Quaternion.identity);
                return false;
            }
            
        }

        else
        {
            //Deni request
            SoundFXManager.instance.PlaySoundFXClip(_rejectSoundEffect, this.transform, 0.75f);
            _MaleController.ReproductionManager.RegectedFemalesList.Add(this.gameObject);
            Vector3 BrokenHeartPos = new Vector3(_MaleController.transform.position.x, _MaleController.transform.position.y + BrokenHeartOffset, _MaleController.transform.position.z);
            Instantiate(BrokenHeartsPrefab.prefab, BrokenHeartPos, Quaternion.identity);
            return false;
        }
    }

    

   
    public void GiveDeerBirth(Animal MaleControl, Animal FemaleControl)
    {
        //Decide how many child will have
        int childToSpawn;
        if(Random.value <= TwoChildrenSpawnChance)
        {
            childToSpawn = 2;
        }
        else
        {
            childToSpawn = 1;
        }


        isPregnant = false;

        for (int i = 0; i < childToSpawn; i++)
        {
            GameObject Baby;

            if(Random.value >= 0.5f)
            {
                Baby = Instantiate(MaleChildObj.prefab, transform.position, Quaternion.identity);
            }
            else
            {
                Baby = Instantiate(FemaleChildObj.prefab, transform.position, Quaternion.identity);
            }

            Animal Babycontroller = Baby.GetComponent<Animal>();


            //run speed
            if(Random.value >= 0.5f)
            {   
                Babycontroller.DefaultRunSpeed = MaleControl.runSpeed + MaleControl.runSpeed * -1 * (ReturnRandom(-1f,1f) * maxMutationAmount);
            }
            else
            {
                Babycontroller.DefaultRunSpeed = FemaleControl.runSpeed + FemaleControl.runSpeed * -1 * (ReturnRandom(-1f,1f) * maxMutationAmount);
            }

            // FleeDistance
             if(Random.value >= 0.5f)
            {   
                Baby.gameObject.GetComponent<DeerController>().startFleeDistance = MaleControl.gameObject.GetComponent<DeerController>().startFleeDistance + MaleControl.gameObject.GetComponent<DeerController>().startFleeDistance * -1 * (ReturnRandom(-1f,1f) * maxMutationAmount);
            }
            else
            {
                Baby.gameObject.GetComponent<DeerController>().startFleeDistance = FemaleControl.gameObject.GetComponent<DeerController>().startFleeDistance + FemaleControl.gameObject.GetComponent<DeerController>().startFleeDistance * -1 * (ReturnRandom(-1f,1f) * maxMutationAmount);
            }

            //Field of view radius
            if(Random.value >= 0.5f)
            {
                    Babycontroller.FieldOfview.radius = MaleControl.FieldOfview.radius + MaleControl.FieldOfview.radius * -1 * (ReturnRandom(-1f,1f) * maxMutationAmount);
            }
            else
            {
                    Babycontroller.FieldOfview.radius = FemaleControl.FieldOfview.radius + FemaleControl.FieldOfview.radius * -1 * (ReturnRandom(-1f,1f) * maxMutationAmount);
            }

            //Stamina

            if(Random.value >= 0.5f)
            {
                    Babycontroller.SurvivalManager.maxStamina = MaleControl.SurvivalManager.maxStamina + MaleControl.SurvivalManager.maxStamina * -1 * (ReturnRandom(-1f,1f) * maxMutationAmount);
            }
            else
            {
                    Babycontroller.SurvivalManager.maxStamina = FemaleControl.SurvivalManager.maxStamina + FemaleControl.SurvivalManager.maxStamina * -1 * (ReturnRandom(-1f,1f) * maxMutationAmount);
            }

            //neck height
            float RandomMutationAmount = ReturnRandom(-1f,0.6f);
            if(Random.value >= 0.5f)
            {   
                DeerController BabyDeerController = Babycontroller.GetComponent<DeerController>();
                //Set baby neck height number and actual height to parent
                BabyDeerController.NeckHeight = MaleControl.GetComponent<DeerController>().NeckHeight;
                Babycontroller.Neck.transform.localPosition = new Vector3(Babycontroller.Neck.transform.localPosition.x, MaleControl.Neck.transform.localPosition.y, Babycontroller.Neck.transform.localPosition.z);
                
                //Mutate baby neck height number and actual height by RandomMutationAmount
                BabyDeerController.NeckHeight = BabyDeerController.NeckHeight + (2 * -1 * (RandomMutationAmount * maxMutationAmount));
                float newYvalue = Babycontroller.Neck.transform.localPosition.y + (0.003485635f * -5f * (RandomMutationAmount * maxMutationAmount));
                Babycontroller.Neck.transform.localPosition = new Vector3(Babycontroller.Neck.transform.localPosition.x, newYvalue, Babycontroller.Neck.transform.localPosition.z);
            }
            else
            {
                DeerController BabyDeerController = Babycontroller.GetComponent<DeerController>();
                //Set baby neck height number and actual height to parent
                BabyDeerController.NeckHeight = FemaleControl.GetComponent<DeerController>().NeckHeight;
                Babycontroller.Neck.transform.localPosition = new Vector3(Babycontroller.Neck.transform.localPosition.x, FemaleControl.Neck.transform.localPosition.y, Babycontroller.Neck.transform.localPosition.z);

                //Mutate baby neck height number and actual height by RandomMutationAmount
                BabyDeerController.NeckHeight = BabyDeerController.NeckHeight + (2 * -1 * (RandomMutationAmount * maxMutationAmount));
                float newYvalue = Babycontroller.Neck.transform.localPosition.y + (0.003485635f * -5f * (RandomMutationAmount * maxMutationAmount));
                Babycontroller.Neck.transform.localPosition = new Vector3(Babycontroller.Neck.transform.localPosition.x, newYvalue, Babycontroller.Neck.transform.localPosition.z);
            }
        }
        


        MaleController = null;
    }
    

    public void GiveCoyoteBirth(Animal MaleControl, Animal FemaleControl)
    {
        isPregnant = false;
        GameObject Baby;

        if(Random.value >= 0.5f)
        {
            Baby = Instantiate(MaleChildObj.prefab, transform.position, Quaternion.identity);
        }
        else
        {
            Baby = Instantiate(FemaleChildObj.prefab, transform.position, Quaternion.identity);
        }

        Animal Babycontroller = Baby.GetComponent<Animal>();


        //run speed
        if(Random.value >= 0.5f)
        {
                Babycontroller.DefaultRunSpeed = MaleControl.runSpeed + MaleControl.runSpeed * -1 * (ReturnRandom(-1f,1f) * maxMutationAmount);
        }
        else
        {
                Babycontroller.DefaultRunSpeed = FemaleControl.runSpeed + FemaleControl.runSpeed * -1 * (ReturnRandom(-1f,1f) * maxMutationAmount);
        }

        //Field of view radius
        if(Random.value >= 0.5f)
        {
                Babycontroller.FieldOfview.radius = MaleControl.FieldOfview.radius + MaleControl.FieldOfview.radius * -1 * (ReturnRandom(-1f,1f) * maxMutationAmount);
        }
        else
        {
                Babycontroller.FieldOfview.radius = FemaleControl.FieldOfview.radius + FemaleControl.FieldOfview.radius * -1 * (ReturnRandom(-1f,1f) * maxMutationAmount);
        }

        //Stamina

        if(Random.value >= 0.5f)
        {
            Babycontroller.SurvivalManager.maxStamina = MaleControl.SurvivalManager.maxStamina + MaleControl.SurvivalManager.maxStamina * -1 * (ReturnRandom(-1f,1f) * maxMutationAmount);
        }
        else
        {
            Babycontroller.SurvivalManager.maxStamina = FemaleControl.SurvivalManager.maxStamina + FemaleControl.SurvivalManager.maxStamina * -1 * (ReturnRandom(-1f,1f) * maxMutationAmount);
        }

        MaleController = null;
    }

    public float ReturnRandom(float min, float max)
    {
        return Random.Range((float)min, (float)max);
    }

    public IEnumerator RotateTowards(Transform target)
    {
        Vector3 HeartPos = Vector3.Lerp(Animal.Head.transform.position, target.GetComponent<Animal>().Head.transform.position, 0.5f);
        Instantiate(HeartsPrefab.prefab, HeartPos, Quaternion.identity);
        

        float mateTimer = 0f;
        float turn_speed = 0.1f;
        while(mateTimer < MateTime)
        {
            Quaternion _lookRotation = Quaternion.LookRotation((target.position - transform.position).normalized);
            transform.rotation = _lookRotation;
            //transform.LookAt(target);
            mateTimer += Time.deltaTime;
            
            // transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * turn_speed); 
        }

        yield return new WaitForSeconds(2);

        target.GetComponent<Animal>().state = State.Patrol;
        Animal.state = State.Patrol;
    }

    public void Rotate(Transform target)
    {
        
    }



    // public IEnumerator RotateTowards(Transform target)
    // {
    //     Vector3 HeartPos = Vector3.Lerp(AnimalController.Head.transform.position, target.GetComponent<AnimalController>().Head.transform.position, 0.5f);
    //     //HeartPos = new Vector3(HeartPos.x, HeartPos.y + 2f, HeartPos.z);
    //     Instantiate(HeartsPrefab.prefab, HeartPos, Quaternion.identity);
        

    //     float timeToRotate = 2f;
    //     var t = 0f;
    //     while(t < 1)
    //     {
    //         t += Time.deltaTime / timeToRotate;
    //         transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Inverse(target.rotation), t);
    //         yield return null;
    //     }

    //     yield return new WaitForSeconds(2);

    //     target.GetComponent<AnimalController>().state = State.Patrol;
    //     AnimalController.state = State.Patrol;
    // }
}




