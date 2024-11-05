using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepBehaviour : BaseBehaviour
{
    public override void Behave(Action onBehaviourComplete)
    {
        BehaviourStart(onBehaviourComplete);
        Debug.Log("Sleeping");
    }

    protected override int CalculateBehaviourScore()
    {
        return (int)(100 - _unit.Energy);
    }
}
