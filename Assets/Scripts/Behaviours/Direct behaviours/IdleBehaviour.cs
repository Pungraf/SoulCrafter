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
        if (_unitController.PackManager.PackLeader != null && _unitController.PackManager.HasPack && !_unitController.PackManager.IsLeader)
        {
            if(_unitController.IsControlled)
            {
                randomDirection = randomDirection.normalized * 5;
            }
            randomDirection += _unitController.PackManager.PackLeader.transform.position;
        }
        else
        {
            randomDirection += transform.position;
        }
        isAwatingPathCallback = true;
        _unitController.MoveUnit(randomDirection);
    }

    protected override float CalculateBehaviourScore()
    {
        return 0.1f;
    }
}
