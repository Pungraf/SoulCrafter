using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenManager : MonoBehaviour
{
    public static GenManager Instance { get; private set; }

    private GenMerger genMerged;

    System.Random rand = new System.Random();

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


}
