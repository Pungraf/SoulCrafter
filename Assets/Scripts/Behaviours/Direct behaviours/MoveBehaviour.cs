using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBehaviour : BaseBehaviour
{
    public override void Behave(Action onBehaviourComplete)
    {
        BehaviourStart(onBehaviourComplete);

        if(behaviourLocation == null)
        {
            Debug.LogError("Invalid Move target !");
            BehaviourComplete();
            return;
        }

        isAwatingPathCallback = true;
        _unitController.MoveUnit(behaviourLocation);
    }

    protected override int CalculateBehaviourScore()
    {
        //Only commanded behaviour
        return -1;
    }
}
