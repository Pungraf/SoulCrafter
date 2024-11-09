using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardBehaviour : BaseBehaviour
{
    public bool IsGuarding = false;
    public Vector3 GuardingLocation;

    public override void Behave(Action onBehaviourComplete)
    {
        BehaviourStart(onBehaviourComplete);

        GuardingLocation = behaviourLocation;

        Vector3 guardingLoaction = UnityEngine.Random.insideUnitSphere * _unit.Gens.Speed;
        guardingLoaction += GuardingLocation;
        _unitController.MoveUnit(guardingLoaction);
        IsGuarding = true;
    }

    protected override float CalculateBehaviourScore()
    {
        if (IsGuarding)
        {
            return criticalScoreValue * behaviourPriority;
        }
        else
        {
            return -1;
        }
    }

    public override void ClearPersistentData()
    {
        IsGuarding = false;
    }
}
