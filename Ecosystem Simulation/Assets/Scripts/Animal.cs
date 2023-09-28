using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum State {None, Chase, Patrol, Attack, WalkToReproduce, WalkToDrink, WalkToEat, Drinking, Eating, Searching, Mating, Flee, GettingSlapped}
public enum SearchState{None, SearchingFood, SearchingDrink, SearchingMate}

public abstract class Animal : MonoBehaviour
{
    public SearchState searchState;
    public State state;

    [Header("DefaultStats")]
    public float DefaultWalkSpeed;
    public float DefaultRunSpeed;
    public float walkSpeed;
    public float runSpeed;

    [Header("State Reach Distances")]
    public float RequestMateDist;
    public float DrinkWaterDist;

    [Header("State Durations")]
    public float DrinkDuration;
    public float MateDuration;

    [HideInInspector]
    public float _patrolTimer;

    public float MateTimer;
    
    public float patrolMinRadius;
    public float patrolMaxRadius;
    public float patrolForThisTime;


    public Transform Head;
    public GameObject Neck;

    public NavMeshAgent Agent;
    public Animator myAnim;

    [HideInInspector]
    public FieldOfView FieldOfview;
    public SurvivalManager SurvivalManager;
    public AgeController AgeController;
    public ReproductionManager ReproductionManager;

    public List<Transform> KnownWaterLocationsList = new List<Transform>();
    public int RememberWaterLocationsCount;

    [HideInInspector]
    public Transform ClosestWater;


