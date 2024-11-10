using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncapacitatedBehaviour : BaseBehaviour
{
    public override void Behave(Action onBehaviourComplete)
    {
        BehaviourStart(onBehaviourComplete);
    }

    protected override float CalculateBehaviourScore()
    {
        //Only commanded behaviour
        return -1;
    }
}
