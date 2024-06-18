using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

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
        Disposing,
        Hunting,
        RunningAway
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

    [SerializeField] protected PackManager packManager;
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
        Ticker.Tick_05 += Update_Tick05;
        packManager = GetComponent<PackManager>();
    }


    // Update is called once per frame
    void Update()
    {
        behaviurCounter -= Time.deltaTime;
        ExecuteBehaviour();
    }

    private void Update_Tick05(object sender, EventArgs e)
    {
        unit.CheckStatuses();
        LifeCycleStatusCheck();
        DeprecatedBehaviour();
    }

    protected void ExecuteBehaviour()
    {
        switch (CurrentBehaviourState)
        {
            case BehaviourState.None:
                FindeActivity();
                break;
            case BehaviourState.Idle:
                behaviurCounter -= Time.deltaTime;
                if (behaviurCounter <= 0)
                {
                    FindeActivity();
                }
                break;
            case BehaviourState.Wandering:
                if (BehaviourDestintaionReached())
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
                if (unit.targetedTransform != null)
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
                if (unit.Gens.IsFemale)
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
            case BehaviourState.Disposing:
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
        if (behaviurCounter <= 0)
        {
            if(!SensedDanger())
            {
                Wandering();
            }
        }
    }

    protected void FindeActivity()
    {
        //Sense dangers
        if (SensedDanger())
        {
            return;
        }

        // Critical behaviuors
        if (unit.IsHungry || unit.IsThirsty)
        {
            if (unit.IsHungry)
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
        else if (unit.IsWasteReady)
        {
            DisposeWastes();
        }
        else if(packManager != null && !packManager.HasPack)
        {
            packManager.LookForPack();
        }
        else if (unit.Urge >= unit.Gens.UrgeTreshold && unit.IsAdult)
        {
            lookForValidMatingPartners();
        }
        // 50% chance for idle behaviours
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

    protected bool SensedDanger()
    {
        Collider[] inSenseRadius = Physics.OverlapSphere(transform.position, unit.Gens.SenseRadius);

        Unit predatorUnit = SensePredaot(inSenseRadius);
        if(predatorUnit != null)
        {
            RunAway(predatorUnit.transform);
            return true;
        }

        return false;
    }

    protected Unit SensePredaot(Collider[] sensedObjects)
    {
        Unit potentialPredator = null;
        List<Unit> predatorTargets = new List<Unit>();
        float closestTargetDistance = float.MaxValue;
        NavMeshPath path = null;

        foreach (var hitCollider in sensedObjects)
        {
            Unit sensedUnit = hitCollider.GetComponent<Unit>();
            if (sensedUnit != null && unit.predators.Contains(sensedUnit.species))
            {
                predatorTargets.Add(sensedUnit);
            }
        }

        foreach (Unit predator in predatorTargets)
        {
            path = new NavMeshPath();

            //TODO: potensialy chacne areaMask for predator areaMask
            if (NavMesh.CalculatePath(transform.position, predator.GetComponent<Collider>().ClosestPoint(transform.position), navMeshAgent.areaMask, path))
            {
                float distance = Vector3.Distance(transform.position, path.corners[0]);
                for (int i = 1; i < path.corners.Length; i++)
                {
                    distance += Vector3.Distance(path.corners[i - 1], path.corners[i]);
                }
                if (distance < closestTargetDistance)
                {
                    closestTargetDistance = distance;
                    potentialPredator = predator;
                }
            }
        }

        if (potentialPredator != null)
        {
            return potentialPredator;
        }
        return null;

    }

    protected void RunAway(Transform runAwayTarget)
    {
        currentBehaviourState = BehaviourState.RunningAway;
        unit.targetedTransform = runAwayTarget;


        Vector3 directionAway = (transform.position - runAwayTarget.position).normalized * unit.Gens.WalkRadius;
        directionAway += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(directionAway, out hit, unit.Gens.WalkRadius, navMeshAgent.areaMask);
        Vector3 finalPosition = hit.position;
        if (navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.SetDestination(finalPosition);
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
        Food potentialFoodTarget = null;
        List<Unit> preyTargets = new List<Unit>();
        List<Food> foodTargets = new List<Food>();
        float closestTargetDistance = float.MaxValue;
        NavMeshPath path = null;

        foreach (var hitCollider in inSenseRadius)
        {
            //Build lists with proper types
            Unit sensedUnit = hitCollider.GetComponent<Unit>();
            if(sensedUnit != null && unit.foodChainSpecies.Contains(sensedUnit.species))
            {
                preyTargets.Add(sensedUnit);
            }

            Food food = hitCollider.GetComponent<Food>();
            if (food != null && unit.edibleFood.Contains(food.foodType))
            {
                foodTargets.Add(food);
            }
        }

        //Search for closest food
        foreach(Food food in foodTargets)
        {
            path = new NavMeshPath();

            if (food != null && NavMesh.CalculatePath(transform.position, food.GetComponent<Collider>().ClosestPoint(transform.position), navMeshAgent.areaMask, path))
            {
                float distance = Vector3.Distance(transform.position, path.corners[0]);
                for (int i = 1; i < path.corners.Length; i++)
                {
                    distance += Vector3.Distance(path.corners[i - 1], path.corners[i]);
                }
                if (distance < closestTargetDistance && path.status == NavMeshPathStatus.PathComplete)
                {
                    closestTargetDistance = distance;
                    potentialFoodTarget = food;
                }
            }
        }
        if(potentialFoodTarget != null)
        {
            SearchingForFood(potentialFoodTarget.GetComponent<Collider>().ClosestPoint(transform.position));
            return true;
        }
        //Search for closest prey
        foreach (Unit prey in preyTargets)
        {
            path = new NavMeshPath();

            if (prey != unit && NavMesh.CalculatePath(transform.position, prey.GetComponent<Collider>().ClosestPoint(transform.position), navMeshAgent.areaMask, path))
            {
                float distance = Vector3.Distance(transform.position, path.corners[0]);
                for(int i = 1; i < path.corners.Length; i++)
                {
                    distance += Vector3.Distance(path.corners[i - 1], path.corners[i]);
                }
                if(distance < closestTargetDistance && path.status == NavMeshPathStatus.PathComplete)
                {
                    closestTargetDistance = distance;
                    potentialHuntTarget = prey;
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

        Drink potentialDrinkTarget = null;
        List<Drink> drinkTargets = new List<Drink>();
        float closestTargetDistance = float.MaxValue;
        NavMeshPath path = null;

        foreach (var hitCollider in inSenseRadius)
        {
            //Build list with proper type
            Drink sensedDrink = hitCollider.GetComponent<Drink>();
            if (sensedDrink != null)
            {
                drinkTargets.Add(sensedDrink);
            }
        }
        
        //Search for closest Drink
        foreach (Drink drink in drinkTargets)
        {
            path = new NavMeshPath();

            if (NavMesh.CalculatePath(transform.position, drink.GetComponent<Collider>().ClosestPoint(transform.position), navMeshAgent.areaMask, path))
            {
                float distance = Vector3.Distance(transform.position, path.corners[0]);
                for (int i = 1; i < path.corners.Length; i++)
                {
                    distance += Vector3.Distance(path.corners[i - 1], path.corners[i]);
                }
                if (distance < closestTargetDistance && path.status == NavMeshPathStatus.PathComplete)
                {
                    closestTargetDistance = distance;
                    potentialDrinkTarget = drink;
                }
            }
        }
        if (potentialDrinkTarget != null)
        {
            SearchingForDrink(potentialDrinkTarget.GetComponent<Collider>().ClosestPoint(transform.position));
            return true;
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

            UnitController potentialMatingTarget = null;
            List<UnitController> unitTargets = new List<UnitController>();
            float closestTargetDistance = float.MaxValue;
            NavMeshPath path = null;
            foreach (var hitCollider in inSenseRadius)
            {
                UnitController sensedUnit = hitCollider.GetComponent<UnitController>();
                if (sensedUnit != null)
                {
                    unitTargets.Add(sensedUnit);
                }
            }
            
            foreach(UnitController unitTarget in unitTargets)
            {
                path = new NavMeshPath();

                if (unitTarget.transform != transform && NavMesh.CalculatePath(transform.position, unitTarget.GetComponent<Collider>().ClosestPoint(transform.position), navMeshAgent.areaMask, path))
                {
                    float distance = Vector3.Distance(transform.position, path.corners[0]);
                    for (int i = 1; i < path.corners.Length; i++)
                    {
                        distance += Vector3.Distance(path.corners[i - 1], path.corners[i]);
                    }
                    if (distance < closestTargetDistance && path.status == NavMeshPathStatus.PathComplete)
                    {
                        closestTargetDistance = distance;
                        potentialMatingTarget = unitTarget;
                    }
                }
            }

            if (potentialMatingTarget != null)
            {
                if (potentialMatingTarget.ProposeMating(unit))
                {
                    unit.targetedTransform = potentialMatingTarget.transform;
                    potentialMatingTarget.unit.targetedTransform = transform;
                    MaleMating(potentialMatingTarget.GetComponent<Collider>().ClosestPoint(transform.position), potentialMatingTarget.transform);
                    return;
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
            double randomDouble = rand.NextDouble();
            
            if (randomDouble < offeredMatting.Gens.Attractivness)
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
                    //TODO: Change for attack speed
                    yield return StartCoroutine(Attack(huntTarget));
                    break;
                }
            }

            if (navMeshAgent.isOnNavMesh && huntTarget != null)
            {
                navMeshAgent.SetDestination(huntTarget.GetComponent<Collider>().ClosestPoint(transform.position));
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

    protected void DisposeWastes()
    {
        currentBehaviourState = BehaviourState.Disposing;
        behaviurCounter = 1f;

        Destroy(Instantiate(unit.wastePrefab, unit.transform.position, Quaternion.identity), 1f);

        RaycastHit hit;
        FertilePlane fertilePlane;
        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1f);
        if(hit.collider != null && hit.transform.TryGetComponent(out fertilePlane))
        {
            fertilePlane.Fertilize(unit.Waste);
        }
        unit.Dispose();
    }

    protected void Wandering()
    {
        CurrentBehaviourState = BehaviourState.Wandering;
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * unit.Gens.WalkRadius;
        if(packManager != null && packManager.HasPack && !packManager.IsLeader)
        {
            randomDirection += packManager.PackLeader.transform.position;
        }
        else
        {
            randomDirection += transform.position;
        }
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, unit.Gens.WalkRadius, navMeshAgent.areaMask);
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

            if (packManager.HasPack)
            {
                PackManager evolvedPackMember = evolvedUnit.GetComponent<PackManager>();
                if (packManager.IsLeader)
                {
                    evolvedPackMember.IsLeader = true;
                    evolvedPackMember.HasPack = true;
                    evolvedPackMember.Pack = new List<PackManager>
                    {
                        evolvedPackMember
                    };
                    packManager.Pack.Remove(packManager);
                    foreach (PackManager packMember in packManager.Pack)
                    {
                        packMember.PackLeader = evolvedPackMember;
                        packMember.UnsubscribePackChnageHandler();
                        evolvedPackMember.Pack.Add(packMember);
                        packMember.SubscribePackChnageHandler(evolvedPackMember);
                    }
                }
                else
                {
                    packManager.PackLeader.Pack.Remove(packManager);
                    packManager.PackLeader.Pack.Add(evolvedPackMember);
                    evolvedPackMember.PackLeader = packManager.PackLeader;
                    evolvedPackMember.HasPack = true;
                }


            }
            Ticker.Tick_05 -= Update_Tick05;
            Destroy(gameObject);
        }
    }

    protected virtual void Birth()
    {
        if (unit.IsAdult && unit.Gens.IsFemale)
        {
            int offspringQuantity = rand.Next(1,(int)unit.Gens.OffspringMaxPopulation);

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
        if(packManager.HasPack)
        {
            if(packManager.IsLeader)
            {
                foreach(PackManager packMember in packManager.Pack)
                {
                    packMember.UnsubscribePackChnageHandler();
                    packMember.PackLeader = null;
                    packMember.HasPack = false;
                }
            }
            else
            {
                packManager.PackLeader.Pack.Remove(packManager);
            }
        }
        Ticker.Tick_05 -= Update_Tick05;
        Destroy(gameObject);
    }
}
