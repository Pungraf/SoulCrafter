using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GenSample
{
    [SerializeField] protected string _species;

    [SerializeField] protected bool _isFemale;
    [SerializeField] protected float _lifeTime;
    [SerializeField] protected float _maxHealth;
    [SerializeField] protected float _hungerTreshold;
    [SerializeField] protected float _hungerResistance;
    [SerializeField] protected float _thirstTreshold;
    [SerializeField] protected float _thirstResistance;
    [SerializeField] protected float _urgeTreshold;

    [SerializeField] protected float _walkRadius;
    [SerializeField] protected float _walkSpeed;
    [SerializeField] protected float _senseRadius;
    [SerializeField] protected float _interactionRadius;

    [SerializeField] protected float _consumingSpeed;

    [SerializeField] protected float _pregnancyTime;
    [SerializeField] protected float _offspringTime;
    [SerializeField] protected int _offspringMaxPopulation;
    [SerializeField] protected float _attractivness;

    public string Species
    {
        get { return _species; }
        set { _species = value; }
    }
    public bool IsFemale
    {
        get { return _isFemale;}
        set { _isFemale = value;}
    }
    public float LifeTime
    {
        get { return _lifeTime; }
        set { _lifeTime = value; }
    }
    public float MaxHealth
    {
        get { return _maxHealth; }
        set { _maxHealth = value; }
    }
    public float HungerTreshold
    {
        get { return _hungerTreshold; }
        set { _hungerTreshold = value; }
    }
    public float HungerResistance
    {
        get { return _hungerResistance; }
        set { _hungerResistance = value;}
    }
    public float ThirstTreshold
    {
        get { return _thirstTreshold; }
        set { _thirstTreshold = value;}
    }
    public float ThirstResistance
    {
        get { return _thirstResistance; }
        set { _thirstResistance = value;}
    }
    public float UrgeTreshold
    {
        get { return _urgeTreshold; }
        set { _urgeTreshold = value; }
    }
    public float WalkRadius
    {
        get { return _walkRadius; }
        set { _walkRadius = value; }
    }
    public float WalkSpeed
    { 
        get { return _walkSpeed; }
        set { _walkSpeed = value; }
    }
    public float SenseRadius
    {
        get { return _senseRadius; }
        set { _senseRadius = value; }   
    }
    public float InteractionRadius
    {
        get { return _interactionRadius; }
        set { _interactionRadius = value; }
    }
    public float ConsumingSpeed
    {
        get { return _consumingSpeed; }
        set { _consumingSpeed = value; }
    }
    public float PregnancyTime
    {
        get { return _pregnancyTime; }
        set { _pregnancyTime = value;}
    }
    public float OffspringTime
    {
        get { return _offspringTime; }
        set { _offspringTime = value; }
    }
    public int OffspringMaxPopulation
    {
        get { return _offspringMaxPopulation; }
        set { _offspringMaxPopulation = value; }
    }
    public float Attractivness
    {
        get { return _attractivness; }
        set { _attractivness = value; }
    }
}
