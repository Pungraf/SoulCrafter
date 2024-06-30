using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(AIPath))]
[RequireComponent(typeof(FunnelModifier))]
public abstract class UnitController : MonoBehaviour
{
    //Debug
    public GameObject debugSphere;

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

    public Seeker seeker;
    public AIPath aIPath;

    protected System.Random rand = new System.Random();

    [SerializeField] protected PackManager packManager;
    [SerializeField] protected BehaviourState currentBehaviourState = BehaviourState.None;
    [SerializeField] protected float idleTime = 5f;
    [SerializeField] protected float behaviourTimeLimit = 10f;

    public float behaviurCounter;
    protected Unit unit;

    protected void Awake()
    {
        seeker = GetComponent<Seeker>();
        aIPath = GetComponent<AIPath>();
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
                if (unit.IsFemale)
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
        return aIPath.reachedDestination;
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
        
        else if (unit.Urge >= 100 && unit.IsAdult)
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
        if (!packManager.HasPack || (packManager.IsLeader && packManager.Pack.Count < packManager.PackSize))
        {
            packManager.LookForPack();
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
        Collider[] inSenseRadius = Physics.OverlapSphere(transform.position, unit.Gens.Perception);
        Transform predatorUnit = SensePredaot(inSenseRadius);
        if(predatorUnit != null)
        {
            RunAway(predatorUnit.transform);
            return true;
        }

        return false;
    }

    protected Transform SensePredaot(Collider[] sensedObjects)
    {
        Transform potentialPredator = null;
        List<Transform> predatorTargets = new List<Transform>();

        foreach (Collider hitCollider in sensedObjects)
        {
            Transform sensedTransform = hitCollider.transform;
            Unit sensedUnit = sensedTransform.GetComponent<Unit>();
            if (sensedUnit != null && unit.predators.Contains(sensedUnit.species))
            {
                predatorTargets.Add(sensedTransform);
            }
        }

        potentialPredator = FindClosestTransformPath(predatorTargets);

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


        Vector3 directionAway = (transform.position - runAwayTarget.position).normalized * unit.Gens.Speed * 10f;
        directionAway += transform.position;

        var constraint = NNConstraint.None;
        constraint.constrainWalkability = true;
        constraint.walkable = true;
        GraphNode node = AstarPath.active.GetNearest(directionAway,constraint).node;
        aIPath.destination = (Vector3)node.position;
        aIPath.SearchPath();

    }
    protected bool LookForFood()
    {
        Collider[] inSenseRadius = Physics.OverlapSphere(transform.position, unit.Gens.Perception);
        Collider[] inInteractRadius = Physics.OverlapSphere(transform.position, unit.Gens.Reach);

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

        Transform potentialHuntTarget = null;
        Transform potentialFoodTarget = null;
        List<Transform> preyTargets = new List<Transform>();
        List<Transform> foodTargets = new List<Transform>();

        foreach (var hitCollider in inSenseRadius)
        {
            //Build lists with proper types
            Transform sensedTransform = hitCollider.transform;
            Unit unitTransform = sensedTransform.GetComponent<Unit>();
            Food foodTransform = sensedTransform.GetComponent<Food>();
            if(unitTransform != null && unit.foodChainSpecies.Contains(unitTransform.species))
            {
                preyTargets.Add(sensedTransform);
            }

            if (foodTransform != null && unit.edibleFood.Contains(foodTransform.foodType))
            {
                foodTargets.Add(sensedTransform);
            }
        }

        //Search for closest food
        potentialFoodTarget = FindClosestTransformPath(foodTargets);
        if(potentialFoodTarget != null)
        {
            SearchingForFood(potentialFoodTarget.GetComponent<Collider>().ClosestPoint(transform.position));
            return true;
        }
        //Search for closest prey
        potentialHuntTarget = FindClosestTransformPath(preyTargets);
        if(potentialHuntTarget != null)
        {
            StartCoroutine(Hunt(potentialHuntTarget));
        }

        return false;
    }

    protected bool LookForDrink()
    {
        Collider[] inSenseRadius = Physics.OverlapSphere(transform.position, unit.Gens.Perception);
        Collider[] inInteractRadius = Physics.OverlapSphere(transform.position, unit.Gens.Reach);

        foreach (var hitCollider in inInteractRadius)
        {
            if (hitCollider.GetComponent<Drink>() != null)
            {
                Drink(hitCollider.GetComponent<Drink>());
                return true;
            }
        }

        Transform potentialDrinkTarget = null;
        List<Transform> drinkTargets = new List<Transform>();

        foreach (var hitCollider in inSenseRadius)
        {
            //Build list with proper type
            Drink sensedDrink = hitCollider.GetComponent<Drink>();
            if (sensedDrink != null)
            {
                drinkTargets.Add(sensedDrink.transform);
            }
        }

        //Search for closest Drink
        potentialDrinkTarget = FindClosestTransformPath(drinkTargets);

        if (potentialDrinkTarget != null)
        {
            SearchingForDrink(potentialDrinkTarget.GetComponent<Collider>().ClosestPoint(transform.position));
            return true;
        }

        return false;
    }

    protected void lookForValidMatingPartners()
    {
        if (unit.IsFemale)
        {
            FemaleMating();
            return;
        }
        else
        {
            Collider[] inSenseRadius = Physics.OverlapSphere(transform.position, unit.Gens.Perception);

            Transform potentialMatingTarget = null;
            List<Transform> unitTargets = new List<Transform>();

            foreach (var hitCollider in inSenseRadius)
            {
                UnitController sensedUnit = hitCollider.GetComponent<UnitController>();
                if (sensedUnit != null && sensedUnit.currentBehaviourState == BehaviourState.Mating && sensedUnit.unit.IsFemale)
                {
                    unitTargets.Add(sensedUnit.transform);
                }
            }
            
            potentialMatingTarget = FindClosestTransformPath(unitTargets);
            if (potentialMatingTarget != null)
            {
                if (potentialMatingTarget.GetComponent<UnitController>().ProposeMating(unit))
                {
                    potentialMatingTarget.GetComponent<Unit>().targetedTransform = transform;
                    MaleMating(potentialMatingTarget.transform);
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
        if(currentBehaviourState == BehaviourState.Mating && unit.IsFemale)
        {
            double randomDouble = rand.NextDouble();
            
            if (randomDouble < offeredMatting.Gens.Attractiveness)
            {
                behaviurCounter = behaviourTimeLimit;
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

    protected void MaleMating(Transform matingTarget)
    {
        CurrentBehaviourState = BehaviourState.Mating;
        unit.targetedTransform = matingTarget;

        var constraint = NNConstraint.None;
        constraint.constrainWalkability = true;
        constraint.walkable = true;
        GraphNode node = AstarPath.active.GetNearest(matingTarget.position, constraint).node;
        aIPath.destination = (Vector3)node.position;
        aIPath.SearchPath();
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
        //Change validation from layer
        Collider[] inInteractRadius = Physics.OverlapSphere(transform.position, unit.Gens.Reach, 1 << LayerMask.NameToLayer(unit.species.ToString()));
        
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
        if (unit.IsFemale && unit.targetedTransform.GetComponent<Unit>().Gens.Fecundity > rand.NextDouble())
        {
            unit.PregnancyCounter = unit.Gens.Gestation;
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
        CurrentBehaviourState = BehaviourState.SearchingForFood;

        var constraint = NNConstraint.None;
        constraint.constrainWalkability = true;
        constraint.walkable = true;
        GraphNode node = AstarPath.active.GetNearest(foodPosition, constraint).node;
        aIPath.destination = (Vector3)node.position;
        aIPath.SearchPath();
    }

    IEnumerator Hunt(Transform huntedUnit)
    {
        CurrentBehaviourState = BehaviourState.Hunting;
        Unit huntTarget = huntedUnit.GetComponent<Unit>();
        unit.targetedTransform = huntTarget.transform;
        while(huntTarget != null)
        {
            //TODO: Change detection mask here, nad in other Overlaps
            Collider[] inInteractRadius = Physics.OverlapSphere(transform.position, unit.Gens.Reach);
            foreach (var hitCollider in inInteractRadius)
            {
                Unit unitInRange = hitCollider.GetComponent<Unit>();
                if (unitInRange != null && unitInRange != unit && unit.foodChainSpecies.Contains(unitInRange.species))
                {
                    huntTarget = unitInRange;
                    unit.targetedTransform = huntTarget.transform;
                    //TODO: Change for attack speed
                    yield return StartCoroutine(Attack(huntTarget.transform));
                    break;
                }
            }

            if (huntTarget != null)
            {
                var constraint = NNConstraint.None;
                constraint.constrainWalkability = true;
                constraint.walkable = true;
                GraphNode node = AstarPath.active.GetNearest(huntTarget.transform.position, constraint).node;
                aIPath.destination = (Vector3)node.position;
                aIPath.SearchPath();
                yield return new WaitForSeconds(0.5f);
            }
        }
        FindeActivity();
    }

    IEnumerator Attack(Transform targetUnit)
    {
        aIPath.enabled = false;
        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (targetUnit.position - transform.position).normalized;
        //TODO: parameter for uni radius
        Vector3 attackPosition = targetUnit.position - dirToTarget * (0.5f + 0.5f / 2);

        float attackSpeed = 3;
        float percent = 0;

        bool hasAppliedDamage = false;

        while (percent <= 1)
        {
            if (percent >= .5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                //TODO: parameter for unit damage
                targetUnit.GetComponent<Unit>().TakeDamage(unit.Gens.Strength);
            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            yield return null;
        }

        aIPath.enabled = true;
    }

    protected void SearchingForDrink(Vector3 drinkPosition)
    {
        CurrentBehaviourState = BehaviourState.SearchingForDrink;

        var constraint = NNConstraint.None;
        constraint.constrainWalkability = true;
        constraint.walkable = true;
        GraphNode node = AstarPath.active.GetNearest(drinkPosition, constraint).node;
        aIPath.destination = (Vector3)node.position;
        aIPath.SearchPath();
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
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * unit.Gens.Speed * 10f;
        if(packManager.PackLeader != null && packManager.HasPack && !packManager.IsLeader)
        {
            randomDirection += packManager.PackLeader.transform.position;
        }
        else
        {
            randomDirection += transform.position;
        }

        var constraint = NNConstraint.None;
        constraint.constrainWalkability = true;
        constraint.walkable = true;

        GraphNode node = AstarPath.active.GetNearest(randomDirection, constraint).node;

        aIPath.destination = (Vector3)node.position;
        aIPath.SearchPath();
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
        if (unit.IsAdult && unit.IsFemale)
        {
            int offspringQuantity = rand.Next(1,(int)unit.Gens.Fertility);

            for(int i = 0; i < offspringQuantity; i++)
            {
                Unit offspring;
                GenSample newGen = GenManager.Instance.InheritGens(unit.Gens, unit.LastPartnerGenSample, 0.1f);
                // 50% chance for gender
                if (0.5f > rand.NextDouble())
                {
                    offspring = Instantiate(unit.femaleOffspringPrefab, transform.position, Quaternion.identity).GetComponent<Unit>();
                }
                else
                {
                    offspring = Instantiate(unit.maleOffspringPrefab, transform.position, Quaternion.identity).GetComponent<Unit>();
                }
               
                offspring.Initialize(newGen, newGen.Vitality);
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
                corpse.Nutritiousness = unit.Gens.Vitality / 4f;
            }
            else
            {
                corpse.Nutritiousness = unit.Gens.Vitality / 2f;
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

    //Change for statick methoid in controller
    public Transform FindClosestTransformPath(List<Transform> transforms)
    {
        Transform closestTransform = null;
        float closestDistance = float.MaxValue;
        foreach (var targetTransform in transforms)
        {
            Path path = ABPath.Construct(transform.position, targetTransform.position);
            AstarPath.StartPath(path);
            AstarPath.BlockUntilCalculated(path);

            float distance = path.GetTotalLength();
            if (distance < closestDistance)
            {
                closestTransform = targetTransform;
                closestDistance = distance;
            }
        }
        return closestTransform;
    }
}
