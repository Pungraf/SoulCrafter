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

            if (_unitController.packManager.HasPack)
            {
                PackManager evolvedPackMember = evolvedUnit.GetComponent<PackManager>();
                if (_unitController.packManager.IsLeader)
                {
                    evolvedPackMember.IsLeader = true;
                    evolvedPackMember.HasPack = true;
                    evolvedPackMember.Pack = new List<PackManager>
                    {
                        evolvedPackMember
                    };
                    _unitController.packManager.Pack.Remove(_unitController.packManager);
                    foreach (PackManager packMember in _unitController.packManager.Pack)
                    {
                        packMember.PackLeader = evolvedPackMember;
                        packMember.UnsubscribePackChnageHandler();
                        evolvedPackMember.Pack.Add(packMember);
                        packMember.SubscribePackChnageHandler(evolvedPackMember);
                    }
                }
                else
                {
                    _unitController.packManager.PackLeader.Pack.Remove(_unitController.packManager);
                    _unitController.packManager.PackLeader.Pack.Add(evolvedPackMember);
                    evolvedPackMember.PackLeader = _unitController.packManager.PackLeader;
                    evolvedPackMember.HasPack = true;
                }


            }
            _unitController.DestroyAndUnsubscribe();
        }
    }

    protected override int CalculateBehaviourScore()
    {
        if(_unit.RemainingStageLifeTime <= 0)
        {
            return (int)criticalScoreValue * 2;
        }
        else
        {
            return 0;
        }
    }
}
