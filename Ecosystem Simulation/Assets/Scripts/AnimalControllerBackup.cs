// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.AI;

// public enum EnemyType {Coyote, Deer};
// public enum State {None, Chase, Patrol, Attack, WalkToReproduce, WalkToDrink, Drinking, Searching, Mating, RunningAway}
// public enum SearchState{None, SearchingFood, SearchingDrink, SearchingMate}


// public class AnimalController : MonoBehaviour
// {
//     [Header("Animal States")]
//     public EnemyType Enemytype;
//     public SearchState searchState;
//     public State state;

//     [Header("DefaultStats")]
//     public float DefaultWalkSpeed;
//     public float DefaultRunSpeed;

//     [Header("State Reach Distances")]
//     public float chaseDist;
//     public float attackDist;
//     public float RequestMateDist;
//     public float DrinkWaterDist;

//     [Header("State Durations")]
//     public float DrinkDuration;
//     public float MateDuration;
//     public float walkSpeed;
//     public float runSpeed;

    
    
    





//     public float patrolMinRadius;
//     public float patrolMaxRadius;
//     public float patrolForThisTime;

//     //Priate Variables
//     private float patrolTimer;

//     private float MateTimer;
//     private float currentChaseDist;
//     private bool attackedThisTime;
//     private NavMeshAgent Agent;
//     private Animator myAnim;

//     private FieldOfView FieldOfview;
//     private SurvivalManager SurvivalManager;
//     private AgeController AgeController;

//     public List<Transform> KnownWaterLocations = new List<Transform>();
//     public int RememberWaterLocationsCount;
//     private Transform ClosestWater;

//     public Transform Head;

    
//     void Awake()
//     {
//         Agent = GetComponent<NavMeshAgent>();
//         AgeController = GetComponent<AgeController>();
//         myAnim = GetComponent<Animator>();
//         FieldOfview = GetComponent<FieldOfView>();
//         SurvivalManager = GetComponent<SurvivalManager>();
//     }

    
//     void Start()
//     {
//         state = State.Patrol;
//         patrolTimer = patrolForThisTime;
//         MateTimer = MateDuration;

//         if(Enemytype == EnemyType.Coyote)
//         {
//             GameController.instance.AddCoyoteToOverall();
//             GameController.instance.DailyBornCoyotes++;
//             GameController.instance.InsertIndividuaCoyoteData(DefaultRunSpeed, FieldOfview.radius, SurvivalManager.maxStamina, this.gameObject);
//         }

//         currentChaseDist = chaseDist;

//     }

//     // Update is called once per frame
//     void Update()
//     {
       
//         switch(state)
//         {
//             case State.Patrol:
//             Patrol();
//             break;

//             case State.Chase:
//             Chase();
//             break;

//             case State.Attack:
//             Attack();
//             break;

//             case State.WalkToReproduce:
//             WalkToReproduce();
//             break;

//             case State.WalkToDrink:
//             WalkToDrink();
//             break;

//             case State.Searching:
//             break;

//             case State.Mating:
//             Mating();
//             break;
//         }

//         switch(searchState)
//         {
//             case SearchState.SearchingFood:
//             SearchFood();
//             break;

//             case SearchState.SearchingDrink:
//             SearchDrink();
//             break;

//             case SearchState.SearchingMate:
//             SearchMate();
//             break;
//         }


//         //if is hungry, go search food
//         if(SurvivalManager.isHungry && state != State.Chase && state != State.Attack && state != State.WalkToReproduce && state != State.WalkToDrink && state != State.Mating && searchState != SearchState.SearchingDrink && state != State.Drinking)
//         {
//             searchState = SearchState.SearchingFood;  
//         }

//         // if thursty, and not chasing, attacking, searching food, Drinking, Walking to drink, Walking to Reproduce, Mating
//         else if(SurvivalManager.isThursty && state != State.Chase && state != State.Attack && searchState != SearchState.SearchingFood && state != State.Drinking && state != State.WalkToDrink && state != State.WalkToReproduce && state != State.Mating)
//         {
//             searchState = SearchState.SearchingDrink;  
//         }

//         //if needs to reproduce and not hungry or thursty, go search female
//         else if(SurvivalManager.needsReproduce && state != State.WalkToReproduce && state != State.Chase && state != State.Attack && searchState != SearchState.SearchingFood && state != State.Drinking && state != State.WalkToDrink && searchState != SearchState.SearchingFood && state != State.Mating)
//         {
//             searchState = SearchState.SearchingMate;
//         }
        
//     }
//     public void Patrol()
//     {
//         FieldOfview.ChoosenObject = null;
//         Walk(true);        

