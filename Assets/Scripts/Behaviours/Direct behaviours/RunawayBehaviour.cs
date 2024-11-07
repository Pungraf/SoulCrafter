using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunawayBehaviour : BaseBehaviour
{
    private Transform dangerSource;
    public override void Behave(Action onBehaviourComplete)
    {
        BehaviourStart(onBehaviourComplete);

        if(dangerSource != null)
        {
            RunAway(dangerSource);
            return;
        }

        BehaviourComplete();
    }

    protected override float CalculateBehaviourScore()
    {
        if(SensedDanger())
        {
            return criticalScoreValue;
        }
        else
        {
            return -1;
        }
    }

    protected bool SensedDanger()
    {
        Collider[] inSenseRadius = Physics.OverlapSphere(transform.position, _unit.Gens.Perception);
        Transform predatorUnit = SensePredator(inSenseRadius);
        if (predatorUnit != null)
        {
            dangerSource = predatorUnit;
            return true;
        }

        return false;
    }

    protected Transform SensePredator(Collider[] sensedObjects)
    {
        Transform potentialPredator = null;
        List<Transform> predatorTargets = new List<Transform>();

        foreach (Collider hitCollider in sensedObjects)
        {
            Transform sensedTransform = hitCollider.transform;
            Unit sensedUnit = sensedTransform.GetComponent<Unit>();
            if (sensedUnit != null && _unit.predators.Contains(sensedUnit.species))
            {
                predatorTargets.Add(sensedTransform);
            }
        }

        potentialPredator = _unitController.FindClosestTransformPath(predatorTargets);

        if (potentialPredator != null)
        {
            return potentialPredator;
        }
        return null;
    }

    protected void RunAway(Transform runAwayTarget)
    {
        _unit.targetedTransform = runAwayTarget;

        Vector3 directionAway = (transform.position - runAwayTarget.position).normalized * _unit.Gens.Speed * 10f;
        directionAway += transform.position;
        isAwatingPathCallback = true;
        _unitController.MoveUnit(directionAway);
    }
}
