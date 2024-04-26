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

    public new float LifeTime
    {
        get { return _lifeTime / _samplesCount; }
        set { _lifeTime += value; }
    }
    public new float MaxHealth
    {
        get { return _maxHealth / _samplesCount; }
        set { _maxHealth += value; }
    }
    public new float HungerTreshold
    {
        get { return _hungerTreshold / _samplesCount; }
        set { _hungerTreshold += value; }
    }
    public new float HungerResistance
    {
        get { return _hungerResistance / _samplesCount; }
        set { _hungerResistance += value; }
    }
    public new float ThirstTreshold
    {
        get { return _thirstTreshold / _samplesCount; }
        set { _thirstTreshold += value; }
    }
    public new float ThirstResistance
    {
        get { return _thirstResistance / _samplesCount; }
        set { _thirstResistance += value; }
    }
    public new float UrgeTreshold
    {
        get { return _urgeTreshold / _samplesCount; }
        set { _urgeTreshold += value; }
    }
    public new float WalkRadius
    {
        get { return _walkRadius / _samplesCount; }
        set { _walkRadius += value; }
    }
    public new float WalkSpeed
    {
        get { return _walkSpeed / _samplesCount; }
        set { _walkSpeed += value; }
    }
    public new float SenseRadius
    {
        get { return _senseRadius / _samplesCount; }
        set { _senseRadius += value; }
    }
    public new float InteractionRadius
    {
        get { return _interactionRadius / _samplesCount; }
        set { _interactionRadius += value; }
    }
    public new float ConsumingSpeed
    {
        get { return _consumingSpeed / _samplesCount; }
        set { _consumingSpeed += value; }
    }
    public new float PregnancyTime
    {
        get { return _pregnancyTime / _samplesCount; }
        set { _pregnancyTime += value; }
    }
    public new float OffspringTime
    {
        get { return _offspringTime / _samplesCount; }
        set { _offspringTime += value; }
    }
    public new int OffspringMaxPopulation
    {
        get { return _offspringMaxPopulation / _samplesCount; }
        set { _offspringMaxPopulation += value; }
    }
    public new float Attractivness
    {
        get { return _attractivness / _samplesCount; }
        set { _attractivness += value; }
    }
}
