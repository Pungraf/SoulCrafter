using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    public enum Species
    {
        Wisp,
        Wolf
    }

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

    [SerializeField] public bool IsFemale;
    [SerializeField] public Species species;
    [SerializeField] private bool isAdult = true;
    [SerializeField] private float health = 100;
    [SerializeField] private float remainingStageLifeTime = 0;
    [SerializeField] private float hunger = 0;
    [SerializeField] private float thirst = 0;
    [SerializeField] private float urge = 0;
    [SerializeField] private float waste = 0;
    [SerializeField] public List<Food.FoodType> edibleFood = new List<Food.FoodType>();
    [SerializeField] public List<Species> foodChainSpecies = new List<Species>();
    [SerializeField] public List<Species> predators = new List<Species>();

    [SerializeField] private bool isHungry;
    [SerializeField] private bool isThirsty;
    [SerializeField] private bool isWasteReady;
    [SerializeField] private bool isEagerToMate;
    [SerializeField] private bool isReadyToGrowUp;
    [SerializeField] private bool isReadyToBear;

    [SerializeField] private float pregnancyCounter;
    //Female gense
    [SerializeField] private bool isPregnant = false;


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

        if (IsFemale)
        {
            pregnancyCounter = gens.Gestation;
        }

        OnAnyUnitSpawn?.Invoke(this, EventArgs.Empty);

        genScore = rand.Next(50, 100);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateParameters();
    }

    public virtual void Initialize(GenSample gen = null, float health = 100, float hunger = 0, float thirst = 0)
    {

        if(gen != null)
        {
            gens = gen;
        }
        if(IsAdult)
        {
            controller.aIPath.maxSpeed = gens.Speed;
            this.health = health;
            this.hunger = hunger;
            this.thirst = thirst;
        }
        else
        {
            controller.aIPath.maxSpeed = gens.Speed / 2;
            health = gens.Vitality / 2;
            Hunger = 50f;
            Thirst = 50f;
        }

        if(isAdult)
        {
            remainingStageLifeTime = gens.LifeSpan;
        }
        else
        {
            remainingStageLifeTime = gens.LifeSpan * 0.05f;
        }
        isReadyToGrowUp = false;

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
        set { health = value; }

    }

    public float Hunger
    {
        get { return hunger; }
        set { hunger = value; }

    }

    public float Thirst
    {
        get { return thirst; }
        set { thirst = value; }

    }

    public float Urge
    {
        get { return urge; }
        set { urge = value; }

    }

    public float Waste
    {
        get { return waste; }
        set { waste = value; }

    }

    public bool IsHungry
    {
        get { return isHungry; }
        set { isHungry = value; }
    }

    public bool IsThirsty
    {
        get { return isThirsty; }
        set { isThirsty = value; }
    }

    public bool IsWasteReady
    {
        get { return isWasteReady; }
        set { isWasteReady = value; }
    }

    public bool IsEagerToMate
    {
        get { return isEagerToMate; }
        set { isEagerToMate = value; }
    }

    public bool IsReadyToBear
    {
        get { return isReadyToBear; }
        set { isReadyToBear = value; }
    }

    public bool IsAdult
    {
        get { return isAdult; }
        set { isAdult = value; }
    }

    public bool IsReadyToGrowUp
    {
        get { return isReadyToGrowUp; }
        set { isReadyToGrowUp = value; }
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
        hunger -= amount;
        if (hunger < 0)
        {
            hunger = 0;
        }
        //TODO: change all statee variable to 0-1 range;
        Waste += amount / 100;
    }

    public void Hydrate(float amount)
    {
        thirst -= amount;
        if (thirst < 0)
        {
            thirst = 0;
        }
    }

    public void Dispose()
    {
        Waste = 0;
        IsWasteReady = false;
    }
    public void TakeDamage(float amount)
    {
        health -= amount;
    }

    public void Die()
    {
        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
        controller.Death();
    }

    public void CheckStatuses()
    {
        if (RemainingStageLifeTime <= 0)
        {
            isReadyToGrowUp = true;
        }
        if(Waste >= 1f)
        {
            IsWasteReady = true;
        }
        if (!isAdult)
        {
            if (Hunger >= 70)
            {
                isHungry = true;
            }
            else
            {
                isHungry = false;
            }

            if (Thirst > 70)
            {
                IsThirsty = true;
            }
            else
            {
                IsThirsty = false;
            }
        }
        else
        {
            if (IsPregnant)
            {
                if (pregnancyCounter <= 0)
                {
                    IsReadyToBear = true;
                }
            }
            if (Urge >= 100)
            {
                isEagerToMate = true;
            }
            else
            {
                isEagerToMate = false;
            }

            if (Hunger > 70)
            {
                isHungry = true;
            }
            else
            {
                isHungry = false;
            }

            if (Thirst > 70)
            {
                IsThirsty = true;
            }
            else
            {
                IsThirsty = false;
            }
        }

        if (health <= 0)
        {
            Die();
        }
    }

    private void UpdateParameters()
    {
        Hunger += Time.deltaTime * gens.Satiety;
        Thirst += Time.deltaTime * gens.Hydration;


        RemainingStageLifeTime -= Time.deltaTime;


        if (hunger >= 100 || Thirst >= 100)
        {
            Health -= Time.deltaTime * 10;
        }

        if (IsAdult)
        {
            Urge += 10*Time.deltaTime;
            if(isPregnant)
            {
                pregnancyCounter -= Time.deltaTime;
            }
        }
    }
}
