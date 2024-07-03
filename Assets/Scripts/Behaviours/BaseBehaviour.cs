using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseBehaviour : MonoBehaviour
{
    public int currnetBehaviourScore;

    protected Unit _unit;
    protected UnitController _unitController;
    protected Action onBehaviourComplete;
    //TODO: Change for event ?
    protected bool isActive = false;

    protected virtual void Awake()
    {
        _unit = GetComponent<Unit>();
        _unitController = GetComponent<UnitController>();
    }

    public abstract void Behave(Action onBehaviourComplete);

    public void CalculateCurrentBehaviourScore()
    {
        currnetBehaviourScore = CalculateBehaviourScore();
    }

    protected abstract int CalculateBehaviourScore();

    protected void BehaviourStart(Action OnBehaviourComplete)
    {
        isActive = true;
        this.onBehaviourComplete = OnBehaviourComplete;
    }

    protected void BehaviourComplete(object sender, EventArgs e)
    {
        isActive = false;
        onBehaviourComplete();
    }
}
