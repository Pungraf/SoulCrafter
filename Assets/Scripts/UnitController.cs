using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public abstract class UnitController : MonoBehaviour
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
        Copulating,
        Hunting
    }
    public BehaviourState CurrentBehaviourState
    {
        get { return currentBehaviourState; }
        set
        {
            currentBehaviourState = value;
            behaviurCounter = behaviourTimeLimit;
            unit.targetedTransform = null;
        }
    }
    public NavMeshAgent navMeshAgent;

    protected System.Random rand = new System.Random();

    [SerializeField] protected BehaviourState currentBehaviourState = BehaviourState.None;
    [SerializeField] protected float idleTime = 5f;
    [SerializeField] protected float behaviourTimeLimit = 10f;

    protected float behaviurCounter;
    protected Unit unit;

    protected void Awake()
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

    protected void ExecuteBehaviour()
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
            case BehaviourState.Hunting:

                break;
        }
    }

    protected bool BehaviourDestintaionReached()
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

    protected void DeprecatedBehaviour()
    {
        if(behaviurCounter <= 0)
        {
            Wandering();
        }
    }

    protected void FindeActivity()
    {
        // Critical behaviuors
        if (unit.IsHungry || unit.IsThirsty)
        {
            if(unit.IsHungry)
            {
                if (!LookForFood() && unit.IsThirsty)
                {
                    if (!LookForDrink())
                    {
                        Wandering();
                    }
                }
            }
            else
            {
                if (!LookForDrink() && unit.IsHungry)
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

    protected void LifeCycleStatusCheck()
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

        if (unit.IsReadyToGrowUp)
        {
            Mature();
        }
    }

    protected bool LookForFood()
    {
        Collider[] inSenseRadius = Physics.OverlapSphere(transform.position, unit.Gens.SenseRadius);
        Collider[] inInteractRadius = Physics.OverlapSphere(transform.position, unit.Gens.InteractionRadius);

        foreach (var hitCollider in inInteractRadius)
        {
            Food food = hitCollider.GetComponent<Food>();
            if (food != null)
            {
                if(unit.edibleFood.Contains(food.foodType))
                {
                    EatFood(hitCollider.GetComponent<Food>());
                    return true;
                }
            }
        }

        Unit potentialHuntTarget = null;
        foreach (var hitCollider in inSenseRadius)
        {
            Unit sensedUnit = hitCollider.GetComponent<Unit>();
            Food food = hitCollider.GetComponent<Food>();
            if(sensedUnit != null && sensedUnit != unit && unit.foodChainSpecies.Contains(sensedUnit.species))
            {
                potentialHuntTarget = sensedUnit;
            }

            if (food != null)
            {
                if (unit.edibleFood.Contains(food.foodType))
                {
                    SearchingForFood(hitCollider.ClosestPoint(transform.position));
                    return true;
                }
            }
        }
        if(potentialHuntTarget != null)
        {
            StartCoroutine(Hunt(potentialHuntTarget));
        }

        return false;
    }

    protected bool LookForDrink()
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

    protected void lookForValidMatingPartners()
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
                        MaleMating(hitCollider.ClosestPoint(transform.position), potentialMatingTarget.transform);
                        return;
                    }
                }
            }
        }

        Wandering();
    }

    protected void IdleBehaviour(float time)
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

    protected void MaleMating(Vector3 partnerPosition, Transform matingTarget)
    {
        Unit targetMating = unit;
        CurrentBehaviourState = BehaviourState.Mating;
        unit.targetedTransform = matingTarget;
        navMeshAgent.SetDestination(partnerPosition);
    }

    protected void FemaleMating()
    { 
        if(!unit.IsPregnant)
        {
            CurrentBehaviourState = BehaviourState.Mating;
        }
    }

    protected bool CheckForValidPartner()
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

    protected void SearchingForFood( Vector3 foodPosition)
    {
        if (navMeshAgent.isOnNavMesh)
        {
            CurrentBehaviourState = BehaviourState.SearchingForFood;
            navMeshAgent.SetDestination(foodPosition);
        }
        
    }

    IEnumerator Hunt(Unit huntedUnit)
    {
        CurrentBehaviourState = BehaviourState.Hunting;
        Unit huntTarget = huntedUnit;
        unit.targetedTransform = huntTarget.transform;
        while(huntTarget != null)
        {
            //TODO: Change detection mask here, nad in other Overlaps
            Collider[] inInteractRadius = Physics.OverlapSphere(transform.position, unit.Gens.InteractionRadius);
            foreach (var hitCollider in inInteractRadius)
            {
                Unit unitInRange = hitCollider.GetComponent<Unit>();
                if (unitInRange != null && unitInRange != unit && unit.foodChainSpecies.Contains(unitInRange.species))
                {
                    huntTarget = unitInRange;
                    unit.targetedTransform = huntTarget.transform;
                    StartCoroutine(Attack(huntTarget));
                    //TODO: Change for attack speed
                    yield return StartCoroutine(Attack(huntTarget));
                    break;
                }
            }

            if (navMeshAgent.isOnNavMesh && huntTarget != null)
            {
                navMeshAgent.SetDestination(huntTarget.transform.position);
                yield return new WaitForSeconds(0.5f);
            }
        }
        FindeActivity();
    }

    IEnumerator Attack(Unit targetUnit)
    {
        navMeshAgent.enabled = false;
        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (targetUnit.transform.position - transform.position).normalized;
        //TODO: parameter for uni radius
        Vector3 attackPosition = targetUnit.transform.position - dirToTarget * (0.5f + 0.5f / 2);

        float attackSpeed = 3;
        float percent = 0;

        bool hasAppliedDamage = false;

        while (percent <= 1)
        {
            if (percent >= .5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                //TODO: parameter for unit damage
                targetUnit.TakeDamage(10f);
            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            yield return null;
        }

        navMeshAgent.enabled = true;
    }

    protected void SearchingForDrink(Vector3 drinkPosition)
    {
        if (navMeshAgent.isOnNavMesh)
        {
            CurrentBehaviourState = BehaviourState.SearchingForDrink;
            navMeshAgent.SetDestination(drinkPosition);
        }
        
    }

    protected void EatFood(Food food)
    {
        currentBehaviourState = BehaviourState.Eating;
        unit.targetedTransform = food.transform;
    }

    protected void Drink(Drink drink)
    {
        currentBehaviourState = BehaviourState.Drinking;
        unit.targetedTransform = drink.transform;
    }

    protected void Wandering()
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

    protected void Mature()
    {
        if(unit.evolvedUnitPrefab == null)
        {
            Death();
        }
        else
        {
            Unit evolvedUnit = Instantiate(unit.evolvedUnitPrefab, transform.position, Quaternion.identity).GetComponent<Unit>();
            evolvedUnit.Initialize(unit.Gens, unit.Health, unit.Hunger, unit.Thirst);
            Destroy(gameObject);
        }
    }

    protected virtual void Birth()
    {
        if (unit.IsAdult && unit.Gens.IsFemale)
        {
            int offspringQuantity = rand.Next((int)unit.Gens.OffspringMaxPopulation);

            for(int i = 0; i < offspringQuantity; i++)
            {
                Unit offspring;
                GenSample newGen = GenManager.Instance.InheritGens(unit.Gens, unit.LastPartnerGenSample, 0.1f);
                // 50% chance for gender
                if (newGen.IsFemale)
                {
                    offspring = Instantiate(unit.femaleOffspringPrefab, transform.position, Quaternion.identity).GetComponent<Unit>();
                }
                else
                {
                    offspring = Instantiate(unit.maleOffspringPrefab, transform.position, Quaternion.identity).GetComponent<Unit>();
                }
               
                offspring.Initialize(newGen, newGen.MaxHealth,newGen.HungerTreshold, newGen.ThirstTreshold);
            }
        }
        unit.IsPregnant = false;
        unit.LastPartnerGenSample = null;
    }

    public void Death()
    {
        if(unit.corpseUnitPrefab != null)
        {
            Food corpse = Instantiate(unit.corpseUnitPrefab, transform.position, Quaternion.identity).GetComponent<Food>();
            if (!unit.IsAdult)
            {
                corpse.Nutritiousness = unit.Gens.MaxHealth / 4f;
            }
            else
            {
                corpse.Nutritiousness = unit.Gens.MaxHealth / 2f;
            }
            corpse.Initialize();
        }
        Destroy(gameObject);
    }
}
