using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class CoyoteController : Animal
{

    [Header("State Reach Distances")]
    public float chaseDist;
    public float attackDist;

    private float currentChaseDist;
    
    private bool _attackedThisTime;

    public GameObject ChoosenDeer;



    //Priate Variables
   

    

    void Awake()
    {
        base.Awake();
    }
    
    void Start()
    {
        base.Start();

        GameController.instance.AddCoyoteToOverall();
        GameController.instance.DailyBornCoyotes++;
        GameController.instance.InsertIndividuaCoyoteData(DefaultRunSpeed, FieldOfview.radius, SurvivalManager.maxStamina, this.gameObject);
        
        currentChaseDist = chaseDist;
    }

    // Update is called once per frame
    void Update()
    {
        

        switch(state)
        {
            case State.Chase:
            Chase();
            break;

            case State.Attack:
            Attack();
            break;
        }

        base.Update();
    }

    public override void Patrol()
    {
        FieldOfview.ChoosenObject = null;
        Walk(true);        

        if(chaseDist != currentChaseDist)
        {
            chaseDist = currentChaseDist;
        }
    }

    public void Chase()
    {
        UnStopAgent(runSpeed);

        searchState = SearchState.None;

        myAnim.SetBool("Walk", false);
        myAnim.SetBool("Run", Agent.velocity.sqrMagnitude > 0 ? true : false);
        
        if(ChoosenDeer != null)
        {
            Agent.SetDestination(ChoosenDeer.transform.position); 
            if(Vector3.Distance(transform.position, ChoosenDeer.transform.position) <= attackDist)
            {      
            state = State.Attack;
            if(chaseDist != currentChaseDist)
            {
                chaseDist = currentChaseDist;
            }
            } 
        }
        else
        {
            state = State.Patrol;
        }
    }

    public override void OnStaminaExpired()
    {
        if(ChoosenDeer != null)
        {
            ChoosenDeer.GetComponent<DeerController>().UnsetChaser();
            ChoosenDeer = null;
        }
        

        state = State.Patrol;
        searchState = SearchState.None;
        myAnim.SetBool("Run", false);
    }

    public void Attack()
    {
        StopAgent();
        if(!_attackedThisTime)
        {
            if(ChoosenDeer.GetComponent<DeerController>())
            {
                ChoosenDeer.GetComponent<DeerController>().BeEaten();
            }
            SurvivalManager.RegenHealth();
            myAnim.SetTrigger("Attack");
            myAnim.SetBool("Run", false);
            myAnim.SetBool("Walk", false);
            myAnim.SetBool("Idle", true);
            StartCoroutine(WaitAfterAttack());
            _attackedThisTime = true;
            ChoosenDeer = null;
        }  
    }

    public IEnumerator WaitAfterAttack()
    {
        yield return new WaitForSeconds(2.5f);
        FieldOfview.searchObject = SearchObject.None;
        state = State.Patrol;
        _attackedThisTime = false;
    }

    public override void SearchFood()
    {
        state = State.Searching;
        FieldOfview.searchObject = SearchObject.Deer;

        if(FieldOfview.ChoosenObject != null && ChoosenDeer == null && SurvivalManager.staminaPercent > 0.92f)
        {   
            ChoosenDeer = FieldOfview.ChoosenObject;
            Debug.Log("mtela search food state");
            ChoosenDeer.GetComponent<DeerController>().SetChaser(this.transform);
            searchState = SearchState.None;
            state = State.Chase;
        }
        else
        {
            Walk(false);
        }
    }

    public override void OnDead()
    {
        GameController.instance.RemoveIndividulData(this.gameObject);
        GameController.instance.RemoveCoyoteFromOverall();
        GameController.instance.DailyDiedCoyotes++;
        Destroy(gameObject);
    }

    
}









