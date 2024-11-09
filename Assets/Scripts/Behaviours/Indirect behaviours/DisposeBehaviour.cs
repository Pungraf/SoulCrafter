using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisposeBehaviour : BaseBehaviour
{
    public override void Behave(Action onBehaviourComplete)
    {
        BehaviourStart(onBehaviourComplete);

        Destroy(Instantiate(_unit.wastePrefab, _unit.transform.position, Quaternion.identity), 1f);

        RaycastHit hit;
        FertilePlane fertilePlane;
        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1f);
        if (hit.collider != null && hit.transform.TryGetComponent(out fertilePlane))
        {
            fertilePlane.Fertilize(_unit.Waste);
        }
        _unit.Dispose();

        BehaviourComplete();
    }

    protected override float CalculateBehaviourScore()
    {
        if(_unit.Waste >= 1f)
        {
            return _unit.Waste * behaviourPriority + criticalScoreValue;
        }
        return _unit.Waste * behaviourPriority;
    }
}
