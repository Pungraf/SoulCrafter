using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PackManager : MonoBehaviour
{
    [SerializeField]
    private bool hasPack;
    [SerializeField]
    private bool isLeader;
    private Unit unit;
    private Unit.Species packSpecies;
    [SerializeField]
    private List<PackManager> pack = new List<PackManager>();
    [SerializeField]
    private PackManager packLeader;

    [SerializeField]
    private int packSize;

    public bool HasPack
    {
        get { return hasPack; }
        set { hasPack = value; }
    }

    public bool IsLeader
    {
        get { return isLeader; }
        set { isLeader = value; }
    }

    public int PackSize
    {
        get { return packSize; }
    }

    public List<PackManager> Pack
    {
        get { return pack; }
        set
        {
            if (pack == value) return;
            pack = value;
            if (OnPackChange != null)
            {
                OnPackChange(pack);
                pack.RemoveAll(item => item == null);
            }
        }
    }
    public delegate void OnVariableChangeDelegate(List<PackManager> newVal);
    public event OnVariableChangeDelegate OnPackChange;

    public PackManager PackLeader
    {
        get { return packLeader; }
        set { packLeader = value; }
    }

    public Unit.Species PackSpecies
    {
        get { return packSpecies; }
    }

    private void Start()
    {
        unit = GetComponent<Unit>();
        packSpecies = unit.species;
    }

    private void PackChangeHandler(List<PackManager> newPack)
    {
        Debug.Log(this.name + " pack leader: " + packLeader + "changed pack members");
    }


    public void LookForPack()
    {
        Collider[] inSenseRadius = Physics.OverlapSphere(transform.position, unit.Gens.Perception);
        Transform potentialLeaderTransform = null;
        List<Transform> packTargets = new List<Transform>();
        List<Transform> freeTargets = new List<Transform>();

        foreach (var hitCollider in inSenseRadius)
        {
            //Build lists with proper types
            PackManager sensedUnit = hitCollider.GetComponent<PackManager>();
            if (sensedUnit != null && sensedUnit.packSpecies == unit.species && sensedUnit.isLeader && sensedUnit != this && sensedUnit.Pack.Count < sensedUnit.PackSize)
            {
                packTargets.Add(sensedUnit.transform);
            }
            if (sensedUnit != null && sensedUnit.packSpecies == unit.species && !sensedUnit.hasPack)
            {
                freeTargets.Add(sensedUnit.transform);
            }
        }

        potentialLeaderTransform = FindClosestTransformPath(packTargets);

        if(potentialLeaderTransform != null)
        {
            PackManager potentialLeader = potentialLeaderTransform.GetComponent<PackManager>();
            if (IsLeader)
            {
                if((Pack.Count + potentialLeader.Pack.Count) <= PackSize)
                {
                    MergePacks(potentialLeader);
                    return;
                }
                else
                {
                    return;
                }
            }
            else
            {
                potentialLeader.Pack.Add(this);
                PackLeader = potentialLeader;
                HasPack = true;
                SubscribePackChnageHandler(potentialLeader);
                return;
            }
        }
        if(IsLeader)
        {
            return;
        }

        int highestGenValue = 0;
        PackManager newPackLeader = null;

        foreach(Transform packTransform in freeTargets)
        {
            PackManager packUnit = packTransform.GetComponent<PackManager>();
            try
            {
                if (packUnit != null && packUnit.unit.genScore > highestGenValue)
                {
                    newPackLeader = packUnit;
                    highestGenValue = packUnit.unit.genScore;
                }
            }
            catch (NullReferenceException e)
            {
                Debug.Log("Pack unit, doesnt exist anymore. :" + e);
            }
        }

        if(newPackLeader != null)
        {
            newPackLeader.IsLeader = true;
            newPackLeader.HasPack = true;
            newPackLeader.Pack.Clear();
            newPackLeader.Pack.Add(newPackLeader);
            newPackLeader.PackLeader = newPackLeader;
        }
    }

    public void UnsubscribePackChnageHandler()
    {
        if(PackLeader != null)
        {
            PackLeader.OnPackChange -= PackChangeHandler;
        }
    }

    public void SubscribePackChnageHandler(PackManager packLeader)
    {
        packLeader.OnPackChange -= PackChangeHandler;
    }

    public void MergePacks(PackManager secondLeader)
    {
        if(unit.genScore >= secondLeader.unit.genScore)
        {
            secondLeader.IsLeader = false;
            foreach(PackManager packUnit in secondLeader.Pack)
            {
                Pack.Add(packUnit);
                packUnit.packLeader = this;
            }
            secondLeader.Pack.Clear();
        }
        else
        {
            IsLeader = false;
            foreach (PackManager packUnit in Pack)
            {
                secondLeader.Pack.Add(packUnit);
                packUnit.packLeader = secondLeader;
            }
            Pack.Clear();
        }
    }

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
