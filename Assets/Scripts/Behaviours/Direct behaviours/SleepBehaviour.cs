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

    protected override int CalculateBehaviourScore()
    {
        return (int)(100 - _unit.Energy);
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
