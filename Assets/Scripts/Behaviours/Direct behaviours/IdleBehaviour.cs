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

        if(_unit.IsAgressive)
        {
            _unit.Anger += 1f;
        }

        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * _unit.Gens.Speed * 10f;
        if (_unitController.PackManager.PackLeader != null && _unitController.PackManager.HasPack && !_unitController.PackManager.IsLeader)
        {
            randomDirection += _unitController.PackManager.PackLeader.transform.position;
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
