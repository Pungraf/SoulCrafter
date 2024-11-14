using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPackManager : PackManager
{

    private Unit unit;
    private UnitController unitController;
    public UnitController UnitController
    {
        get { return unitController; }
    }
    private Unit.Species packSpecies;

    public Unit.Species PackSpecies
    {
        get { return packSpecies; }
    }

    private void Awake()
    {
        unit = GetComponent<Unit>();
        unitController = GetComponent<UnitController>();
        packSpecies = unit.species;
    }


    public void LookForPack()
    {
        Collider[] inSenseRadius = Physics.OverlapSphere(transform.position, unit.Gens.Perception.Value);
        Transform potentialLeaderTransform = null;
        List<Transform> packTargets = new List<Transform>();
        List<Transform> freeTargets = new List<Transform>();

        foreach (var hitCollider in inSenseRadius)
        {
            //Build lists with proper types
            UnitPackManager sensedUnit = hitCollider.GetComponent<UnitPackManager>();
            if (sensedUnit != null && sensedUnit.packSpecies == unit.species && sensedUnit.IsLeader && sensedUnit != this && sensedUnit.Pack.Count < sensedUnit.PackSize)
            {
                packTargets.Add(sensedUnit.transform);
            }
            if (sensedUnit != null && sensedUnit.packSpecies == unit.species && !sensedUnit.HasPack)
            {
                freeTargets.Add(sensedUnit.transform);
            }
        }

        potentialLeaderTransform = unitController.FindClosestTransformPath(packTargets);

        if (potentialLeaderTransform != null)
        {
            UnitPackManager potentialLeader = potentialLeaderTransform.GetComponent<UnitPackManager>();
            if (IsLeader)
            {
                if ((Pack.Count + potentialLeader.Pack.Count) <= PackSize)
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
        if (IsLeader)
        {
            return;
        }

        int highestGenValue = 0;
        UnitPackManager newPackLeader = null;

        foreach (Transform packTransform in freeTargets)
        {
            UnitPackManager packUnit = packTransform.GetComponent<UnitPackManager>();
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

        if (newPackLeader != null)
        {
            newPackLeader.IsLeader = true;
            newPackLeader.HasPack = true;
            newPackLeader.Pack.Clear();
            newPackLeader.Pack.Add(newPackLeader);
            newPackLeader.PackLeader = newPackLeader;
        }
    }

    public void MergePacks(UnitPackManager secondLeader)
    {
        if (unit.genScore >= secondLeader.unit.genScore)
        {
            secondLeader.IsLeader = false;
            foreach (UnitPackManager packUnit in secondLeader.Pack)
            {
                Pack.Add(packUnit);
                packUnit.PackLeader = this;
            }
            secondLeader.Pack.Clear();
        }
        else
        {
            IsLeader = false;
            foreach (UnitPackManager packUnit in Pack)
            {
                secondLeader.Pack.Add(packUnit);
                packUnit.PackLeader = secondLeader;
            }
            Pack.Clear();
        }
    }

    public void JoinPlayer(PlayerPackManager playerPackManager)
    {
        if (playerPackManager.Pack.Count >= playerPackManager.PackSize)
        {
            Debug.Log("Player has already full team");
            return;
        }
        else if(playerPackManager.Pack.Contains(this))
        {
            Debug.Log("Already in players pack");
            return;
        }
        if (IsLeader)
        {
            IsLeader = false;
            foreach (UnitPackManager packUnit in Pack)
            {
                packUnit.PackLeader = null;
                packUnit.HasPack = false;
            }
            Pack.Clear();
        }
        else
        {
            if (HasPack)
            {
                PackLeader.Pack.Remove(this);
                PackLeader = null;
                HasPack = false;
            }
        }

        PackLeader = playerPackManager;
        PackLeader.Pack.Add(this);
        HasPack = true;
        unitController.IsControlled = true;
        //4 is traversable tag of Player restricted area
        UnitController.ResetTraversableTag();
        UnitController.AddTraversableTag(4);
    }

    public void DisbandPlayer()
    {
        UnsubscribePackChnageHandler();
        PackLeader = null;
        HasPack = false;
        unitController.IsControlled = false;
        UnitController.Brain.ClearAllPeristentBehaviours();
        if(UnitController.GetTileTagNumberBeneath() == 4)
        {
            //Set only Player restricted area as walkable
            UnitController.SetTraversableMask(new List<byte>() { 4 });
        }
        else
        {
            UnitController.ResetTraversableTag();
        }
    }

    public override void DisbandPack()
    {
        foreach (UnitPackManager packMember in Pack)
        {
            packMember.UnitController.Brain.ClearAllPeristentBehaviours();
            packMember.PackLeader = null;
            packMember.HasPack = false;
        }
        base.DisbandPack();
    }
}
