using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenMerger: GenSample
{
    private int _samplesCount = 0;
    public int SamplesCount
    {
        get { return _samplesCount; }
        set { _samplesCount = value; }
    }

    public new SingleGen LifeSpan
    {
        get { return new SingleGen(_lifeSpan.Type, _lifeSpan.Value / SamplesCount); }
        set { _lifeSpan = new SingleGen(_lifeSpan.Type, _lifeSpan.Value + value.Value); }
    }
    public new SingleGen Incubation
    {
        get { return new SingleGen(_incubation.Type, _incubation.Value / SamplesCount); }
        set { _incubation = new SingleGen(_incubation.Type, _incubation.Value + value.Value); }
    }
    public new SingleGen Vitality
    {
        get { return new SingleGen(_vitality.Type, _vitality.Value / SamplesCount); }
        set { _vitality = new SingleGen(_vitality.Type, _vitality.Value + value.Value); }
    }
    public new SingleGen Speed
    {
        get { return new SingleGen(_speed.Type, _speed.Value / SamplesCount); }
        set { _speed = new SingleGen(_speed.Type, _speed.Value + value.Value); }
    }
    public new SingleGen Strength
    {
        get { return new SingleGen(_strength.Type, _strength.Value / SamplesCount); }
        set { _strength = new SingleGen(_strength.Type, _strength.Value + value.Value); }
    }
    public new SingleGen Satiety
    {
        get { return new SingleGen(_satiety.Type, _satiety.Value / SamplesCount); }
        set { _satiety = new SingleGen(_satiety.Type, _satiety.Value + value.Value); }
    }
    public new SingleGen Hydration
    {
        get { return new SingleGen(_hydration.Type, _hydration.Value / SamplesCount); }
        set { _hydration = new SingleGen(_hydration.Type, _hydration.Value + value.Value); }
    }
    public new SingleGen Ingestion
    {
        get { return new SingleGen(_ingestion.Type, _ingestion.Value / SamplesCount); }
        set { _ingestion = new SingleGen(_ingestion.Type, _ingestion.Value + value.Value); }
    }
    public new SingleGen Urge
    {
        get { return new SingleGen(_urge.Type, _urge.Value / SamplesCount); }
        set { _urge = new SingleGen(_urge.Type, _urge.Value + value.Value); }
    }
    public new SingleGen Reach
    {
        get { return new SingleGen(_reach.Type, _reach.Value / SamplesCount); }
        set { _reach = new SingleGen(_reach.Type, _reach.Value + value.Value); }
    }
    public new SingleGen Perception
    {
        get { return new SingleGen(_perception.Type, _perception.Value / SamplesCount); }
        set { _perception = new SingleGen(_perception.Type, _perception.Value + value.Value); }
    }
    public new SingleGen Fecundity
    {
        get { return new SingleGen(_fecundity.Type, _fecundity.Value / SamplesCount); }
        set { _fecundity = new SingleGen(_fecundity.Type, _fecundity.Value + value.Value); }
    }
    public new SingleGen Attractiveness
    {
        get { return new SingleGen(_attractiveness.Type, _attractiveness.Value / SamplesCount); }
        set { _attractiveness = new SingleGen(_attractiveness.Type, _attractiveness.Value + value.Value); }
    }
    public new SingleGen Gestation
    {
        get { return new SingleGen(_gestation.Type, _gestation.Value / SamplesCount); }
        set { _gestation = new SingleGen(_gestation.Type, _gestation.Value + value.Value); }
    }
    public new SingleGen Fertility
    {
        get { return new SingleGen(_fertility.Type, _fertility.Value / SamplesCount); }
        set { _fertility = new SingleGen(_fertility.Type, _fertility.Value + value.Value); }
    }
}
