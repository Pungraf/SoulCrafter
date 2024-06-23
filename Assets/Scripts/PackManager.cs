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
        Collider[] inSenseRadius = Physics.OverlapSphere(transform.position, unit.Gens.SenseRadius);

        PackManager potentialLeader = null;
        List<PackManager> packTargets = new List<PackManager>();
        List<PackManager> freeTargets = new List<PackManager>();
        float closestTargetDistance = float.MaxValue;
        NavMeshPath path = null;

        foreach (var hitCollider in inSenseRadius)
        {
            //Build lists with proper types
            PackManager sensedUnit = hitCollider.GetComponent<PackManager>();
            if (sensedUnit != null && sensedUnit.packSpecies == unit.species)
            {
                packTargets.Add(sensedUnit);
            }
        }

        foreach (PackManager packUnit in packTargets)
        {
            path = new NavMeshPath();

            if (packUnit != null && NavMesh.CalculatePath(transform.position, packUnit.GetComponent<Collider>().ClosestPoint(transform.position), unit.controller.navMeshAgent.areaMask, path))
            {
                if(!packUnit.hasPack)
                {
                    freeTargets.Add(packUnit);
                }
                float distance = Vector3.Distance(transform.position, path.corners[0]);
                for (int i = 1; i < path.corners.Length; i++)
                {
                    distance += Vector3.Distance(path.corners[i - 1], path.corners[i]);
                }
                if (packUnit.isLeader && packUnit != this && packUnit.Pack.Count < packUnit.PackSize && distance < closestTargetDistance && path.status == NavMeshPathStatus.PathComplete)
                {
                    closestTargetDistance = distance;
                    potentialLeader = packUnit;
                }
            }
        }

        if(potentialLeader != null)
        {
            if(IsLeader)
            {
                if((Pack.Count + potentialLeader.Pack.Count) <= PackSize)
                {
                    Debug.Log("Merging pack number of: " + Pack.Count + " with senocd pack iwth: " + potentialLeader.Pack.Count);
                    MergePacks(potentialLeader);
                    return;
                }
                else
                {
                    Debug.Log("Packs are too big to merge.");
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

        foreach(PackManager packUnit in freeTargets)
        {
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
}
