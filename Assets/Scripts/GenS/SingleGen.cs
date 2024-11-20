using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct SingleGen
{
    [System.Serializable]
    public enum GenType
    {
        LifeSpan,
        Incubation,
        Vitality,
        Speed,
        Strength,
        Satiety,
        Hydration,
        Ingestion,
        Urge,
        Reach,
        Perception,
        Fecundity,
        Attractiveness,
        Gestation,
        Fertility
    }

    public SingleGen(GenType type, float value)
    {
        Type = type;
        Value = value;
    }

    public GenType Type { get; }
    public float Value { get; }
}
