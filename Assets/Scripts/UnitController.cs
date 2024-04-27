using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class UnitController : MonoBehaviour
{
    public enum BehaviourState
    {
        None,
        Idle,
        SearchingForFood,
        Eating,
        SearchingForDrink,
        Drinking,
        Wandering,
        Mating,
        Copulating
    }
    public BehaviourState CurrentBehaviourState
    {
        get { return currentBehaviourState; }
        set
        {
            currentBehaviourState = value;
            behaviurCounter = behaviourTimeLimit;
        }
    }
    public NavMeshAgent navMeshAgent;

    System.Random rand = new System.Random();

    [SerializeField] private BehaviourState currentBehaviourState = BehaviourState.None;
    [SerializeField] private float idleTime = 5f;
    [SerializeField] private float behaviourTimeLimit = 10f;

    private float behaviurCounter;
    private Unit unit;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        unit = GetComponent<Unit>();
    }
    // Start is called before the first frame update
    void Start()
    {
        CurrentBehaviourState = BehaviourState.None;
    }


    // Update is called once per frame
    void Update()
    {
        behaviurCounter -= Time.deltaTime;

        LifeCycleStatusCheck();

        ExecuteBehaviour();
        DeprecatedBehaviour();
    }

    private void ExecuteBehaviour()
    {
        switch(CurrentBehaviourState)
        {
            case BehaviourState.None:
                FindeActivity();
                break;
            case BehaviourState.Idle:
                behaviurCounter -= Time.deltaTime;
                if(behaviurCounter <= 0)
                {
                    FindeActivity();
                }
                break;
            case BehaviourState.Wandering:
                if(BehaviourDestintaionReached())
                {
                    FindeActivity();
                }
                break;
            case BehaviourState.SearchingForFood:
                if (BehaviourDestintaionReached())
                {
                    FindeActivity();
                }
                break;
            case BehaviourState.Eating:
                if(unit.targetedTransform != null)
                {
                    unit.targetedTransform.TryGetComponent<Food>(out Food currnetFood);
                    if (currnetFood != null && unit.Hunger > 1)
                    {
                        currnetFood.Eat(unit);
                        break;
                    }
                }
                currentBehaviourState = BehaviourState.None;
                break;
            case BehaviourState.SearchingForDrink:
                if (BehaviourDestintaionReached())
                {
                    FindeActivity();
                }
                break;
            case BehaviourState.Drinking:
                if (unit.targetedTransform != null)
                {
                    unit.targetedTransform.TryGetComponent<Drink>(out Drink currnetDrink);
                    if (currnetDrink != null && unit.Thirst > 1)
                    {
                        currnetDrink.Drinking(unit);
                        break;
                    }
                }
                currentBehaviourState = BehaviourState.None;
                break;
            case BehaviourState.Mating:
                if(unit.Gens.IsFemale)
                {
                    break;
                }
                else
                {
                    if (BehaviourDestintaionReached())
                    {
                        if (CheckForValidPartner())
                        {
                            Copulating();
                        }
                        else
                        {
                            FindeActivity();
                        }
                    }
                }
                break;
            case BehaviourState.Copulating:
                
                break;
        }
    }

    private bool BehaviourDestintaionReached()
    {
        if (!navMeshAgent.pathPending && navMeshAgent.isOnNavMesh)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void DeprecatedBehaviour()
    {
        if(behaviurCounter <= 0)
        {
            Wandering();
        }
    }

    private void FindeActivity()
    {
        // Critical behaviuors
        if (unit.IsHungry || unit.IsThirsty)
        {
            if(unit.Hunger > unit.Thirst)
            {
                if (!LookForFood())
                {
                    if (!LookForDrink())
                    {
                        Wandering();
                    }
                }
            }
            else
            {
                if (!LookForDrink())
                {
                    if (!LookForFood())
                    {
                        Wandering();
                    }
                }

            }
            
        }
        // Secondary behaviours
        else if (unit.Urge >= unit.Gens.UrgeTreshold && unit.IsAdult)
        {
            lookForValidMatingPartners();
        }
        // 50% chance for ddle behaviours
        else
        {
            if (rand.NextDouble() > 0.5f)
            {
                IdleBehaviour(idleTime);
            }
            else
            {
                Wandering();
            }
        }
    }

    private void LifeCycleStatusCheck()
    {
        if (unit.IsAdult)
        {
            if (unit.IsPregnant)
            {
                if (unit.IsReadyToBear)
                {
                    Birth();
                    unit.IsReadyToBear = false;
                    unit.IsPregnant = false;
                }
            }
        }
        else
        {
            if (unit.IsReadyToGrowUp)
            {
                Mature();
            }
        }
    }

    private bool LookForFood()
    {
        Collider[] inSenseRadius = Physics.OverlapSphere(transform.position, unit.Gens.SenseRadius);
        Collider[] inInteractRadius = Physics.OverlapSphere(transform.position, unit.Gens.InteractionRadius);

        foreach (var hitCollider in inInteractRadius)
        {
            if (hitCollider.GetComponent<Food>() != null)
            {
                EatFood(hitCollider.GetComponent<Food>());
                return true;
            }
        }
        foreach (var hitCollider in inSenseRadius)
        {
            if (hitCollider.GetComponent<Food>() != null)
            {
                SearchingForFood(hitCollider.ClosestPoint(transform.position));
                return true;
            }
        }

        return false;
    }

    private bool LookForDrink()
    {
        Collider[] inSenseRadius = Physics.OverlapSphere(transform.position, unit.Gens.SenseRadius);
        Collider[] inInteractRadius = Physics.OverlapSphere(transform.position, unit.Gens.InteractionRadius);

        foreach (var hitCollider in inInteractRadius)
        {
            if (hitCollider.GetComponent<Drink>() != null)
            {
                Drink(hitCollider.GetComponent<Drink>());
                return true;
            }
        }
        foreach (var hitCollider in inSenseRadius)
        {
            if (hitCollider.GetComponent<Drink>() != null)
            {
                SearchingForDrink(hitCollider.ClosestPoint(transform.position));
                return true;
            }
        }

        return false;
    }

    private void lookForValidMatingPartners()
    {
        if (unit.Gens.IsFemale)
        {
            FemaleMating();
            return;
        }
        else
        {
            Collider[] inSenseRadius = Physics.OverlapSphere(transform.position, unit.Gens.SenseRadius);

            foreach (var hitCollider in inSenseRadius)
            {
                UnitController potentialMatingTarget = hitCollider.GetComponent<UnitController>();
                if (potentialMatingTarget != null && potentialMatingTarget.transform != transform)
                {
                    if (potentialMatingTarget.ProposeMating(unit))
                    {
                        unit.targetedTransform = potentialMatingTarget.transform;
                        potentialMatingTarget.unit.targetedTransform = transform;
                        MaleMating(hitCollider.ClosestPoint(transform.position));
                        return;
                    }
                }
            }
        }

        Wandering();
    }

    private void IdleBehaviour(float time)
    {
        CurrentBehaviourState = BehaviourState.Idle;
        behaviurCounter = time;
    }

    public bool ProposeMating(Unit offeredMatting)
    {
        if(currentBehaviourState == BehaviourState.Mating && unit.Gens.IsFemale)
        {
            if (rand.NextDouble() > offeredMatting.Gens.Attractivness)
            {
                return true;
            }
            else
            {
                offeredMatting.Urge = 0;
                return false;
            }
        }
        return false;
    }

    private void MaleMating(Vector3 partnerPosition)
    {
        navMeshAgent.SetDestination(partnerPosition);
        CurrentBehaviourState = BehaviourState.Mating;
    }

    private void FemaleMating()
    { 
        if(!unit.IsPregnant)
        {
            CurrentBehaviourState = BehaviourState.Mating;
        }
    }

    private bool CheckForValidPartner()
    {
        Collider[] inInteractRadius = Physics.OverlapSphere(transform.position, unit.Gens.InteractionRadius, 1 << LayerMask.NameToLayer(unit.Gens.Species));
        
        foreach (var hitCollider in inInteractRadius)
        {
            UnitController potentialCopulateTarget = hitCollider.GetComponent<UnitController>();
            if (potentialCopulateTarget != null)
            {
                try
                {
                    if (potentialCopulateTarget.transform != transform && potentialCopulateTarget.unit.targetedTransform == transform)
                    {
                        return true;
                    }
                }
                catch(Exception e) 
                {
                    if (e.Message == "Object reference not set to an instance of an object")
                    {
                        return false;
                    }
                    else
                    {
                        Debug.LogError(e.Message);
                    }
                }
            }
        }
        return false;
    }

    public void Copulating()
    {
        currentBehaviourState = BehaviourState.Copulating;
        behaviurCounter = idleTime;
        unit.Urge = 0;
        if (unit.Gens.IsFemale && unit.targetedTransform.GetComponent<Unit>().Gens.ReproductionChance > rand.NextDouble())
        {
            unit.PregnancyCounter = unit.Gens.PregnancyTime;
            unit.IsPregnant = true;
        }
        else
        {
            if(unit.targetedTransform != null)
            {
                unit.targetedTransform.GetComponent<UnitController>().Copulating();
                unit.targetedTransform.GetComponent<Unit>().LastPartnerGenSample = unit.Gens;
            }
            
        }
    }

    private void SearchingForFood( Vector3 foodPosition)
    {
        navMeshAgent.SetDestination(foodPosition);
        CurrentBehaviourState = BehaviourState.SearchingForFood;
    }

    private void SearchingForDrink(Vector3 drinkPosition)
    {
        navMeshAgent.SetDestination(drinkPosition);
        CurrentBehaviourState = BehaviourState.SearchingForDrink;
    }

    private void EatFood(Food food)
    {
        unit.targetedTransform = food.transform;
        currentBehaviourState = BehaviourState.Eating;
    }

    private void Drink(Drink drink)
    {
        unit.targetedTransform = drink.transform;
        currentBehaviourState = BehaviourState.Drinking;
    }

    private void Wandering()
    {
        CurrentBehaviourState = BehaviourState.Wandering;
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * unit.Gens.WalkRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, unit.Gens.WalkRadius, 1);
        Vector3 finalPosition = hit.position;
        if(navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.SetDestination(finalPosition);
        }
    }

    private void Mature()
    {
        if(!unit.IsAdult)
        {
            GameObject adultObject;
            if(unit.Gens.IsFemale)
            {
                adultObject = unit.femalePrefab;
            }
            else
            {
                adultObject = unit.malePrefab;
            }
            Unit adultUnit =  Instantiate(adultObject, transform.position, Quaternion.identity).GetComponent<Unit>();
            adultUnit.Initialize(unit.Gens);
            Destroy(gameObject);
        }
    }

    private void Birth()
    {
        if (unit.IsAdult && unit.Gens.IsFemale)
        {
            int offspringQuantity = rand.Next((int)unit.Gens.OffspringMaxPopulation);

            for(int i = 0; i < offspringQuantity; i++)
            {
                Unit offspring = Instantiate(unit.offspringPrefab, transform.position, Quaternion.identity).GetComponent<Unit>();
                // 50% chance for gender
                offspring.Initialize(GenManager.Instance.InheritGens(unit.Gens, unit.LastPartnerGenSample, 0.1f));
                offspring.Gens.IsFemale = (rand.NextDouble() > 0.5f);
            }
        }
        unit.IsPregnant = false;
        unit.LastPartnerGenSample = null;
    }
}
