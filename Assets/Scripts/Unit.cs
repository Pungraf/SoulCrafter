using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public abstract class Unit : MonoBehaviour
{
    private static float sekPerDay = 1440f;
    private static float counterUpdateSampling = 1f;
    [SerializeField] private int sleepHour = 22;
    [SerializeField] private int wakeupHour = 6;
    public enum Species
    {
        Wisp,
        Wolf
    }

    public GameObject eggPrefab;
    public GameObject femaleOffspringPrefab;
    public GameObject maleOffspringPrefab;
    public GameObject evolvedUnitPrefab;
    public GameObject corpseUnitPrefab;
    public GameObject wastePrefab;

    public int genScore;

    public Transform targetedTransform;
    public UnitController controller;

    protected System.Random rand = new System.Random();

    [SerializeField] private GenSample gens;
    [SerializeField] private GenSample lastPartnerGenSample;

    //States
    [SerializeField] public bool IsFemale;
    [SerializeField] public Species species;
    [SerializeField] private bool isAdult = true;
    [SerializeField] private bool isAgressive = false;
    [SerializeField] private bool isPregnant = false;

    //Timers
    [SerializeField] private float remainingStageLifeTime = 0f;
    [SerializeField] private float pregnancyCounter;

    //Parameters
    [SerializeField] private float health = 100;
    [SerializeField] private float hunger = 0;
    [SerializeField] private float thirst = 0;
    [SerializeField] private float urge = 0;
    [SerializeField] private float waste = 0;
    [SerializeField] private float anger = 0;
    [SerializeField] private float energy = 1f;

    [SerializeField] public List<Food.FoodType> edibleFood = new List<Food.FoodType>();
    [SerializeField] public List<Species> foodChainSpecies = new List<Species>();
    [SerializeField] public List<Species> predators = new List<Species>();


    public static event EventHandler OnAnyUnitSpawn;
    public static event EventHandler OnAnyUnitDead;

    private void Awake()
    {
        controller = GetComponent<UnitController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Initialize starting parameters
        Initialize();

        OnAnyUnitSpawn?.Invoke(this, EventArgs.Empty);

        // TODO: calculate based on gens
        genScore = rand.Next(50, 100);

        TimeManager.OnHourChanged += HourChanged;
        InvokeRepeating("UpdateParameters", 0f, counterUpdateSampling);
    }


    //TODO: redo initialization types from bron to evlolved !
    public virtual void Initialize()
    {
        if (Gens == null)
        {
            Debug.LogError("Gen sample miisng !");
            return;
        }
        //TODO: Change to inspector serializable verison with Odin
        if(Gens.LifeSpan.Value == 0)
        {
            if(species == Species.Wisp)
            {
                Gens.LifeSpan = new SingleGen(SingleGen.GenType.LifeSpan, 7);
                Gens.Incubation = new SingleGen(SingleGen.GenType.Incubation, 1);

                Gens.Vitality = new SingleGen(SingleGen.GenType.Vitality, 100);
                Gens.Speed = new SingleGen(SingleGen.GenType.Speed, 3);
                Gens.Strength = new SingleGen(SingleGen.GenType.Strength, 1);

                Gens.Satiety = new SingleGen(SingleGen.GenType.Satiety, 2);
                Gens.Hydration = new SingleGen(SingleGen.GenType.Hydration, 2);
                Gens.Ingestion = new SingleGen(SingleGen.GenType.Ingestion, 0.02f);
                Gens.Urge = new SingleGen(SingleGen.GenType.Urge, 2);

                Gens.Reach = new SingleGen(SingleGen.GenType.Reach, 2);
                Gens.Perception = new SingleGen(SingleGen.GenType.Perception, 50);

                Gens.Fecundity = new SingleGen(SingleGen.GenType.Fecundity, 0.7f);
                Gens.Attractiveness = new SingleGen(SingleGen.GenType.Attractiveness, 0.7f);
                Gens.Gestation = new SingleGen(SingleGen.GenType.Gestation, 2);
                Gens.Fertility = new SingleGen(SingleGen.GenType.Fertility, 5);

            }
            else if(species == Species.Wolf)
            {
                Gens.LifeSpan = new SingleGen(SingleGen.GenType.LifeSpan, 30);
                Gens.Incubation = new SingleGen(SingleGen.GenType.Incubation, 3);

                Gens.Vitality = new SingleGen(SingleGen.GenType.Vitality, 300);
                Gens.Speed = new SingleGen(SingleGen.GenType.Speed, 6);
                Gens.Strength = new SingleGen(SingleGen.GenType.Strength, 3);

                Gens.Satiety = new SingleGen(SingleGen.GenType.Satiety, 0.3f);
                Gens.Hydration = new SingleGen(SingleGen.GenType.Hydration, 1);
                Gens.Ingestion = new SingleGen(SingleGen.GenType.Ingestion, 0.01f);
                Gens.Urge = new SingleGen(SingleGen.GenType.Urge, 1);

                Gens.Reach = new SingleGen(SingleGen.GenType.Reach, 5);
                Gens.Perception = new SingleGen(SingleGen.GenType.Perception, 80);

                Gens.Fecundity = new SingleGen(SingleGen.GenType.Fecundity, 0.5f);
                Gens.Attractiveness = new SingleGen(SingleGen.GenType.Attractiveness, 0.9f);
                Gens.Gestation = new SingleGen(SingleGen.GenType.Gestation, 3);
                Gens.Fertility = new SingleGen(SingleGen.GenType.Fertility, 3);
            }
        }
        if (IsAdult)
        {
            controller.aIPath.maxSpeed = gens.Speed.Value;
            RemainingStageLifeTime = gens.LifeSpan.Value;
            if(Health == 0)
            {
                Health = Gens.Vitality.Value;
                controller.ResetTraversableTag();
            }
            if(Hunger == 0)
            {
                Hunger = 0.5f;
            }
            if(Thirst == 0)
            {
                Thirst = 0.5f;
            }
        }
        else
        {
            controller.aIPath.maxSpeed = gens.Speed.Value / 2;
            Health = gens.Vitality.Value / 2;
            controller.ResetTraversableTag();
            RemainingStageLifeTime = gens.LifeSpan.Value * 0.05f;
            if (Hunger == 0)
            {
                Hunger = 0.5f;
            }
            if (Thirst == 0)
            {
                Thirst = 0.5f;
            }
        }
    }

    public virtual void Initialize(GenSample gen, float health, int traversableMask, float hunger = 0.5f, float thirst = 0.5f)
    {
        if (gen != null)
        {
            gens = gen;
        }
        Health = health;
        Hunger = hunger;
        Thirst = thirst;
        controller.SetTraversableMask(traversableMask);

        Debug.Log(Gens.Vitality.Value);
    }

    // Accesors
    public GenSample Gens
    {
        get { return gens; }
        set { gens = value; }
    }
    public GenSample LastPartnerGenSample
    {
        get { return lastPartnerGenSample; }
        set { lastPartnerGenSample = value; }
    }
    public float Health
    {
        get { return health; }
        set
        {
            health = Mathf.Clamp(value, 0f, Gens.Vitality.Value); 
            if(health <= 0f)
            {
                controller.Death();
            }
        }

    }

    public float Hunger
    {
        get { return hunger; }
        set { hunger = Mathf.Clamp(value, 0f, 1f); }

    }

    public float Thirst
    {
        get { return thirst; }
        set { thirst = Mathf.Clamp(value, 0f, 1f); }

    }

    public float Urge
    {
        get { return urge; }
        set { urge = Mathf.Clamp(value, 0f, 1f); }

    }

    public float Waste
    {
        get { return waste; }
        set { waste = Mathf.Clamp(value, 0f, 1f); }

    }

    public float Anger
    {
        get { return anger; }
        set { anger = Mathf.Clamp(value, 0f, 1f); }
    }

    public float Energy
    {
        get { return energy; }
        set { energy = Mathf.Clamp(value, 0f, 1f); }
    }

    public bool IsAdult
    {
        get { return isAdult; }
        set { isAdult = value; }
    }

    public bool IsAgressive
    {
        get { return isAgressive; }
        set { isAgressive = value; }
    }

    public bool IsPregnant
    {
        get { return isPregnant;}
        set { isPregnant = value;}
    }

    public float RemainingStageLifeTime
    {
        get { return remainingStageLifeTime; }
        set { remainingStageLifeTime = value; }
    }

    public float PregnancyCounter
    {
        get { return pregnancyCounter; }
        set { pregnancyCounter = value; }
    }

    public void Feed(float amount)
    {
        Hunger += amount;
        Waste += amount;
    }

    public void Hydrate(float amount)
    {
        Thirst += amount;
    }

    public void Dispose()
    {
        Waste = 0;
    }
    public bool TakeDamage(float amount)
    {
        Health -= amount;
        if(IsAgressive)
        {
            Anger += 0.1f;
        }
        //Notife if was killed by damage
        if(Health <= 0)
        {
            return true;
        }
        return false;
    }

    private void HourChanged(int hour)
    {
        if(hour == sleepHour)
        {
            Energy = 0f;
        }
        else if( hour == wakeupHour)
        {
            Energy = 1f;
        }
    }

    public void Die()
    {
        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
        controller.Death();
    }

    private void UpdateParameters()
    {
        Hunger -= gens.Satiety.Value * counterUpdateSampling / sekPerDay;
        Thirst -= gens.Hydration.Value * counterUpdateSampling / sekPerDay;
        RemainingStageLifeTime -= counterUpdateSampling / sekPerDay;

        if (IsAdult)
        {
            if (isPregnant)
            {
                PregnancyCounter -= counterUpdateSampling / sekPerDay;
            }
            else
            {
                Urge += gens.Urge.Value * counterUpdateSampling / sekPerDay;
            }
        }

        if (Hunger <= 0 || Thirst <= 0)
        {
            TakeDamage(1f * counterUpdateSampling);
        }

        if(IsAgressive && (Hunger <= 0.5f || Thirst <= 0.5f))
        {
            Anger += 0.01f;
        }
        else
        {
            Anger -= 0.01f;
        }
    }

    public void Unsubscribe()
    {
        TimeManager.OnHourChanged -= HourChanged;
    }
}