//         if(chaseDist != currentChaseDist)
//         {
//             chaseDist = currentChaseDist;
//         }
//     }


//     public void Walk(bool withStops)
//     {
//         myAnim.SetBool("Run", false);
//         UnStopAgent(walkSpeed);
//         patrolTimer += Time.deltaTime;
//         float dist = Agent.remainingDistance;
        
//         if(withStops)
//         {
//             if(patrolTimer > patrolForThisTime)
//                 {
//                     SetNewRandomDestination();
//                     patrolTimer = 0;
//                 }
//         }
//         else
//         {
//             if (dist != Mathf.Infinity && Agent.pathStatus==NavMeshPathStatus.PathComplete && Agent.remainingDistance == 0)
//                 {
//                     SetNewRandomDestination();
//                     patrolTimer = 0;
//                 } 
//         }

//         if(Agent.velocity.sqrMagnitude > 0)
//         {
//             myAnim.SetBool("Walk", true);
//             myAnim.SetBool("Idle", false);
//         }
//         else
//         {
//             myAnim.SetBool("Walk", false);
//             myAnim.SetBool("Idle", true);
//         }
//     }
//     public void SearchFood()
//     {
//         state = State.Searching;
//         FieldOfview.searchObject = SearchObject.Food;

//         if(FieldOfview.ChoosenObject != null && SurvivalManager.staminaPercent > 0.92f)
//         {
//             searchState = SearchState.None;
//             state = State.Chase;
//         }
//         else
//         {
//             Walk(false);
//         }
//     }

//     public void SearchDrink()
//     {
//         state = State.Searching;
//         FieldOfview.searchObject = SearchObject.Drink;

//         //If does not see any water
//         if(FieldOfview.ChoosenObject != null && state != State.Drinking)
//         {
//             state = State.WalkToDrink;
//         }

//         //If remembers where is water
//         else if(KnownWaterLocations.Count > 0)
//         {
//             float closestEnemy = Mathf.Infinity;
            
//             for (int i = 0; i < KnownWaterLocations.Count; i++)
//             {
//                 float distanceToTarget = Vector3.Distance(transform.position, KnownWaterLocations[i].transform.position);
//                 if (distanceToTarget < closestEnemy)
//                 {
//                     closestEnemy = distanceToTarget;
//                     ClosestWater = KnownWaterLocations[i].transform;
//                 } 
//             }
//             state = State.WalkToDrink;
//         }

//         //if doesnt see water, and does not remember
//         else
//         {
//             Walk(false);
//         }
//     }
//     public void SearchMate()
//     {
//         state = State.Searching;
//         FieldOfview.searchObject = SearchObject.Reproduce;

//         if(FieldOfview.ChoosenObject != null)
//         {
//             state = State.WalkToReproduce;
//         }
//         else
//         {
//             Walk(false);
//         } 
//     }

//     public void WalkToReproduce()
//     {   
//         UnStopAgent(walkSpeed);

//         searchState = SearchState.None;

//         myAnim.SetBool("Walk", true);
//         myAnim.SetBool("Run", false);
        
//         Agent.SetDestination(FieldOfview.ChoosenObject.transform.position);
            
//         if(Vector3.Distance(transform.position, FieldOfview.ChoosenObject.transform.position) <= RequestMateDist)
//         {   
//             StopAgent();
//             myAnim.SetBool("Walk", false);
//             if(FieldOfview.ChoosenObject.GetComponent<ReproductionManager>())
//             {
//                 if(FieldOfview.ChoosenObject.GetComponent<ReproductionManager>().RequestReproduction(this))
//                 {
//                     //state = State.Mating;
//                     SurvivalManager.currentReproductionUrge = 0;
                    
//                     FieldOfview.ChoosenObject = null;
//                     //state = State.Patrol;
//                     searchState = SearchState.None;
//                 }
//                 else
//                 {
                    
//                     state = State.Patrol;
//                     searchState = SearchState.None;
//                 }
//             }  
//         }
//     }

    

//     public void WalkToDrink()
//     {   
//         UnStopAgent(walkSpeed);

//         searchState = SearchState.SearchingDrink;

//         myAnim.SetBool("Walk", true);
//         myAnim.SetBool("Run", false);
//         myAnim.SetBool("Idle", false);
        
//         if(ClosestWater != null && FieldOfview.ChoosenObject == null)
//         {
//             Agent.SetDestination(ClosestWater.transform.position);

//             if(Vector3.Distance(transform.position, ClosestWater.transform.position) <= DrinkWaterDist)
//             {
//                 SurvivalManager.RegenThurst();
//                 state = State.Drinking;
//                 StopAgent();
//                 StartCoroutine(Drink());
//                 searchState = SearchState.None;
//                 myAnim.SetBool("Drink", true);  
//             }
//         }
//         else
//         {
//             Agent.SetDestination(FieldOfview.ChoosenObject.transform.position);

