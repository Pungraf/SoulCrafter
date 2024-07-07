using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class BaseBehaviour : MonoBehaviour
{
    public enum Behaviour
    {
        Idle,
        Feed
    }

    public int currnetBehaviourScore;

    protected Unit _unit;
    protected UnitController _unitController;
    protected Action onBehaviourComplete;
    protected float criticalScoreValue = 100f;
    protected String followUpBehaviourName;
    protected bool isAwatingPathCallback = false;
    //TODO: Change for event ?
    [SerializeField] protected bool isActive = false;

    public Behaviour behaviourType;

    protected virtual void Awake()
    {
        _unit = GetComponentInParent<Unit>();
        _unitController = GetComponentInParent<UnitController>();
    }

    protected virtual void Start()
    {
        _unitController.OnDestinationReached += BehaviourComplete;
    }

    public abstract void Behave(Action onBehaviourComplete);

    public void CalculateCurrentBehaviourScore()
    {
        currnetBehaviourScore = CalculateBehaviourScore();
    }

    protected abstract int CalculateBehaviourScore();

    protected void BehaviourStart(Action OnBehaviourComplete)
    {
        isAwatingPathCallback = false;
        isActive = true;
        this.onBehaviourComplete = OnBehaviourComplete;
    }

    protected void BehaviourComplete(object sender, EventArgs e)
    {
        if(isActive && isAwatingPathCallback)
        {
            isAwatingPathCallback = false;
            isActive = false;
            onBehaviourComplete();
        }
    }
    protected void BehaviourComplete()
    {
        if (isActive)
        {
            isAwatingPathCallback = false;
            isActive = false;
            onBehaviourComplete();
        }
    }

    protected void BehaviourComplete(Behaviour behaviour)
    {
        if (isActive)
        {
            isAwatingPathCallback = false;
            isActive = false;
            _unitController.ChooseBehaviour(behaviour);
        }
    }
}
