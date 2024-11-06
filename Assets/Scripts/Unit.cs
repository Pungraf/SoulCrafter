using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public abstract class Unit : MonoBehaviour
{
    [SerializeField] private float counterUpdateSampling = 1f;
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
    [SerializeField] private float remainingStageLifeTime = 0;
    [SerializeField] private float pregnancyCounter;

    //Parameters
    [SerializeField] private float health = 100;
    [SerializeField] private float hunger = 0;
    [SerializeField] private float thirst = 0;
    [SerializeField] private float urge = 0;
    [SerializeField] private float waste = 0;
    [SerializeField] private float anger = 0;
    [SerializeField] private float energy = 100;

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

        genScore = rand.Next(50, 100);

        TimeManager.OnHourChanged += HourChanged;
        InvokeRepeating("UpdateParameters", 0f, 1f);
    }


    //TODO: redo initialization types from bron to evlolved !
    public virtual void Initialize()
    {
        if (Gens == null)
        {
            Debug.LogError("Gen sample miisng !");
            return;
        }
        if (IsAdult)
        {
            controller.aIPath.maxSpeed = gens.Speed;
            RemainingStageLifeTime = gens.LifeSpan;
            if(Health == 0)
            {
                Health = Gens.Vitality;
                controller.ResetTraversableTag();
            }
            if(Hunger == 0)
            {
                Hunger = 50f;
            }
            if(Thirst == 0)
            {
                Thirst = 50f;
            }
        }
        else
        {
            controller.aIPath.maxSpeed = gens.Speed / 2;
            Health = gens.Vitality / 2;
            controller.ResetTraversableTag();
            RemainingStageLifeTime = gens.LifeSpan * 0.05f;
            if (Hunger == 0)
            {
                Hunger = 50f;
            }
            if (Thirst == 0)
            {
                Thirst = 50f;
            }
        }
    }

    public virtual void Initialize(GenSample gen, float health, int traversableMask, float hunger = 50f, float thirst = 50f)
    {
        if (gen != null)
        {
            gens = gen;
        }
        Health = health;
        Hunger = hunger;
        Thirst = thirst;
        controller.SetTraversableMask(traversableMask);
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
            health = Mathf.Clamp(value, 0f, Gens.Vitality); 
            if(health <= 0f)
            {
                controller.Death();
            }
        }

    }

    public float Hunger
    {
        get { return hunger; }
        set { hunger = Mathf.Clamp(value, 0f, 100f); }

    }

    public float Thirst
    {
        get { return thirst; }
        set { thirst = Mathf.Clamp(value, 0f, 100f); }

    }

    public float Urge
    {
        get { return urge; }
        set { urge = Mathf.Clamp(value, 0f, 100f); }

    }

    public float Waste
    {
        get { return waste; }
        set { waste = Mathf.Clamp(value, 0f, 100f); }

    }

    public float Anger
    {
        get { return anger; }
        set { anger = Mathf.Clamp(value, 0f, 100f); }
    }

    public float Energy
    {
        get { return energy; }
        set { energy = Mathf.Clamp(value, 0f, 100f); }
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
        //TODO: change all statee variable to 0-1 range;
        Waste += amount / 100;
    }

    public void Hydrate(float amount)
    {
        Thirst += amount;
    }

    public void Dispose()
    {
        Waste = 0;
    }
    public void TakeDamage(float amount)
    {
        Health -= amount;
        if(IsAgressive)
        {
            Anger += 10f;
        }
    }

    private void HourChanged(int hour)
    {
        if(hour == sleepHour)
        {
            Energy = 0f;
        }
        else if( hour == wakeupHour)
        {
            Energy = 100f;
        }
    }

    public void Die()
    {
        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
        controller.Death();
    }

    private void UpdateParameters()
    {
        Hunger -= gens.Satiety / counterUpdateSampling;
        Thirst -= gens.Hydration / counterUpdateSampling;
        RemainingStageLifeTime -= counterUpdateSampling;

        if (IsAdult)
        {
            if (isPregnant)
            {
                PregnancyCounter -= counterUpdateSampling;
            }
            else
            {
                Urge += gens.Urge / counterUpdateSampling;
            }
        }

        if (Hunger <= 0 || Thirst <= 0)
        {
            TakeDamage(10 * counterUpdateSampling);
        }
    }

    public void Unsubscribe()
    {
        TimeManager.OnHourChanged -= HourChanged;
    }
}
