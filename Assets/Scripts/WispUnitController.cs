using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispUnitController : UnitController
{
    protected override void Birth()
    {
        if (unit.IsAdult && unit.Gens.IsFemale)
        {
            int offspringQuantity = rand.Next((int)unit.Gens.OffspringMaxPopulation);

            for (int i = 0; i < offspringQuantity; i++)
            {
                WispEgg offspring;
                GameObject evolvingPrefab;
                GenSample newGen = GenManager.Instance.InheritGens(unit.Gens, unit.LastPartnerGenSample, 0.1f);

                //Place eggs around mother
                float placementRange = 1f;
                float theta = (float)(2 * Math.PI / offspringQuantity) * i;
                Vector3 spawnPosition = new Vector3(transform.position.x + (float)Math.Cos(theta) * placementRange, transform.position.y, transform.position.z + (float)Math.Sin(theta) * placementRange);

                // 50% chance for gender
                if (newGen.IsFemale)
                {
                    evolvingPrefab = unit.femaleOffspringPrefab;
                }
                else
                {
                    evolvingPrefab = unit.maleOffspringPrefab;
                }
                //TODO: Spread spawn location around mother
                offspring = Instantiate(unit.GetComponent<WispUnit>().eggPrefab, spawnPosition, Quaternion.identity).GetComponent<WispEgg>();
                offspring.Initialize(newGen, evolvingPrefab, newGen.MaxHealth, newGen.HungerTreshold, newGen.ThirstTreshold);
            }
        }
        unit.IsPregnant = false;
        unit.LastPartnerGenSample = null;
    }
}
