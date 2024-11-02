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
public abstract class UnitController : MonoBehaviour, IInteractable
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

    [SerializeField] protected UnitPackManager packManager;
    public UnitPackManager PackManager
    {
        get { return packManager; }
    }

    [SerializeField] protected BaseBehaviour.Behaviour currentBehaviour = BaseBehaviour.Behaviour.None;
    protected Unit unit;
    public Unit Unit
    {
        get { return unit; }
    }

    [SerializeField]
    protected List<byte> baseTraversableTagMask = new List<byte>();

    [SerializeField] protected bool isControlled;
    public bool IsControlled
    {
        get { return isControlled; }
        set {
                unitUI.SetControlledUI(value);
                isControlled = value; 
            }
    }

    protected UnitUI unitUI;

    protected void Awake()
    {
        seeker = GetComponent<Seeker>();
        aIPath = GetComponent<AIUnit>();
        unit = GetComponent<Unit>();
        _brain = GetComponentInChildren<Brain>();
        unitUI = GetComponent<UnitUI>();
    }
    // Start is called before the first frame update
    void Start()
    {
        packManager = GetComponent<UnitPackManager>();

        UpdateTraversableConstrain();

        aIPath.OnDestinationReached += BehaviourDestintaionReached;


        InvokeRepeating("SearchForPack", 5f, 10f);
        ChooseBehaviour();
        GetTileTagNumberBeneath();
    }

    public void BehaviourDestintaionReached(object sender, EventArgs e)
    {
        OnDestinationReached?.Invoke(this, EventArgs.Empty);
    }

    public void SetTraversableMask(List<byte> mask)
    {
        seeker.traversableTags = ConvertToBitmask(mask);
        UpdateTraversableConstrain();
    }

    public void SetTraversableMask(int mask)
    {
        seeker.traversableTags = mask;
        UpdateTraversableConstrain();
    }

    public void RemoveTraversableTag(int tagIndex)
    {
        if((seeker.traversableTags & (1 << tagIndex)) != 0)
        {
            seeker.traversableTags ^= 1 << tagIndex;
            UpdateTraversableConstrain();
        }
    }

    public void AddTraversableTag(int tagIndex)
    {
        seeker.traversableTags |= 1 << tagIndex;
        UpdateTraversableConstrain();
    }

    public void ResetTraversableTag()
    {
        SetTraversableMask(baseTraversableTagMask);
        UpdateTraversableConstrain();
    }

    public void UpdateTraversableConstrain()
    {
        constraint.constrainWalkability = true;
        constraint.walkable = true;
        constraint.constrainTags = true;
        constraint.tags = seeker.traversableTags;
    }

    protected int ConvertToBitmask(List<byte> mask)
    {
        int bitmask = 0;
        foreach(byte tag in mask)
        {
            bitmask |= (1 << tag);
        }
        return bitmask;
    }

    public int GetTileTagNumberBeneath()
    {
        int tagNumber = 0;
        Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);
        RaycastHit hit;
        int layerNumber = LayerMask.NameToLayer("MapTerrain");
        if (Physics.Raycast(ray, out hit, 10f, 1 << layerNumber))
        {
            GraphUpdateScene tileGraph = hit.transform.GetComponent<GraphUpdateScene>();
            if(tileGraph != null)
            {
                tagNumber = tileGraph.setTag;
            }
        }
        return tagNumber;
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
                packManager.DisbandPack();
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
            //TODO: change for paid multipath search
            while(!path.IsDone())
            {
            }

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
        BaseBehaviour choosedBehaviour = _brain.GetBehaviourByType(behaviour);
        if(choosedBehaviour != null)
        {
            _brain.GetBehaviourByType(behaviour).Behave(ChooseBehaviour);
        }
        else
        {
            Debug.LogWarning("Null behaviour, next behaviour choosed");
            ChooseBehaviour();
        }
    }
    public void ChooseBehaviour(BaseBehaviour.Behaviour behaviour, Vector3 behaviourLocation)
    {
        BaseBehaviour choosedBehaviour = _brain.GetBehaviourByType(behaviour);
        if (choosedBehaviour != null)
        {
            _brain.GetBehaviourByType(behaviour).Behave(ChooseBehaviour, behaviourLocation);
        }
        else
        {
            Debug.LogWarning("Null behaviour, next behaviour choosed");
            ChooseBehaviour();
        }
    }

    public void ForceBehaviour(BaseBehaviour.Behaviour behaviour, Vector3 behaviourLoaction)
    {
        _brain.CurrentBehaviour.BehaviourComplete(behaviour, behaviourLoaction);
    }

    public void ForceBehaviour(BaseBehaviour.Behaviour behaviour)
    {
        _brain.CurrentBehaviour.BehaviourComplete(behaviour);
    }

    protected void SearchForPack()
    {
        if (!packManager.HasPack || (packManager.IsLeader && packManager.Pack.Count < packManager.PackSize))
        {
            PackManager.LookForPack();
        }
        else
        {
            return;
        }
    }

    public void DestroyAndUnsubscribe()
    {
        Destroy(gameObject);
    }

    public void Interact(PlayerController player)
    {
        packManager.JoinPlayer(player.PackManager);
    }
}
