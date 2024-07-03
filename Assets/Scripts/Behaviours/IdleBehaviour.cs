using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleBehaviour : BaseBehaviour
{
    private void Start()
    {
        _unitController.OnDestinationReached += BehaviourComplete;
    }

    public override void Behave(Action onBehaviourComplete)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * _unit.Gens.Speed * 10f;
        if (_unitController.packManager.PackLeader != null && _unitController.packManager.HasPack && !_unitController.packManager.IsLeader)
        {
            randomDirection += _unitController.packManager.PackLeader.transform.position;
        }
        else
        {
            randomDirection += transform.position;
        }

        _unitController.MoveUnit(randomDirection);

        BehaviourStart(onBehaviourComplete);
    }

    protected override int CalculateBehaviourScore()
    {
        return 10;
    }
}
