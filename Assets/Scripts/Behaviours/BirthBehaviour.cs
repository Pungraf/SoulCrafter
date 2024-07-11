using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirthBehaviour : BaseBehaviour
{
    public override void Behave(Action onBehaviourComplete)
    {
        BehaviourStart(onBehaviourComplete);
        if (_unit.IsPregnant && _unit.IsFemale)
        {
            int offspringQuantity = _unitController.Rand.Next(1, (int)_unit.Gens.Fertility);

            for (int i = 0; i < offspringQuantity; i++)
            {
                WispEgg offspring;
                GameObject evolvingPrefab;
                GenSample newGen = GenManager.Instance.InheritGens(_unit.Gens, _unit.LastPartnerGenSample, 0.1f);

                //Place eggs around mother
                float placementRange = 1f;
                float theta = (float)(2 * Math.PI / offspringQuantity) * i;
                Vector3 spawnPosition = new Vector3(transform.position.x + (float)Math.Cos(theta) * placementRange, transform.position.y, transform.position.z + (float)Math.Sin(theta) * placementRange);

                // 50% chance for gender
                if (0.5f > _unitController.Rand.NextDouble())
                {
                    evolvingPrefab = _unit.femaleOffspringPrefab;
                }
                else
                {
                    evolvingPrefab = _unit.maleOffspringPrefab;
                }
                //TODO: Spread spawn location around mother
                offspring = Instantiate(_unit.GetComponent<WispUnit>().eggPrefab, spawnPosition, Quaternion.identity).GetComponent<WispEgg>();
                offspring.Initialize(newGen, evolvingPrefab, newGen.Vitality);
            }

            _unit.IsPregnant = false;
            _unit.LastPartnerGenSample = null;
        }
        BehaviourComplete();
    }

    protected override int CalculateBehaviourScore()
    {
        if(_unit.IsPregnant && _unit.PregnancyCounter < 0)
        {
            return (int)criticalScoreValue;
        }
        return -1;
    }
}
