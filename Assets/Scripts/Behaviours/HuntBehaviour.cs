using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntBehaviour : BaseBehaviour
{
    public override void Behave(Action onBehaviourComplete)
    {
        BehaviourStart(onBehaviourComplete);

        Collider[] inSenseRadius = Physics.OverlapSphere(transform.position, _unit.Gens.Perception);
        Transform potentialHuntTarget = null;
        List<Transform> preyTargets = new List<Transform>();

        foreach (var hitCollider in inSenseRadius)
        {
            //Build lists with proper types
            Transform sensedTransform = hitCollider.transform;
            Unit unitTransform = sensedTransform.GetComponent<Unit>();
            if (unitTransform != null && _unit.foodChainSpecies.Contains(unitTransform.species))
            {
                preyTargets.Add(sensedTransform);
            }
        }
        //Search for closest prey
        potentialHuntTarget = _unitController.FindClosestTransformPath(preyTargets);
        if (potentialHuntTarget != null)
        {
            StartCoroutine(Hunt(potentialHuntTarget));
            return;
        }

        BehaviourComplete(Behaviour.Idle);
    }

    protected override int CalculateBehaviourScore()
    {
        return (int)_unit.Anger;
    }

    IEnumerator Hunt(Transform huntedUnit)
    {
        Unit huntTarget = huntedUnit.GetComponent<Unit>();
        _unit.targetedTransform = huntTarget.transform;
        while (huntTarget != null)
        {
            //TODO: Change detection mask here, nad in other Overlaps
            Collider[] inInteractRadius = Physics.OverlapSphere(transform.position, _unit.Gens.Reach);
            foreach (var hitCollider in inInteractRadius)
            {
                Unit unitInRange = hitCollider.GetComponent<Unit>();
                if (unitInRange != null && unitInRange != _unit && _unit.foodChainSpecies.Contains(unitInRange.species))
                {
                    huntTarget = unitInRange;
                    _unit.targetedTransform = huntTarget.transform;
                    //TODO: Change for attack speed
                    yield return StartCoroutine(Attack(huntTarget.transform));
                    break;
                }
            }

            if (huntTarget != null)
            {
                _unitController.MoveUnit(huntTarget.transform.position);
                yield return new WaitForSeconds(0.5f);
            }
        }

        BehaviourComplete();
    }

    IEnumerator Attack(Transform targetUnit)
    {
        _unitController.aIPath.enabled = false;
        Vector3 originalPosition = _unitController.transform.position;
        Vector3 dirToTarget = (targetUnit.position - _unitController.transform.position).normalized;
        //TODO: parameter for uni radius
        Vector3 attackPosition = targetUnit.position - dirToTarget * (0.5f + 0.5f / 2);

        float attackSpeed = 3;
        float percent = 0;

        bool hasAppliedDamage = false;

        while (percent <= 1)
        {
            if (percent >= .5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetUnit.GetComponent<Unit>().TakeDamage(_unit.Gens.Strength);
            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            _unitController.transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            yield return null;
        }

        _unitController.aIPath.enabled = true;
    }
}
