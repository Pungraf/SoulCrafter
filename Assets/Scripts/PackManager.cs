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
        set { pack = value; }
    }

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
                if (packUnit.isLeader && packUnit.Pack.Count < packUnit.PackSize && distance < closestTargetDistance && path.status == NavMeshPathStatus.PathComplete)
                {
                    closestTargetDistance = distance;
                    potentialLeader = packUnit;
                }
            }
        }

        if(potentialLeader != null)
        {
            potentialLeader.Pack.Add(this);
            pack = potentialLeader.Pack;
            PackLeader = potentialLeader;
            HasPack = true;
            return;
        }

        int highestGenValue = 0;
        PackManager newPackLeader = null;

        foreach(PackManager packUnit in freeTargets)
        {
            if(packUnit != null && packUnit.unit.genScore > highestGenValue)
            {
                newPackLeader = packUnit;
                highestGenValue = packUnit.unit.genScore;
            }
        }

        if(newPackLeader != null)
        {
            freeTargets.Remove(newPackLeader);
            newPackLeader.IsLeader = true;
            newPackLeader.PackLeader = newPackLeader;
        }
        else
        {
            return;
        }

        foreach (PackManager packUnit in freeTargets)
        {
            if(newPackLeader.Pack.Count < newPackLeader.PackSize)
            {
                newPackLeader.Pack.Add(packUnit);
                packUnit.Pack = newPackLeader.Pack;
                packUnit.hasPack = true;
                packUnit.PackLeader = newPackLeader;
            }
        }
    }
}
