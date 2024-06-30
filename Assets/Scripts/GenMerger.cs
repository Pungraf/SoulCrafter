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

    public new float LifeSpan
    {
        get { return _lifeSpan / _samplesCount; }
        set { _lifeSpan += value; }
    }
    public new float Incubation
    {
        get { return _incubation / _samplesCount; }
        set { _incubation += value; }
    }
    public new float Vitality
    {
        get { return _vitality / _samplesCount; }
        set { _vitality += value; }
    }
    public new float Speed
    {
        get { return _speed / _samplesCount; }
        set { _speed += value; }
    }
    public new float Strength
    {
        get { return _strength / _samplesCount; }
        set { _strength += value; }
    }
    public new float Satiety
    {
        get { return _satiety / _samplesCount; }
        set { _satiety += value; }
    }
    public new float Hydration
    {
        get { return _hydration / _samplesCount; }
        set { _hydration += value; }
    }
    public new float Ingestion
    {
        get { return _ingestion / _samplesCount; }
        set { _ingestion += value; }
    }
    public new float Urge
    {
        get { return _urge / _samplesCount; }
        set { _urge += value; }
    }
    public new float Reach
    {
        get { return _reach / _samplesCount; }
        set { _reach += value; }
    }
    public new float Perception
    {
        get { return _perception / _samplesCount; }
        set { _perception += value; }
    }
    public new float Fecundity
    {
        get { return _fecundity / _samplesCount; }
        set { _fecundity += value; }
    }
    public new float Attractiveness
    {
        get { return _attractiveness / _samplesCount; }
        set { _attractiveness += value; }
    }
    public new float Gestation
    {
        get { return _gestation / _samplesCount; }
        set { _gestation += value; }
    }
    public new float Fertility
    {
        get { return _fertility / _samplesCount; }
        set { _fertility += value; }
    }
}
