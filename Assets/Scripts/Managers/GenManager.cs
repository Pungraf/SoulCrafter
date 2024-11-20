using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenManager : MonoBehaviour
{
    public static GenManager Instance { get; private set; }

    private GenMerger genMerged;

    System.Random rand = new System.Random();

    public GenSample DefaultWispGen;
    public GenSample DefaultWolfGen;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one GenManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;

        SetDefaultGens();
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

        newGen.LifeSpan = new SingleGen
                        (SingleGen.GenType.LifeSpan,
                        RandomOverRange(firstSample.LifeSpan.Value, secondSample.LifeSpan.Value, rangeModifier));
        newGen.Incubation = new SingleGen
                        (SingleGen.GenType.Incubation,
                        RandomOverRange(firstSample.Incubation.Value, secondSample.Incubation.Value, rangeModifier));
        newGen.Vitality = new SingleGen
                        (SingleGen.GenType.Vitality,
                        RandomOverRange(firstSample.Vitality.Value, secondSample.Vitality.Value, rangeModifier));
        newGen.Speed = new SingleGen
                        (SingleGen.GenType.Speed,
                        RandomOverRange(firstSample.Speed.Value, secondSample.Speed.Value, rangeModifier));
        newGen.Strength = new SingleGen
                        (SingleGen.GenType.Strength,
                        RandomOverRange(firstSample.Strength.Value, secondSample.Strength.Value, rangeModifier));
        newGen.Satiety = new SingleGen
                        (SingleGen.GenType.Satiety,
                        RandomOverRange(firstSample.Satiety.Value, secondSample.Satiety.Value, rangeModifier));
        newGen.Hydration = new SingleGen
                        (SingleGen.GenType.Hydration,
                        RandomOverRange(firstSample.Hydration.Value, secondSample.Hydration.Value, rangeModifier));
        newGen.Ingestion = new SingleGen
                        (SingleGen.GenType.Ingestion,
                        RandomOverRange(firstSample.Ingestion.Value, secondSample.Ingestion.Value, rangeModifier));
        newGen.Urge = new SingleGen
                        (SingleGen.GenType.Urge,
                        RandomOverRange(firstSample.Urge.Value, secondSample.Urge.Value, rangeModifier));
        newGen.Reach = new SingleGen
                        (SingleGen.GenType.Reach,
                        RandomOverRange(firstSample.Reach.Value, secondSample.Reach.Value, rangeModifier));
        newGen.Perception = new SingleGen
                        (SingleGen.GenType.Perception,
                        RandomOverRange(firstSample.Perception.Value, secondSample.Perception.Value, rangeModifier));
        newGen.Fecundity = new SingleGen
                        (SingleGen.GenType.Fecundity,
                        RandomOverRange(firstSample.Fecundity.Value, secondSample.Fecundity.Value, rangeModifier));
        newGen.Attractiveness = new SingleGen
                        (SingleGen.GenType.Attractiveness,
                        RandomOverRange(firstSample.Attractiveness.Value, secondSample.Attractiveness.Value, rangeModifier));
        newGen.Gestation = new SingleGen
                        (SingleGen.GenType.Gestation,
                        RandomOverRange(firstSample.Gestation.Value, secondSample.Gestation.Value, rangeModifier));
        newGen.Fertility = new SingleGen
                        (SingleGen.GenType.Fertility,
                        RandomOverRange(firstSample.Fertility.Value, secondSample.Fertility.Value, rangeModifier));

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



    private void SetDefaultGens()
    {
        // Wisp
        DefaultWispGen.LifeSpan = new SingleGen(SingleGen.GenType.LifeSpan, 7);
        DefaultWispGen.Incubation = new SingleGen(SingleGen.GenType.Incubation, 1);

        DefaultWispGen.Vitality = new SingleGen(SingleGen.GenType.Vitality, 100);
        DefaultWispGen.Speed = new SingleGen(SingleGen.GenType.Speed, 3);
        DefaultWispGen.Strength = new SingleGen(SingleGen.GenType.Strength, 1);

        DefaultWispGen.Satiety = new SingleGen(SingleGen.GenType.Satiety, 2);
        DefaultWispGen.Hydration = new SingleGen(SingleGen.GenType.Hydration, 2);
        DefaultWispGen.Ingestion = new SingleGen(SingleGen.GenType.Ingestion, 0.02f);
        DefaultWispGen.Urge = new SingleGen(SingleGen.GenType.Urge, 2);

        DefaultWispGen.Reach = new SingleGen(SingleGen.GenType.Reach, 2);
        DefaultWispGen.Perception = new SingleGen(SingleGen.GenType.Perception, 50);

        DefaultWispGen.Fecundity = new SingleGen(SingleGen.GenType.Fecundity, 0.7f);
        DefaultWispGen.Attractiveness = new SingleGen(SingleGen.GenType.Attractiveness, 0.7f);
        DefaultWispGen.Gestation = new SingleGen(SingleGen.GenType.Gestation, 2);
        DefaultWispGen.Fertility = new SingleGen(SingleGen.GenType.Fertility, 5);
        // Wolf
        DefaultWolfGen.LifeSpan = new SingleGen(SingleGen.GenType.LifeSpan, 30);
        DefaultWolfGen.Incubation = new SingleGen(SingleGen.GenType.Incubation, 3);

        DefaultWolfGen.Vitality = new SingleGen(SingleGen.GenType.Vitality, 300);
        DefaultWolfGen.Speed = new SingleGen(SingleGen.GenType.Speed, 6);
        DefaultWolfGen.Strength = new SingleGen(SingleGen.GenType.Strength, 3);

        DefaultWolfGen.Satiety = new SingleGen(SingleGen.GenType.Satiety, 0.3f);
        DefaultWolfGen.Hydration = new SingleGen(SingleGen.GenType.Hydration, 1);
        DefaultWolfGen.Ingestion = new SingleGen(SingleGen.GenType.Ingestion, 0.01f);
        DefaultWolfGen.Urge = new SingleGen(SingleGen.GenType.Urge, 1);

        DefaultWolfGen.Reach = new SingleGen(SingleGen.GenType.Reach, 5);
        DefaultWolfGen.Perception = new SingleGen(SingleGen.GenType.Perception, 80);

        DefaultWolfGen.Fecundity = new SingleGen(SingleGen.GenType.Fecundity, 0.5f);
        DefaultWolfGen.Attractiveness = new SingleGen(SingleGen.GenType.Attractiveness, 0.9f);
        DefaultWolfGen.Gestation = new SingleGen(SingleGen.GenType.Gestation, 3);
        DefaultWolfGen.Fertility = new SingleGen(SingleGen.GenType.Fertility, 3);
    }

    public int CalculateGenScore(Unit.Species species, GenSample unitSample)
    {
        // Map of all properties in GenSample
        Dictionary<SingleGen.GenType, Func<GenSample, SingleGen>> genPropertyMap = new Dictionary<SingleGen.GenType, Func<GenSample, SingleGen>>()
        {
            { SingleGen.GenType.LifeSpan, sample => sample.LifeSpan },
            { SingleGen.GenType.Incubation, sample => sample.Incubation },
            { SingleGen.GenType.Vitality, sample => sample.Vitality },
            { SingleGen.GenType.Speed, sample => sample.Speed },
            { SingleGen.GenType.Strength, sample => sample.Strength },
            { SingleGen.GenType.Satiety, sample => sample.Satiety },
            { SingleGen.GenType.Hydration, sample => sample.Hydration },
            { SingleGen.GenType.Ingestion, sample => sample.Ingestion },
            { SingleGen.GenType.Urge, sample => sample.Urge },
            { SingleGen.GenType.Reach, sample => sample.Reach },
            { SingleGen.GenType.Perception, sample => sample.Perception },
            { SingleGen.GenType.Fecundity, sample => sample.Fecundity },
            { SingleGen.GenType.Attractiveness, sample => sample.Attractiveness },
            { SingleGen.GenType.Gestation, sample => sample.Gestation },
            { SingleGen.GenType.Fertility, sample => sample.Fertility }
        };

        float totalScore = 0f;
        int propertyCount = genPropertyMap.Count;

        GenSample defaultSample = null;

        switch (species)
        {
            case Unit.Species.Wisp:
                defaultSample = DefaultWispGen;
                break;
            case Unit.Species.Wolf:
                defaultSample = DefaultWolfGen;
                break;
            default:
                Debug.Log("Wrong species.");
                break;
        }

        if(defaultSample == null)
        {
            Debug.Log("Missins defaut sample.");
            return 0;
        }

        foreach (var kvp in genPropertyMap)
        {
            SingleGen defaultGen = kvp.Value(defaultSample);
            SingleGen unitGen = kvp.Value(unitSample);

            // Avoid division by zero or invalid data
            if (defaultGen.Value <= 0)
            {
                Debug.LogWarning($"Default value for {kvp.Key} is zero or negative; skipping this property.");
                propertyCount--;
                continue;
            }

            // Calculate the score for this property
            totalScore += unitGen.Value / defaultGen.Value;
        }

        // Return the average score
        return (int)(propertyCount > 0 ? totalScore / propertyCount * 100f : 0f);
    }
}