//             if(Vector3.Distance(transform.position, FieldOfview.ChoosenObject.transform.position) <= DrinkWaterDist)
//             {
//                 SurvivalManager.RegenThurst();
//                 state = State.Drinking;
//                 StopAgent();
//                 StartCoroutine(Drink());
//                 searchState = SearchState.None;
//                 myAnim.SetBool("Drink", true);  
//             }
//         }
        
//     }

//     public IEnumerator Drink()
//     {
//         yield return new WaitForSeconds(DrinkDuration);

//         if(KnownWaterLocations.Count < RememberWaterLocationsCount)
//         {
//             if(!KnownWaterLocations.Contains(FieldOfview.ChoosenObject.transform))
//             {
//                 KnownWaterLocations.Add(FieldOfview.ChoosenObject.transform);
//             }
                
//         }
//         else
//         {
//             if(!KnownWaterLocations.Contains(FieldOfview.ChoosenObject.transform))
//             {
//                 KnownWaterLocations.RemoveAt(0);
//                 KnownWaterLocations.Add(FieldOfview.ChoosenObject.transform);
//             }
//         }
        
//         FieldOfview.searchObject = SearchObject.None;
//         state = State.Patrol;
//         ClosestWater = null;
//         myAnim.SetBool("Drink", false);
//     }

//     public void Chase()
//     {
//         UnStopAgent(runSpeed);

//         searchState = SearchState.None;

//         myAnim.SetBool("Walk", false);
//         myAnim.SetBool("Run", Agent.velocity.sqrMagnitude > 0 ? true : false);
                
//         Agent.SetDestination(FieldOfview.ChoosenObject.transform.position);

//         if(Vector3.Distance(transform.position, FieldOfview.ChoosenObject.transform.position) <= attackDist)
//         {      
//             state = State.Attack;
//             if(chaseDist != currentChaseDist)
//             {
//                 chaseDist = currentChaseDist;
//             }
//         }
//     }

//     public void Attack()
//     {
//         StopAgent();
//         if(!attackedThisTime)
//         {
//             SurvivalManager.RegenHealth();
//             myAnim.SetTrigger("Attack");
//             myAnim.SetBool("Run", false);
//             myAnim.SetBool("Walk", false);
//             myAnim.SetBool("Idle", true);
//             StartCoroutine(WaitAfterAttack());
//             attackedThisTime = true;
//         }  
//     }

    

//     public void OnStaminaExpired()
//     {
//         state = State.Patrol;
//         searchState = SearchState.None;
//         myAnim.SetBool("Run", false);  
//     }

//     void Mating()
//     {
//         StopAgent();
//         myAnim.SetBool("Run", false);
//         myAnim.SetBool("Walk", false);
//         myAnim.SetBool("Idle", true);
//         // MateTimer -= Time.deltaTime;
//         // if(MateTimer <= 0)
//         // {
            
//         // }
//     }

    

//     public IEnumerator WaitAfterAttack()
//     {
//         yield return new WaitForSeconds(4f);
//         FieldOfview.searchObject = SearchObject.None;
//         state = State.Patrol;
//         attackedThisTime = false;
//     }

//     public void SetNewRandomDestination()
//     {
//         float randomRadius = Random.Range(patrolMinRadius, patrolMaxRadius);
//         Vector3 randomDir = Random.insideUnitSphere * randomRadius;
//         randomDir += transform.position;
//         NavMeshHit navHit;
//         NavMesh.SamplePosition(randomDir, out navHit, randomRadius, -1);
//         Agent.SetDestination(navHit.position);
//     }
//     public bool CheckReachedDestination()
//     {
//         float dist = Agent.remainingDistance;
//         if(dist != Mathf.Infinity && Agent.pathStatus==NavMeshPathStatus.PathComplete && Agent.remainingDistance == 0)
//         {
//             return true;
//         }
//         else
//         {
//             return false;
//         }
//     }
//     public void OnDead()
//     {
//         GameController.instance.RemoveIndividulData(this.gameObject);
//         GameController.instance.RemoveTreeFromOverall();
//         GameController.instance.DailyDiedCoyotes++;
//         Destroy(gameObject);
//     }
//     public void StopAgent()
//     {
//         Agent.isStopped = true;
//         Agent.velocity = Vector3.zero;
//         //Agent.speed = 0;
//     }
//     public void UnStopAgent(float Speed)
//     {
//         Agent.isStopped = false;
//         Agent.speed = Speed;
//     }


    
// }









