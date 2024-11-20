using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GenSample
{
    //Time properties
    [SerializeField] protected SingleGen _lifeSpan;
    [SerializeField] protected SingleGen _incubation;

    //Main properties
    [SerializeField] protected SingleGen _vitality;
    [SerializeField] protected SingleGen _speed;
    [SerializeField] protected SingleGen _strength;

    //Needs properties
    [SerializeField] protected SingleGen _satiety;
    [SerializeField] protected SingleGen _hydration;
    [SerializeField] protected SingleGen _ingestion;
    [SerializeField] protected SingleGen _urge;

    //Interaction properties
    [SerializeField] protected SingleGen _reach;
    [SerializeField] protected SingleGen _perception;

    //Reproduction properties
    [SerializeField] protected SingleGen _fecundity;
    [SerializeField] protected SingleGen _attractiveness;
    [SerializeField] protected SingleGen _gestation;
    [SerializeField] protected SingleGen _fertility;

    public SingleGen LifeSpan
    {
        get { return _lifeSpan; }
        set { _lifeSpan = value; }
    }
    public SingleGen Incubation
    {
        get { return _incubation; }
        set { _incubation = value; }
    }
    public SingleGen Vitality
    {
        get { return _vitality; }
        set { _vitality = value; }
    }
    public SingleGen Speed
    {
        get { return _speed; }
        set { _speed = value; }
    }
    public SingleGen Strength
    {
        get { return _strength; }
        set { _strength = value; }
    }
    public SingleGen Satiety
    {
        get { return _satiety; }
        set { _satiety = value; }
    }
    public SingleGen Hydration
    {
        get { return _hydration; }
        set { _hydration = value; }
    }
    public SingleGen Ingestion
    {
        get { return _ingestion; }
        set { _ingestion = value; }
    }
    public SingleGen Urge
    {
        get { return _urge; }
        set { _urge = value; }
    }
    public SingleGen Reach
    {
        get { return _reach; }
        set { _reach = value; }
    }
    public SingleGen Perception
    {
        get { return _perception; }
        set { _perception = value; }
    }
    public SingleGen Fecundity
    {
        get { return _fecundity; }
        set { _fecundity = value; }
    }
    public SingleGen Attractiveness
    {
        get { return _attractiveness; }
        set { _attractiveness = value; }
    }
    public SingleGen Gestation
    {
        get { return _gestation; }
        set { _gestation = value; }
    }
    public SingleGen Fertility
    {
        get { return _fertility; }
        set { _fertility = value; }
    }
}
