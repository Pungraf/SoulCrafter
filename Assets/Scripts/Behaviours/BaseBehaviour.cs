using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class BaseBehaviour : MonoBehaviour
{
    public enum Behaviour
    {
        None,
        Idle,
        Feed,
        Mate,
        Copulate,
        RunningAway,
        Hunting,
        Move
    }

    public int currnetBehaviourScore;

    [SerializeField] protected float behaviourTimeLimit;

    protected Unit _unit;
    protected UnitController _unitController;
    protected Brain _brain;
    protected Action onBehaviourComplete;
    protected float criticalScoreValue = 100f;
    protected bool isAwatingPathCallback = false;
    [SerializeField] protected bool isActive = false;

    protected Vector3 behaviourLocation;
    public Vector3 BehaviourLocation
    {
        get { return behaviourLocation; }
        set { behaviourLocation = value; }
    }

    public bool IsActive
    {
        get { return isActive; }
    }

    public Behaviour behaviourType;

    protected virtual void Awake()
    {
        _unit = GetComponentInParent<Unit>();
        _unitController = GetComponentInParent<UnitController>();
        _brain = GetComponent<Brain>();
    }

    protected virtual void Start()
    {
        _unitController.OnDestinationReached += BehaviourComplete;
    }

    public abstract void Behave(Action onBehaviourComplete);
    public virtual void Behave(Action onBehaviourComplete, Vector3 behaviourLocation)
    {
        this.behaviourLocation = behaviourLocation;
        Behave(onBehaviourComplete);
    }

    public void CalculateCurrentBehaviourScore()
    {
        currnetBehaviourScore = CalculateBehaviourScore();
    }

    protected abstract int CalculateBehaviourScore();

    protected void BehaviourStart(Action OnBehaviourComplete)
    {
        isAwatingPathCallback = false;
        isActive = true;
        _unitController.CurrentBehaviour = behaviourType;
        _brain.CurrentBehaviour = this;
        this.onBehaviourComplete = OnBehaviourComplete;
        Invoke("DeprecatedBehaviour", behaviourTimeLimit);
    }

    protected void BehaviourComplete(object sender, EventArgs e)
    {
        if(isActive && isAwatingPathCallback)
        {
            CancelInvoke("DeprecatedBehaviour");
            isAwatingPathCallback = false;
            isActive = false;
            _brain.CurrentBehaviour = null;
            _unitController.CurrentBehaviour = Behaviour.None;
            _unit.targetedTransform = null;
            onBehaviourComplete();
        }
    }
    protected void BehaviourComplete()
    {
        if (isActive)
        {
            CancelInvoke("DeprecatedBehaviour");
            isAwatingPathCallback = false;
            isActive = false;
            _brain.CurrentBehaviour = null;
            _unitController.CurrentBehaviour = Behaviour.None;
            _unit.targetedTransform = null;
            onBehaviourComplete();
        }
    }

    public void BehaviourComplete(Behaviour behaviour, bool keepTarget = false)
    {
        if (isActive)
        {
            CancelInvoke("DeprecatedBehaviour");
            isAwatingPathCallback = false;
            isActive = false;
            _brain.CurrentBehaviour = null;
            _unitController.CurrentBehaviour = Behaviour.None;
            if (!keepTarget)
            {
                _unit.targetedTransform = null;
            }
            _unitController.ChooseBehaviour(behaviour);
        }
    }

    public void BehaviourComplete(Behaviour behaviour, Vector3 behaviourLocation)
    {
        if (isActive)
        {
            CancelInvoke("DeprecatedBehaviour");
            isAwatingPathCallback = false;
            isActive = false;
            _brain.CurrentBehaviour = null;
            _unitController.CurrentBehaviour = Behaviour.None;
            _unitController.ChooseBehaviour(behaviour, behaviourLocation);
        }
    }

    protected void DeprecatedBehaviour()
    {
        BehaviourComplete();
    }
}
