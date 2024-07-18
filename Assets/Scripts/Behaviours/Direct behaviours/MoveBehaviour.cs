using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBehaviour : BaseBehaviour
{
    public override void Behave(Action onBehaviourComplete)
    {
        BehaviourStart(onBehaviourComplete);

        Vector3 destination = MouseWorld.GetPosition();

        isAwatingPathCallback = true;
        _unitController.MoveUnit(destination);
    }

    protected override int CalculateBehaviourScore()
    {
        //Only commanded behaviour
        return -1;
    }
}
