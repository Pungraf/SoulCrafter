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
        _brain = GetComponentInChildren<Brain>();
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
