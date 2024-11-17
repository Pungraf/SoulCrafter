using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SoulsManager : MonoBehaviour
{
    public static SoulsManager Instance { get; private set; }

    public bool UpdateSoulsNumbers;
    public int WispsCount;
    public int WolfsCount;

    [SerializeField] private GameObject window_Graph;
    [SerializeField] private float populationSamplingTime;

    [SerializeField] private Toggle togglePopulation;
    [SerializeField] private Toggle toggleLifeTime;
    [SerializeField] private Toggle toggleMaxHealth;
    [SerializeField] private Toggle toggleHungerResistance;
    [SerializeField] private Toggle toggleThirstResistance;
    [SerializeField] private Toggle toggleWalkSpeed;
    [SerializeField] private Toggle toggleSenseRadius;
    [SerializeField] private Toggle toggleConsumingSpeed;
    [SerializeField] private Toggle togglePregnancyTime;
    [SerializeField] private Toggle toggleOffspringTime;
    [SerializeField] private Toggle toggleOffspringPopulation;
    [SerializeField] private Toggle toggleAttractivness;

    public Transform WispsHolder;
    public Transform WolfsHolder;

    public GenSample DefaultWispGen;
    public GenSample DefaultWolfGen;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one SoulsManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;

        SetDefaultGens();
    }

    private void Update()
    {
        if(UpdateSoulsNumbers)
        {
            WispsCount = WispsHolder.childCount;
            WolfsCount = WolfsHolder.childCount;
            UpdateSoulsNumbers = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Unit.OnAnyUnitSpawn += Unit_OnAnyUnitSpawn;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
    }


    private void Unit_OnAnyUnitSpawn(object sender, EventArgs e)
    {
        
    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        
    }

    private void SetDefaultGens()
    {
        // Wisp
        DefaultWispGen.LifeSpan = new SingleGen(SingleGen.GenType.LifeSpan, 7);
        DefaultWispGen.Incubation = new SingleGen(SingleGen.GenType.Incubation, 1);

        DefaultWispGen.Vitality = new SingleGen(SingleGen.GenType.Vitality, 100);
        DefaultWispGen.Speed = new SingleGen(SingleGen.GenType.Speed, 3);
        DefaultWispGen.Strength = new SingleGen(SingleGen.GenType.Strength, 1);

        DefaultWispGen.Satiety = new SingleGen(SingleGen.GenType.Satiety, 2);
        DefaultWispGen.Hydration = new SingleGen(SingleGen.GenType.Hydration, 2);
        DefaultWispGen.Ingestion = new SingleGen(SingleGen.GenType.Ingestion, 0.02f);
        DefaultWispGen.Urge = new SingleGen(SingleGen.GenType.Urge, 2);

        DefaultWispGen.Reach = new SingleGen(SingleGen.GenType.Reach, 2);
        DefaultWispGen.Perception = new SingleGen(SingleGen.GenType.Perception, 50);
            
        DefaultWispGen.Fecundity = new SingleGen(SingleGen.GenType.Fecundity, 0.7f);
        DefaultWispGen.Attractiveness = new SingleGen(SingleGen.GenType.Attractiveness, 0.7f);
        DefaultWispGen.Gestation = new SingleGen(SingleGen.GenType.Gestation, 2);
        DefaultWispGen.Fertility = new SingleGen(SingleGen.GenType.Fertility, 5);
        // Wolf
        DefaultWolfGen.LifeSpan = new SingleGen(SingleGen.GenType.LifeSpan, 30);
        DefaultWolfGen.Incubation = new SingleGen(SingleGen.GenType.Incubation, 3);

        DefaultWolfGen.Vitality = new SingleGen(SingleGen.GenType.Vitality, 300);
        DefaultWolfGen.Speed = new SingleGen(SingleGen.GenType.Speed, 6);
        DefaultWispGen.Strength = new SingleGen(SingleGen.GenType.Strength, 3);

        DefaultWispGen.Satiety = new SingleGen(SingleGen.GenType.Satiety, 0.3f);
        DefaultWispGen.Hydration = new SingleGen(SingleGen.GenType.Hydration, 1);
        DefaultWispGen.Ingestion = new SingleGen(SingleGen.GenType.Ingestion, 0.01f);
        DefaultWispGen.Urge = new SingleGen(SingleGen.GenType.Urge, 1);

        DefaultWispGen.Reach = new SingleGen(SingleGen.GenType.Reach, 5);
        DefaultWispGen.Perception = new SingleGen(SingleGen.GenType.Perception, 80);

        DefaultWispGen.Fecundity = new SingleGen(SingleGen.GenType.Fecundity, 0.5f);
        DefaultWispGen.Attractiveness = new SingleGen(SingleGen.GenType.Attractiveness, 0.9f);
        DefaultWispGen.Gestation = new SingleGen(SingleGen.GenType.Gestation, 3);
        DefaultWispGen.Fertility = new SingleGen(SingleGen.GenType.Fertility, 3);
    }
}
