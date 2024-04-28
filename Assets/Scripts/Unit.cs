using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    public GameObject femaleOffspringPrefab;
    public GameObject maleOffspringPrefab;
    public GameObject evolvedUnitPrefab;
    public GameObject corpseUnitPrefab;

    public Transform targetedTransform;

    private UnitController controller;

    [SerializeField] private GenSample gens;
    [SerializeField] private GenSample lastPartnerGenSample;

    [SerializeField] private bool isAdult = true;
    [SerializeField] private float health = 100;
    [SerializeField] private float remainingStageLifeTime = 0;
    [SerializeField] private float hunger = 0;
    [SerializeField] private float thirst = 0;
    [SerializeField] private float urge = 0;

    [SerializeField] private bool isHungry;
    [SerializeField] private bool isThirsty;
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
        transform.SetParent(SoulsManager.Instance.WispsHolder);

        if (gens.IsFemale)
        {
            pregnancyCounter = gens.PregnancyTime;
        }

        OnAnyUnitSpawn?.Invoke(this, EventArgs.Empty);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateParameters();
        CheckStatuses();
    }

    public void Initialize(GenSample gen = null, float health = 100, float hunger = 0, float thirst = 0)
    {
        if(gen != null)
        {
            gens = gen;
        }
        if(IsAdult)
        {
            controller.navMeshAgent.speed = gens.WalkSpeed;
            this.health = health;
            this.hunger = hunger;
            this.thirst = thirst;
        }
        else
        {
            controller.navMeshAgent.speed = gens.WalkSpeed / 2;
            health = gens.MaxHealth / 2;
            Hunger = 50f;
            Thirst = 50f;
        }

        if(isAdult)
        {
            remainingStageLifeTime = gens.LifeTime;
        }
        else
        {
            remainingStageLifeTime = gens.OffspringTime;
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
    }

    public void Hydrate(float amount)
    {
        thirst -= amount;
        if (thirst < 0)
        {
            thirst = 0;
        }
    }

    public void Die()
    {
        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
        controller.Death();
    }

    private void CheckStatuses()
    {
        RemainingStageLifeTime -= Time.deltaTime;
        if (RemainingStageLifeTime <= 0)
        {
            isReadyToGrowUp = true;
        }
        if (!isAdult)
        {
            if (Hunger > gens.HungerTreshold / 2)
            {
                isHungry = true;
            }
            else
            {
                isHungry = false;
            }

            if (Thirst > gens.ThirstTreshold / 2)
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
            Urge += Time.deltaTime;
            if (IsPregnant)
            {
                pregnancyCounter -= Time.deltaTime;
                if (pregnancyCounter <= 0)
                {
                    IsReadyToBear = true;
                }
            }
            if (Urge > gens.UrgeTreshold)
            {
                isEagerToMate = true;
            }
            else
            {
                isEagerToMate = false;
            }

            if (Hunger > gens.HungerTreshold)
            {
                isHungry = true;
            }
            else
            {
                isHungry = false;
            }

            if (Thirst > gens.ThirstTreshold)
            {
                IsThirsty = true;
            }
            else
            {
                IsThirsty = false;
            }
        }
    }

    private void UpdateParameters()
    {
        Hunger += Time.deltaTime * gens.HungerResistance;
        Thirst += Time.deltaTime * gens.ThirstResistance;


        if (hunger >= 100 || Thirst >= 100)
        {
            Health -= Time.deltaTime * 10;
        }
        if (health <= 0)
        {
            Die();
        }
    }
}
