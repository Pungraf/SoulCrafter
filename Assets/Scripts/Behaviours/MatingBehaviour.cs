using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CopulatingBehaviour))]
[RequireComponent(typeof(BirthBehaviour))]
public class MatingBehaviour : BaseBehaviour
{
    protected override void Start()
    {
        _unitController.OnDestinationReached += PerformCopulation;
    }

    public override void Behave(Action onBehaviourComplete)
    {
        BehaviourStart(onBehaviourComplete);

        if (_unit.IsFemale)
        {
            FemaleMating();
            return;
        }
        else
        {
            Collider[] inSenseRadius = Physics.OverlapSphere(transform.position, _unit.Gens.Perception);

            Transform potentialMatingTarget = null;
            List<Transform> unitTargets = new List<Transform>();

            foreach (var hitCollider in inSenseRadius)
            {
                UnitController sensedUnit = hitCollider.GetComponent<UnitController>();
                if (sensedUnit != null && sensedUnit != _unit && sensedUnit.CurrentBehaviour == Behaviour.Mate && sensedUnit.Unit.IsFemale)
                {
                    unitTargets.Add(sensedUnit.transform);
                }
            }
            potentialMatingTarget = _unitController.FindClosestTransformPath(unitTargets);
            if (potentialMatingTarget != null)
            {
                MatingBehaviour matingTargetBehaviour = (MatingBehaviour)potentialMatingTarget.GetComponent<UnitController>().Brain.GetBehaviourByType(Behaviour.Mate);
                if (matingTargetBehaviour != null && matingTargetBehaviour.ProposeMating(_unit))
                {
                    MaleMating(potentialMatingTarget.transform);
                    return;
                }
            }
        }

        BehaviourComplete(Behaviour.Idle);
    }

    protected override int CalculateBehaviourScore()
    {
        if(!_unit.IsPregnant)
        {
            return (int)_unit.Urge;
        }
        //Do net performe while pregnant
        return -1;
    }

    protected void FemaleMating()
    {
        if (_unit.IsPregnant)
        {
            BehaviourComplete();
        }
    }
    protected void MaleMating(Transform matingTarget)
    {
        _unitController.MoveUnit(matingTarget.position);
        matingTarget.GetComponent<Unit>().targetedTransform = _unit.transform;
        _unit.targetedTransform = matingTarget;
        isAwatingPathCallback = true;
    }

    public bool ProposeMating(Unit offeredMatting)
    {
        if (_unitController.CurrentBehaviour == Behaviour.Mate && _unit.IsFemale)
        {
            double randomDouble = _unitController.Rand.NextDouble();

            if (randomDouble < offeredMatting.Gens.Attractiveness)
            {
                CancelInvoke("DeprecatedBehaviour");
                Invoke("DeprecatedBehaviour", behaviourTimeLimit);
                return true;
            }
            else
            {
                offeredMatting.Urge = 0;
                return false;
            }
        }
        return false;
    }

    protected void PerformCopulation(object sender, EventArgs e)
    {
        if (isActive && isAwatingPathCallback && !_unit.IsFemale)
        {
            if(Vector3.Distance(_unit.transform.position, _unit.targetedTransform.position) > _unit.Gens.Reach)
            {
                return;
            }
            isAwatingPathCallback = false;
            _unitController.Brain.ForceNextBehaviour(Behaviour.Copulate, true);
        }
    }
}
