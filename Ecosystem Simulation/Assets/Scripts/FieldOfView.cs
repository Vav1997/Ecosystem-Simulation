using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SearchObject{None, Deer, Tree, Drink, Reproduce};
public class FieldOfView : MonoBehaviour
{
    public float DefaultRadius;
    public float radius;
    [Range(0,360)]
    public float angle;

    public SearchObject searchObject;


    public LayerMask targetMask;
    public LayerMask EatableMask;
    public LayerMask DrinkableMask;
    public LayerMask ReproductionMask;
    
    public LayerMask obstructionMask;

    public GameObject ChoosenObject;
    public bool canSeePlayer;

    public ReproductionManager ReproductionManager;

    private void Start()
    {
        ReproductionManager = GetComponent<ReproductionManager>();

        StartCoroutine(FOVRoutine());
    }

    public void Update()
    {

    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
       

        switch (searchObject)
        {
            case SearchObject.Deer:
            
            targetMask = EatableMask;

            Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

            if (rangeChecks.Length != 0)
            {
                Transform target = rangeChecks[0].transform;
                Vector3 directionToTarget = (target.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
                {
                    float closestEnemy = Mathf.Infinity;

                    for (int i = 0; i < rangeChecks.Length; i++)
                    {
                        float distanceToTarget = Vector3.Distance(transform.position, rangeChecks[i].transform.position);
                        if (distanceToTarget < closestEnemy)
                        {
                            closestEnemy = distanceToTarget;
                            ChoosenObject = rangeChecks[i].transform.gameObject;
                            canSeePlayer = true;
                        }
                    }
                }
                else
                {
                    ChoosenObject = null;
                    canSeePlayer = false;
                }     
            }
            else if (canSeePlayer)
            {
                ChoosenObject = null;
                canSeePlayer = false;
            }
            
            break;

            case SearchObject.Drink:

            targetMask = DrinkableMask;
            
            Collider[] DrinkableRangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

            if (DrinkableRangeChecks.Length != 0)
            {
                Transform target = DrinkableRangeChecks[0].transform;
                Vector3 directionToTarget = (target.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
                {
                    float closestEnemy = Mathf.Infinity;

                    for (int i = 0; i < DrinkableRangeChecks.Length; i++)
                    {
                        float distanceToTarget = Vector3.Distance(transform.position, DrinkableRangeChecks[i].transform.position);
                        if (distanceToTarget < closestEnemy)
                        {
                            closestEnemy = distanceToTarget;

                            if(!DrinkableRangeChecks[i].transform.gameObject.GetComponent<Water>().isBeingDrinken)
                            {
                                ChoosenObject = DrinkableRangeChecks[i].transform.gameObject;
                                canSeePlayer = true;
                            }
                            
                        }
                    }
                }
                else
                {
                    ChoosenObject = null;
                    canSeePlayer = false;
                }     
            }
            else if (canSeePlayer)
            {
                ChoosenObject = null;
                canSeePlayer = false;
            }
            
            break;

            case SearchObject.Reproduce:

            targetMask = ReproductionMask;
            
            Collider[] ReproduceRangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

            if (ReproduceRangeChecks.Length != 0)
            {
                Transform target = ReproduceRangeChecks[0].transform;
                Vector3 directionToTarget = (target.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
                {
                    float closestEnemy = Mathf.Infinity;

                    for (int i = 0; i < ReproduceRangeChecks.Length; i++)
                    {
                        float distanceToTarget = Vector3.Distance(transform.position, ReproduceRangeChecks[i].transform.position);
                        if (distanceToTarget < closestEnemy)
                        {
                            closestEnemy = distanceToTarget;
                            if(!ReproduceRangeChecks[i].transform.gameObject.GetComponent<ReproductionManager>().isPregnant && ReproduceRangeChecks[i].transform.gameObject.GetComponent<AgeController>().AgeState == Ages.Adult
                            && !ReproductionManager.RegectedFemalesList.Contains(ReproduceRangeChecks[i].transform.gameObject))
                            {
                                ChoosenObject = ReproduceRangeChecks[i].transform.gameObject;
                                canSeePlayer = true;
                            }
                            
                        }
                    }
                }
                else
                {
                    ChoosenObject = null;
                    canSeePlayer = false;
                }     
            }
            else if (canSeePlayer)
            {
                ChoosenObject = null;
                canSeePlayer = false;
            }
            
            break;

            case SearchObject.Tree:

            targetMask = EatableMask;
            
            Collider[] TreeRangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

            if (TreeRangeChecks.Length != 0)
            {
                Transform target = TreeRangeChecks[0].transform;
                Vector3 directionToTarget = (target.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
                {
                    float closestEnemy = Mathf.Infinity;

                    for (int i = 0; i < TreeRangeChecks.Length; i++)
                    {
                        float distanceToTarget = Vector3.Distance(transform.position, TreeRangeChecks[i].transform.position);
                        if (distanceToTarget < closestEnemy)
                        {
                            closestEnemy = distanceToTarget;
                            TreeController treeController = TreeRangeChecks[i].transform.gameObject.GetComponent<TreeController>();
                            if(!treeController._isBeingEaten  || (treeController._isBeingEaten && treeController.leafesList.Count > 0))
                            {
                                if(treeController.TreeHeight <= GetComponent<DeerController>().NeckHeight)
                                {
                                    ChoosenObject = TreeRangeChecks[i].transform.gameObject;
                                    canSeePlayer = true;
                                }
                            }
                            treeController = null;
                        }
                    }
                }
                else
                {
                    ChoosenObject = null;
                    canSeePlayer = false;
                }     
            }
            else if (canSeePlayer)
            {
                ChoosenObject = null;
                canSeePlayer = false;
            }
            
            break;

            // case SearchObject.None:
            // // ChoosenObject = null;
            // // canSeePlayer = false;
            // // targetMask = LayerMask.GetMask("Nothing");
            // break;
            
        }
        
       
            
    }


    public void SearchFood(CoyoteController CoyoteController)
    {
        targetMask = EatableMask;

            Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);
            
            if (rangeChecks.Length != 0)
            {
                Transform target = rangeChecks[0].transform;
                Vector3 directionToTarget = (target.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
                {
                    float closestEnemy = Mathf.Infinity;

                    for (int i = 0; i < rangeChecks.Length; i++)
                    {
                        float distanceToTarget = Vector3.Distance(transform.position, rangeChecks[i].transform.position);
                        if (distanceToTarget < closestEnemy)
                        {
                            closestEnemy = distanceToTarget;
                            ChoosenObject = rangeChecks[i].transform.gameObject;
                            canSeePlayer = true;
                        }
                    }
                }
                else
                {
                    ChoosenObject = null;
                    canSeePlayer = false;
                }     
            }
            else if (canSeePlayer)
            {
                ChoosenObject = null;
                canSeePlayer = false;
            }
    }
}