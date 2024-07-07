using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleBehaviour : BaseBehaviour
{
    public override void Behave(Action onBehaviourComplete)
    {
        BehaviourStart(onBehaviourComplete);

        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * _unit.Gens.Speed * 10f;
        if (_unitController.packManager.PackLeader != null && _unitController.packManager.HasPack && !_unitController.packManager.IsLeader)
        {
            randomDirection += _unitController.packManager.PackLeader.transform.position;
        }
        else
        {
            randomDirection += transform.position;
        }
        isAwatingPathCallback = true;
        _unitController.MoveUnit(randomDirection);
    }

    protected override int CalculateBehaviourScore()
    {
        return 10;
    }
}
