using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GenSample
{
    public string species = "Soul";

    public bool isFemale = false;
    public float lifeTime = 600;
    public float maxHealth = 100;
    public float hungerTreshold = 40;
    public float hungerResistance = 0.5f;
    public float thirstTreshold = 40;
    public float thirstResistance = 0.5f;
    public float urgeTreshold = 50;

    public float walkRadius = 20f;
    public float senseRadius = 10f;
    public float interactionRadius = 2f;

    public float consumingSpeed = 5;

    public float pregnancyTime = 100;
    public float offspringTime = 100;
    public int offspringMaxPopulation = 5;
    public float attractiveness = 0.5f;
}
