using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepBehaviour : BaseBehaviour
{
    public override void Behave(Action onBehaviourComplete)
    {
        BehaviourStart(onBehaviourComplete);
    }

    protected override float CalculateBehaviourScore()
    {
        if(_unit.Energy == 0)
        {
            return (1 - _unit.Energy) + criticalScoreValue * behaviourPriority;
        }
        return 1 - _unit.Energy * behaviourPriority;
    }

    protected override void DeprecatedBehaviour()
    {
        if(_brain.GetFirstBehaviour().behaviourType == BaseBehaviour.Behaviour.Sleep)
        {
            Invoke("DeprecatedBehaviour", behaviourTimeLimit);
        }
        else
        {
            base.DeprecatedBehaviour();
        }
    }
}
