using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GenSample
{
    [SerializeField] private string _species;

    [SerializeField] private bool _isFemale;
    [SerializeField] private float _lifeTime;
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _hungerTreshold;
    [SerializeField] private float _hungerResistance;
    [SerializeField] private float _thirstTreshold;
    [SerializeField] private float _thirstResistance;
    [SerializeField] private float _urgeTreshold;

    [SerializeField] private float _walkRadius;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _senseRadius;
    [SerializeField] private float _interactionRadius;

    [SerializeField] private float _consumingSpeed;

    [SerializeField] private float _pregnancyTime;
    [SerializeField] private float _offspringTime;
    [SerializeField] private int _offspringMaxPopulation;
    [SerializeField] private float _attractivness;

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
