using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenManager : MonoBehaviour
{
    public static GenManager Instance { get; private set; }

    private GenMerger genMerged;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one GenManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private float RandomOverRange(float firstValue, float secondValue, float mod)
    {
        float minValue = 0f;
        float maxValue = 0f;
        if (firstValue > secondValue)
        {
            minValue = secondValue * (1f - mod);
            maxValue = firstValue * (1f + mod);
        }
        else
        {
            minValue = firstValue * (1f - mod);
            maxValue = secondValue * (1f + mod);
        }

        if(minValue == firstValue || minValue == secondValue)
        {
            minValue -= 0.1f;
        }
        if (maxValue == firstValue || maxValue == secondValue)
        {
            maxValue += 0.1f;
        }
        if (minValue < 0f)
        {
            minValue = 0f;
        }

        return UnityEngine.Random.Range(minValue, maxValue);
    }

    public GenSample InheritGens(GenSample firstSample, GenSample secondSample, float rangeModifier)
    {
        GenSample newGen = new GenSample();

        if(firstSample.Species != secondSample.Species)
        {
            Debug.Log("Wrong spiecies gens !");
            return null;
        }

        newGen.Species = firstSample.Species;
        newGen.LifeTime = RandomOverRange(firstSample.LifeTime, secondSample.LifeTime, rangeModifier);
        newGen.MaxHealth = RandomOverRange(firstSample.MaxHealth, secondSample.MaxHealth, rangeModifier);
        newGen.HungerTreshold = RandomOverRange(firstSample.HungerTreshold, secondSample.HungerTreshold, rangeModifier);
        newGen.HungerResistance = RandomOverRange(firstSample.HungerResistance, secondSample.HungerResistance, rangeModifier);
        newGen.ThirstTreshold = RandomOverRange(firstSample.ThirstTreshold, secondSample.ThirstTreshold, rangeModifier);
        newGen.ThirstResistance = RandomOverRange(firstSample.ThirstResistance, secondSample.ThirstResistance, rangeModifier);
        newGen.UrgeTreshold = RandomOverRange(firstSample.UrgeTreshold, secondSample.UrgeTreshold, rangeModifier);
        newGen.WalkRadius = RandomOverRange(firstSample.WalkRadius, secondSample.WalkRadius, rangeModifier);
        newGen.WalkSpeed = RandomOverRange(firstSample.WalkSpeed, secondSample.WalkSpeed, rangeModifier);
        newGen.SenseRadius = RandomOverRange(firstSample.SenseRadius, secondSample.SenseRadius, rangeModifier);
        newGen.InteractionRadius = RandomOverRange(firstSample.InteractionRadius, secondSample.InteractionRadius, rangeModifier);
        newGen.ConsumingSpeed = RandomOverRange(firstSample.ConsumingSpeed, secondSample.ConsumingSpeed, rangeModifier);
        newGen.PregnancyTime = RandomOverRange(firstSample.PregnancyTime, secondSample.PregnancyTime, rangeModifier);
        newGen.OffspringTime = RandomOverRange(firstSample.OffspringTime, secondSample.OffspringTime, rangeModifier);
        newGen.OffspringMaxPopulation = RandomOverRange(firstSample.OffspringMaxPopulation, secondSample.OffspringMaxPopulation, rangeModifier);
        newGen.Attractivness = RandomOverRange(firstSample.Attractivness, secondSample.Attractivness, rangeModifier);
        newGen.ReproductionChance = RandomOverRange(firstSample.ReproductionChance, secondSample.ReproductionChance, rangeModifier);
        return newGen;
    }

    public GenMerger GetAvarageGen()
    {
        genMerged = new GenMerger();
        foreach (Unit unit in SoulsManager.Instance.WispsHolder.GetComponentsInChildren<Unit>())
        {
            if (unit.IsAdult)
            {
                genMerged.SamplesCount++;
                Reflection.AddPropertiesValues(unit.Gens, genMerged);
            }
        }
        return genMerged;
    }


}
