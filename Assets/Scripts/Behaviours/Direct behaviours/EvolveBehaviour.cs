using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolveBehaviour : BaseBehaviour
{
    public override void Behave(Action onBehaviourComplete)
    {
        if (_unit.evolvedUnitPrefab == null)
        {
            _unitController.Death();
        }
        else
        {
            Unit evolvedUnit = Instantiate(_unit.evolvedUnitPrefab, transform.position, Quaternion.identity).GetComponent<Unit>();
            evolvedUnit.Initialize(_unit.Gens, _unit.Health, _unit.Hunger, _unit.Thirst);

            if (_unitController.PackManager.HasPack)
            {
                UnitPackManager evolvedPackMember = evolvedUnit.GetComponent<UnitPackManager>();
                if (_unitController.PackManager.IsLeader)
                {
                    evolvedPackMember.IsLeader = true;
                    evolvedPackMember.HasPack = true;
                    evolvedPackMember.Pack = new List<UnitPackManager>
                    {
                        evolvedPackMember
                    };
                    _unitController.PackManager.Pack.Remove(_unitController.PackManager);
                    foreach (UnitPackManager packMember in _unitController.PackManager.Pack)
                    {
                        packMember.PackLeader = evolvedPackMember;
                        packMember.UnsubscribePackChnageHandler();
                        evolvedPackMember.Pack.Add(packMember);
                        packMember.SubscribePackChnageHandler(evolvedPackMember);
                    }
                }
                else
                {
                    _unitController.PackManager.PackLeader.Pack.Remove(_unitController.PackManager);
                    _unitController.PackManager.PackLeader.Pack.Add(evolvedPackMember);
                    evolvedPackMember.PackLeader = _unitController.PackManager.PackLeader;
                    evolvedPackMember.HasPack = true;
                }
                evolvedPackMember.UnitController.IsControlled = _unitController.IsControlled;
            }
            _unitController.DestroyAndUnsubscribe();
        }
    }

    protected override float CalculateBehaviourScore()
    {
        if(_unit.RemainingStageLifeTime <= 0)
        {
            return criticalScoreValue * 2 * behaviourPriority;
        }
        else
        {
            return 0;
        }
    }
}