    public void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        AgeController = GetComponent<AgeController>();
        myAnim = GetComponent<Animator>();
        FieldOfview = GetComponent<FieldOfView>();
        SurvivalManager = GetComponent<SurvivalManager>();
        ReproductionManager = GetComponent<ReproductionManager>();
    }
    public void Start()
    {
        state = State.Patrol;
        _patrolTimer = patrolForThisTime;
        MateTimer = MateDuration;
    }

    
    public void Update()
    {
           
        switch(state)
        {
            case State.Patrol:
            Patrol();
            break;

            case State.WalkToReproduce:
            WalkToReproduce();
            break;

            case State.WalkToDrink:
            WalkToDrink();
            break;

            case State.Searching:
            break;

            case State.Mating:
            Mating();
            break;
        }

        switch(searchState)
        {
            case SearchState.SearchingFood:
            SearchFood();
            break;

            case SearchState.SearchingDrink:
            SearchDrink();
            break;

            case SearchState.SearchingMate:
            SearchMate();
            break;
            // case SearchState.None:
            // FieldOfview.searchObject = SearchObject.None;
            // break;
        }

        //if needs to reproduce and not hungry or thursty, go search female
        if(SurvivalManager.needsReproduce && state == State.Patrol)
        {
            searchState = SearchState.SearchingMate;
        }
        //if is hungry, go search food
        else if(SurvivalManager.isHungry && state == State.Patrol)
        {
            searchState = SearchState.SearchingFood;  
        }

        // if thursty, and not chasing, attacking, searching food, Drinking, Walking to drink, Walking to Reproduce, Mating
        else if(SurvivalManager.isThursty && state == State.Patrol)
        {
            searchState = SearchState.SearchingDrink;  
        }

        
        
    }

    public virtual void Patrol()
    {
        FieldOfview.ChoosenObject = null;
        Walk(true);        
    }


    public void Walk(bool withStops)
    {
        myAnim.SetBool("Run", false);
        myAnim.SetBool("Drink", false);
        UnStopAgent(walkSpeed);
        _patrolTimer += Time.deltaTime;
        float dist = Agent.remainingDistance;
        
        if(withStops)
        {
            if(_patrolTimer > patrolForThisTime)
            {
                SetNewRandomDestination();
                _patrolTimer = 0;
            }
            if (dist != Mathf.Infinity && Agent.pathStatus==NavMeshPathStatus.PathComplete && Agent.remainingDistance == 0)
            {
                myAnim.SetBool("Walk", false);
                myAnim.SetBool("Idle", true);
            }
            else
            {
                myAnim.SetBool("Walk", true);
                myAnim.SetBool("Idle", false);
            }
        }
        else
        {
            myAnim.SetBool("Walk", true);
            myAnim.SetBool("Idle", false);

            if (dist != Mathf.Infinity && Agent.pathStatus==NavMeshPathStatus.PathComplete && Agent.remainingDistance == 0)
            {
                SetNewRandomDestination();
                _patrolTimer = 0;
            } 
        }

        // if(Agent.velocity.sqrMagnitude > 0)
        // {
        //     myAnim.SetBool("Walk", true);
        //     myAnim.SetBool("Idle", false);
        // }
        // else
        // {
        //     myAnim.SetBool("Walk", false);
        //     myAnim.SetBool("Idle", true);
        // }
    }
    public virtual void SearchFood()
    {
        state = State.Searching;
        FieldOfview.searchObject = SearchObject.Deer;

        if(FieldOfview.ChoosenObject != null && SurvivalManager.staminaPercent > 0.92f)
        {
            searchState = SearchState.None;
            state = State.Chase;
        }
        else
        {
            Walk(false);
        }
    }

    public void SearchDrink()
    {
        state = State.Searching;
        FieldOfview.searchObject = SearchObject.Drink;

        //If does not see any water
        if(FieldOfview.ChoosenObject != null && state != State.Drinking)
        {
            state = State.WalkToDrink;
        }
        else
        {
            Walk(false);
        }
    }
    public void SearchMate()
    {
        state = State.Searching;
        FieldOfview.searchObject = SearchObject.Reproduce;

        if(FieldOfview.ChoosenObject != null)
        {
            state = State.WalkToReproduce;
        }
        else
        {
            Walk(false);
        } 
    }

    public void WalkToReproduce()
    {   
        UnStopAgent(walkSpeed);

        searchState = SearchState.None;
        myAnim.SetBool("Idle", false);
        myAnim.SetBool("Drink", false);
        myAnim.SetBool("Walk", true);
        myAnim.SetBool("Run", false);
        if(FieldOfview.ChoosenObject != null)
        {
            Agent.SetDestination(FieldOfview.ChoosenObject.transform.position);
            
            if(Vector3.Distance(transform.position, FieldOfview.ChoosenObject.transform.position) <= RequestMateDist)
            {   
                StopAgent();
                myAnim.SetBool("Walk", false);
                if(FieldOfview.ChoosenObject.GetComponent<ReproductionManager>())
                {
                    if(FieldOfview.ChoosenObject.GetComponent<ReproductionManager>().RequestReproduction(this))
                    {
                        //state = State.Mating;
                        SurvivalManager.currentReproductionUrge = 0;
                        
                        FieldOfview.ChoosenObject = null;
                        //state = State.Patrol;
                        searchState = SearchState.None;
                    }
                    else
                    {
                        state = State.Patrol;
                        searchState = SearchState.None;
                    }
                }  
            }
        }
        else
        {
            state = State.Patrol;
            searchState = SearchState.None;
        }
        
    }

    public void WalkToDrink()
    {   
        UnStopAgent(walkSpeed);

        searchState = SearchState.None;

        myAnim.SetBool("Walk", true);
        myAnim.SetBool("Run", false);
        myAnim.SetBool("Idle", false);
        
        if(ClosestWater != null && FieldOfview.ChoosenObject == null)
        {
            Agent.SetDestination(ClosestWater.transform.position);

            if(Vector3.Distance(transform.position, ClosestWater.transform.position) <= DrinkWaterDist)
            {   
                if(FieldOfview.ChoosenObject.gameObject.GetComponent<Water>())
                {
                    FieldOfview.ChoosenObject.gameObject.GetComponent<Water>().isBeingDrinken = true;
                    SurvivalManager.RegenThurst();
                    state = State.Drinking;
                    StopAgent();
                    StartCoroutine(Drink());
                    searchState = SearchState.None;
                    myAnim.SetBool("Drink", true);  
                }
                
            }
        }
        else if (FieldOfview.ChoosenObject != null)
        {
            Agent.SetDestination(FieldOfview.ChoosenObject.transform.position);

            if(Vector3.Distance(transform.position, FieldOfview.ChoosenObject.transform.position) <= DrinkWaterDist)
            {
                SurvivalManager.RegenThurst();
                state = State.Drinking;
                StopAgent();
                StartCoroutine(Drink());
                searchState = SearchState.None;
                myAnim.SetBool("Drink", true);  
            }
        }
    }

    public IEnumerator Drink()
    {

        yield return new WaitForSeconds(DrinkDuration);

        FieldOfview.ChoosenObject.gameObject.GetComponent<Water>().isBeingDrinken = false;

        if(KnownWaterLocationsList.Count < RememberWaterLocationsCount)
        {
            if(!KnownWaterLocationsList.Contains(FieldOfview.ChoosenObject.transform))
            {
                KnownWaterLocationsList.Add(FieldOfview.ChoosenObject.transform);
            }  
        }
        else
        {
            if(!KnownWaterLocationsList.Contains(FieldOfview.ChoosenObject.transform) && KnownWaterLocationsList.Count > 0)
            {
                KnownWaterLocationsList.RemoveAt(0);
                KnownWaterLocationsList.Add(FieldOfview.ChoosenObject.transform);
            }
        }
        
        FieldOfview.searchObject = SearchObject.None;
        state = State.Patrol;
        ClosestWater = null;
        Agent.ResetPath();
        if(_patrolTimer > patrolForThisTime)
        {
            SetNewRandomDestination();
            _patrolTimer = 0;
        }
        myAnim.SetBool("Drink", false);
    }
    void Mating()
    {
        StopAgent();
        myAnim.SetBool("Run", false);
        myAnim.SetBool("Walk", false);
        myAnim.SetBool("Idle", true);
        // MateTimer -= Time.deltaTime;
        // if(MateTimer <= 0)
        // {
            
        // }
    }
    public void SetNewRandomDestination()
    {
        float randomRadius = Random.Range(patrolMinRadius, patrolMaxRadius);

        //new code
        Vector3 normDir = transform.forward.normalized;
        normDir = Quaternion.AngleAxis(Random.Range(-120, 120), Vector3.up) * normDir;
        Vector3 TargetDirection = transform.position + (normDir * randomRadius);
        NavMeshHit navHit;
        NavMesh.SamplePosition(TargetDirection, out navHit, randomRadius, -1);
        Agent.SetDestination(navHit.position);


        // Vector3 randomDir = Random.insideUnitSphere * randomRadius;
        // randomDir += transform.position;
        // NavMeshHit navHit;
        // NavMesh.SamplePosition(randomDir, out navHit, randomRadius, -1);
        // Agent.SetDestination(navHit.position);
    }
    public bool CheckReachedDestination()
    {
        float dist = Agent.remainingDistance;
        if(dist != Mathf.Infinity && Agent.pathStatus==NavMeshPathStatus.PathComplete && Agent.remainingDistance == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public virtual void OnDead()
    {
        
    }
    public void StopAgent()
    {
        Agent.isStopped = true;
        Agent.velocity = Vector3.zero;
        //Agent.speed = 0;
    }
    public void UnStopAgent(float Speed)
    {
        Agent.isStopped = false;
        Agent.speed = Speed;
    }

    public virtual void OnStaminaExpired(){ }
    
}
