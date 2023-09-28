using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DeerController : Animal
{
    public float NeckHeight;
    private bool _ateThisTime;
    [SerializeField ] private float _eatingDistance;
    [SerializeField ] private float _eatingDuration;

    public bool isBeingChased;
    private bool pathReseted;
    public float startFleeDistance;
     [SerializeField] private float _fleeDistance;
    [SerializeField] private float _additionalFleeTime;

    [SerializeField] private float _FleeThisTime;
    [SerializeField] private float _FleeTimer;

    

    public Transform Chaser;
    private Transform tempChaser;
    void Awake()
    {
        base.Awake();
    }
    void Start()
    {
        base.Start();

        GameController.instance.AddDeerToOverall();
        GameController.instance.DailyBornDeers++;
        GameController.instance.InsertIndividualDeerData(DefaultRunSpeed, FieldOfview.radius, SurvivalManager.maxStamina, NeckHeight, startFleeDistance, this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        

        switch(state)
        {
            case State.WalkToEat:
            WalkToEat();
            break;

            case State.Flee:
            Flee();
            break;
        }
        
        base.Update();

        //if being chased just run
        if(isBeingChased && state != State.GettingSlapped)
        {   
            float distance = Vector3.Distance(Chaser.position, transform.position);

            if(distance <= startFleeDistance)
            {   
                if(!pathReseted)
                {
                    pathReseted = true;
                    Agent.ResetPath();
                }
               
                
                state = State.Flee;
                searchState = SearchState.None;
            }
        }
        else
        {
            pathReseted = false;
        }

       
    }

    public void Flee()
    {
        myAnim.SetBool("Walk", false);
        myAnim.SetBool("Drink", false);
        myAnim.SetBool("Eat", false);
        myAnim.SetBool("Run", true);

        UnStopAgent(runSpeed);
        
       
        
        if(Chaser != null)
        {   
             _FleeTimer += Time.deltaTime;
            if(_FleeTimer > _FleeThisTime || CheckReachedDestination())
            {
                SetNewRandomFleeDestination();
                _FleeTimer = 0;
            }

            
            tempChaser = Chaser;
        }
        else
        {
            
            // float _additionalFleeTimer = 0;

            // while(_additionalFleeTimer < _additionalFleeTime)
            // {
            //     Debug.Log("_additionalFleeTimer is " + _additionalFleeTimer + " and _additionalFleeTime is " + _additionalFleeTime);
            //     Vector3 normDir = (tempChaser.position - transform.position).normalized;
            //     _additionalFleeTimer += Time.deltaTime;
            //     Agent.SetDestination(transform.position - normDir);
            // }
            //_additionalFleeTimer = 0;
            state = State.Patrol;
        }
    }

    public void FleeAdditional()
    {

    }

    public void SetNewRandomFleeDestination()
    {
        Vector3 normDir = (Chaser.position - transform.position).normalized;
        normDir = Quaternion.AngleAxis(Random.Range(-70, 70), Vector3.up) * normDir;
        Vector3 TargetDirection = transform.position - (normDir * _fleeDistance);
       
        NavMeshHit navHit;
        NavMesh.SamplePosition(TargetDirection, out navHit, _fleeDistance, -1);
        Agent.SetDestination(navHit.position);
    }


    public void WalkToEat()
    {   
        UnStopAgent(walkSpeed);

        searchState = SearchState.None;

        myAnim.SetBool("Walk", true);
        myAnim.SetBool("Run", false);
        
        if(FieldOfview.ChoosenObject != null)
        {
            Agent.SetDestination(FieldOfview.ChoosenObject.transform.position);
            if(Vector3.Distance(transform.position, FieldOfview.ChoosenObject.transform.position) <= _eatingDistance)
            {   
                IEnumerator eat = Eat();
                StopAgent();
                myAnim.SetBool("Walk", false);
                if(FieldOfview.ChoosenObject.GetComponent<TreeController>())
                {
                    StopAgent();
                    myAnim.SetBool("Eat", true);
                    state = State.Eating;
                    searchState = SearchState.None;
                    SurvivalManager.RegenHealth();
                    StartCoroutine(eat);
                    
                      
                }
                else
                {
                    StopCoroutine(eat);
                    myAnim.SetBool("Eat", false);
                    state = State.Patrol;
                    searchState = SearchState.None;
                }
            }  
        }
        else
        {
            state = State.Patrol;
            searchState = SearchState.None;
        }
    }
        
            
        
    

    public override void SearchFood()
    {
        state = State.Searching;
        FieldOfview.searchObject = SearchObject.Tree;

        if(FieldOfview.ChoosenObject != null)
        {
            searchState = SearchState.None;
            state = State.WalkToEat;
        }
        else
        {
            Walk(false);
        }
    }

    public IEnumerator WaitAfterEating()
    {
        yield return new WaitForSeconds(2.5f);
        FieldOfview.searchObject = SearchObject.None;
        state = State.Patrol;
        _ateThisTime = false;
    }

    public IEnumerator Eat()
    {   
        if(FieldOfview.ChoosenObject.GetComponent<TreeController>())
        {
            FieldOfview.ChoosenObject.GetComponent<TreeController>().BeEaten();
        }
        yield return new WaitForSeconds(_eatingDuration);
        FieldOfview.searchObject = SearchObject.None;
        state = State.Patrol;
        Agent.ResetPath();
        myAnim.SetBool("Eat", false);
    }

    public override void OnStaminaExpired()
    {
        state = State.Patrol;
        searchState = SearchState.None;
        myAnim.SetBool("Run", false);  
    }

    public void SetChaser(Transform _ChaseTransform)
    {
        Debug.Log("set chaser was called");
        isBeingChased = true;
        Chaser = _ChaseTransform;
    }

    public void UnsetChaser()
    {
        isBeingChased = false;
        Chaser = null;
    }

    public void BeEaten()
    {   
        StopAgent();
        myAnim.enabled = false;
        state = State.GettingSlapped;
        Quaternion _lookRotation = Quaternion.LookRotation((Chaser.position - transform.position).normalized);
        transform.rotation = _lookRotation;
        StartCoroutine(DelayOnDead());
    }

    public IEnumerator DelayOnDead()
    {
        yield return new WaitForSeconds(2);
        OnDead();
    }

    

    public override void OnDead()
    {
        GameController.instance.RemoveIndividualDeerData(this.gameObject);
        GameController.instance.RemoveDeerFromOverall();
        GameController.instance.DailyDiedDeers++;
        Destroy(gameObject);
    }
}
