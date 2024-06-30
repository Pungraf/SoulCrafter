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

        newGen.LifeSpan = RandomOverRange(firstSample.LifeSpan, secondSample.LifeSpan, rangeModifier);
        newGen.Incubation = RandomOverRange(firstSample.Incubation, secondSample.Incubation, rangeModifier);
        newGen.Vitality = RandomOverRange(firstSample.Vitality, secondSample.Vitality, rangeModifier);
        newGen.Speed = RandomOverRange(firstSample.Speed, secondSample.Speed, rangeModifier);
        newGen.Strength = RandomOverRange(firstSample.Strength, secondSample.Strength, rangeModifier);
        newGen.Satiety = RandomOverRange(firstSample.Satiety, secondSample.Satiety, rangeModifier);
        newGen.Hydration = RandomOverRange(firstSample.Hydration, secondSample.Hydration, rangeModifier);
        newGen.Ingestion = RandomOverRange(firstSample.Ingestion, secondSample.Ingestion, rangeModifier);
        newGen.Urge = RandomOverRange(firstSample.Urge, secondSample.Urge, rangeModifier);
        newGen.Reach = RandomOverRange(firstSample.Reach, secondSample.Reach, rangeModifier);
        newGen.Perception = RandomOverRange(firstSample.Perception, secondSample.Perception, rangeModifier);
        newGen.Fecundity = RandomOverRange(firstSample.Fecundity, secondSample.Fecundity, rangeModifier);
        newGen.Attractiveness = RandomOverRange(firstSample.Attractiveness, secondSample.Attractiveness, rangeModifier);
        newGen.Gestation = RandomOverRange(firstSample.Gestation, secondSample.Gestation, rangeModifier);
        newGen.Fertility = RandomOverRange(firstSample.Fertility, secondSample.Fertility, rangeModifier);
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
