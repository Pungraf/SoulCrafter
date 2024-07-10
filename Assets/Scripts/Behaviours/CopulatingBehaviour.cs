using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopulatingBehaviour : BaseBehaviour
{
    public override void Behave(Action onBehaviourComplete)
    {
        BehaviourStart(onBehaviourComplete);
        Collider[] inInteractRadius = Physics.OverlapSphere(transform.position, _unit.Gens.Reach);

        foreach (var hitCollider in inInteractRadius)
        {
            UnitController potentialCopulateTarget = hitCollider.GetComponent<UnitController>();
            if (potentialCopulateTarget != null)
            {
                try
                {
                    if (potentialCopulateTarget.transform != _unit.transform && potentialCopulateTarget.Unit.targetedTransform == _unit.transform)
                    {
                        Copulate();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
        }
    }

    protected override int CalculateBehaviourScore()
    {
        //Only direct driven
        return -1;
    }

    private void Copulate()
    {
        //TODO: Add animation
        _unit.Urge = 0f;
        if (_unit.IsFemale)
        {
            if(_unit.targetedTransform.GetComponent<Unit>().Gens.Fecundity > _unitController.Rand.NextDouble())
            {
                _unit.PregnancyCounter = _unit.Gens.Gestation;
                _unit.IsPregnant = true;
            }
        }
        else
        {
            if (_unit.targetedTransform != null)
            {
                _unit.targetedTransform.GetComponent<UnitController>().Brain.ForceNextBehaviour(Behaviour.Copulate, true);
                _unit.targetedTransform.GetComponent<Unit>().LastPartnerGenSample = _unit.Gens;
            }
        }
    }
}
