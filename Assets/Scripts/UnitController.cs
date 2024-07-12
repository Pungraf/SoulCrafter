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
using static UnityEditor.FilePathAttribute;
using static UnityEditor.PlayerSettings;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(AIUnit))]
[RequireComponent(typeof(FunnelModifier))]
public abstract class UnitController : MonoBehaviour
{
    public BaseBehaviour.Behaviour CurrentBehaviour
    {
        get { return currentBehaviour; }
        set { currentBehaviour = value; }
    }

    [SerializeField] protected Brain _brain;
    public Brain Brain
    { 
        get { return _brain; }
    }

    public event EventHandler OnDestinationReached;

    public Seeker seeker;
    public AIUnit aIPath;

    NNConstraint constraint = NNConstraint.None;

    public System.Random Rand = new System.Random();

    [SerializeField] public PackManager packManager;
    [SerializeField] protected BaseBehaviour.Behaviour currentBehaviour = BaseBehaviour.Behaviour.None;
    protected Unit unit;
    public Unit Unit
    {
        get { return unit; }
    }

    protected void Awake()
    {
        seeker = GetComponent<Seeker>();
        aIPath = GetComponent<AIUnit>();
        unit = GetComponent<Unit>();
    }
    // Start is called before the first frame update
    void Start()
    {
        packManager = GetComponent<PackManager>();

        constraint.constrainWalkability = true;
        constraint.walkable = true;
        constraint.constrainTags = true;
        constraint.tags = seeker.traversableTags;

        aIPath.OnDestinationReached += BehaviourDestintaionReached;

        ChooseBehaviour();
    }

    public void BehaviourDestintaionReached(object sender, EventArgs e)
    {
        OnDestinationReached?.Invoke(this, EventArgs.Empty);
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
        unit.targetedTransform = runAwayTarget;

        Vector3 directionAway = (transform.position - runAwayTarget.position).normalized * unit.Gens.Speed * 10f;
        directionAway += transform.position;
        MoveUnit(directionAway);

    }
    protected bool LookForFood()
    {
        Collider[] inSenseRadius = Physics.OverlapSphere(transform.position, unit.Gens.Perception);
        Transform potentialHuntTarget = null;
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
        //Search for closest prey
        potentialHuntTarget = FindClosestTransformPath(preyTargets);
        if(potentialHuntTarget != null)
        {
            StartCoroutine(Hunt(potentialHuntTarget));
        }

        return false;
    }

    IEnumerator Hunt(Transform huntedUnit)
    {
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
        //FindeActivity();
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


    protected void DisposeWastes()
    {
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
        DestroyAndUnsubscribe();
    }

    public void MoveUnit(Vector3 location)
    {
        GraphNode node = AstarPath.active.GetNearest(location, constraint).node;
        aIPath.destination = (Vector3)node.position;
        aIPath.SearchPath();
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

    public void ChooseBehaviour()
    {
        _brain.GetFirstBehaviour().Behave(ChooseBehaviour);         }

    public void ChooseBehaviour(BaseBehaviour.Behaviour behaviour)
    {
        _brain.GetBehaviourByType(behaviour).Behave(ChooseBehaviour);
    }

    public void DestroyAndUnsubscribe()
    {
        Destroy(gameObject);
    }
}
