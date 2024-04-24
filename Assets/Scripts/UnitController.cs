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

    System.Random rand = new System.Random();

    [SerializeField] private BehaviourState currentBehaviourState = BehaviourState.None;
    [SerializeField] private float idleTime = 5f;
    [SerializeField] private float behaviourTimeLimit = 10f;
    [SerializeField] private float urgeTreshold = 100f;

    private float behaviurCounter;
    private NavMeshAgent navMeshAgent;
    private Unit unit;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        unit = GetComponent<Unit>();

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
                BehaviourDestintaionReached();
                break;
            case BehaviourState.SearchingForFood:
                BehaviourDestintaionReached();
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
                BehaviourDestintaionReached();
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
                if(unit.Gens.isFemale)
                {
                    break;
                }
                else
                {
                    if (CheckForValidPartner())
                    {
                        Copulating();
                        break;
                    }
                    BehaviourDestintaionReached();
                }
                break;
            case BehaviourState.Copulating:
                
                break;
        }
    }

    private void BehaviourDestintaionReached()
    {
        if (!navMeshAgent.pathPending && navMeshAgent.isOnNavMesh)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    FindeActivity();
                }
            }
        }
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
        else if (unit.Urge >= urgeTreshold && unit.IsAdult)
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
        Collider[] inSenseRadius = Physics.OverlapSphere(transform.position, unit.Gens.senseRadius);
        Collider[] inInteractRadius = Physics.OverlapSphere(transform.position, unit.Gens.interactionRadius);

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
        Collider[] inSenseRadius = Physics.OverlapSphere(transform.position, unit.Gens.senseRadius);
        Collider[] inInteractRadius = Physics.OverlapSphere(transform.position, unit.Gens.interactionRadius);

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
        if (unit.Gens.isFemale)
        {
            FemaleMating();
            return;
        }
        else
        {
            Collider[] inSenseRadius = Physics.OverlapSphere(transform.position, unit.Gens.senseRadius);

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
        if(currentBehaviourState == BehaviourState.Mating && unit.Gens.isFemale)
        {
            if (rand.NextDouble() > offeredMatting.Gens.attractiveness)
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
        Collider[] inInteractRadius = Physics.OverlapSphere(transform.position, unit.Gens.interactionRadius, LayerMask.NameToLayer(unit.Gens.species));
        foreach (var hitCollider in inInteractRadius)
        {
            UnitController potentialCopulateTarget = hitCollider.GetComponent<UnitController>();
            if (potentialCopulateTarget != null)
            {
                if(potentialCopulateTarget.unit.targetedTransform != null)
                {
                    if (potentialCopulateTarget.transform != transform && potentialCopulateTarget.unit.targetedTransform == transform)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void Copulating()
    {
        Debug.Log(transform.name + " is copulating with " + unit.targetedTransform.name);
        currentBehaviourState = BehaviourState.Copulating;
        behaviurCounter = idleTime;
        unit.Urge = 0;
        if (unit.Gens.isFemale)
        {
            unit.PregnancyCounter = unit.Gens.pregnancyTime;
            unit.IsPregnant = true;
        }
        else
        {
            unit.targetedTransform.GetComponent<UnitController>().Copulating();
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
        Vector3 randomDirection = Random.insideUnitSphere * unit.Gens.walkRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, unit.Gens.walkRadius, 1);
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
            if(unit.Gens.isFemale)
            {
                adultObject = unit.femalePrefab;
            }
            else
            {
                adultObject = unit.malePrefab;
            }
            Instantiate(adultObject, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private void Birth()
    {
        if (unit.IsAdult && unit.Gens.isFemale)
        {
            int offspringQuantity = rand.Next(unit.Gens.offspringMaxPopulation);

            for(int i = 0; i < offspringQuantity; i++)
            {
                Unit offspring = Instantiate(unit.offspringPrefab, transform.position, Quaternion.identity).GetComponent<Unit>();
                // 50% chance for gender
                offspring.Gens.isFemale = (rand.NextDouble() > 0.5f);
            }
        }
        unit.IsPregnant = false;
    }
}
