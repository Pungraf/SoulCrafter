using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GenSample
{
    //Time properties
    [SerializeField] protected float _lifeSpan;
    [SerializeField] protected float _incubation;

    //Main properties
    [SerializeField] protected float _vitality;
    [SerializeField] protected float _speed;
    [SerializeField] protected float _strength;

    //Needs properties
    [SerializeField] protected float _satiety;
    [SerializeField] protected float _hydration;
    [SerializeField] protected float _ingestion;
    [SerializeField] protected float _urge;

    //Interaction properties
    [SerializeField] protected float _reach;
    [SerializeField] protected float _perception;

    //Reproduction properties
    [SerializeField] protected float _fecundity;
    [SerializeField] protected float _attractiveness;
    [SerializeField] protected float _gestation;
    [SerializeField] protected float _fertility;

    public float LifeSpan
    {
        get { return _lifeSpan; }
        set { _lifeSpan = value; }
    }
    public float Incubation
    {
        get { return _incubation; }
        set { _incubation = value; }
    }
    public float Vitality
    {
        get { return _vitality; }
        set { _vitality = value; }
    }
    public float Speed
    {
        get { return _speed; }
        set { _speed = value; }
    }
    public float Strength
    {
        get { return _strength; }
        set { _strength = value; }
    }
    public float Satiety
    {
        get { return _satiety; }
        set { _satiety = value; }
    }
    public float Hydration
    {
        get { return _hydration; }
        set { _hydration = value; }
    }
    public float Ingestion
    {
        get { return _ingestion; }
        set { _ingestion = value; }
    }
    public float Urge
    {
        get { return _urge; }
        set { _urge = value; }
    }
    public float Reach
    {
        get { return _reach; }
        set { _reach = value; }
    }
    public float Perception
    {
        get { return _perception; }
        set { _perception = value; }
    }
    public float Fecundity
    {
        get { return _fecundity; }
        set { _fecundity = value; }
    }
    public float Attractiveness
    {
        get { return _attractiveness; }
        set { _attractiveness = value; }
    }
    public float Gestation
    {
        get { return _gestation; }
        set { _gestation = value; }
    }
    public float Fertility
    {
        get { return _fertility; }
        set { _fertility = value; }
    }
}
