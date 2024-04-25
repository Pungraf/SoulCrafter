using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public GameObject offspringPrefab;
    public GameObject malePrefab;
    public GameObject femalePrefab;

    public Transform targetedTransform;

    [SerializeField] private GenSample gens;

    [SerializeField] private bool isAdult = true;
    [SerializeField] private float health = 100;
    [SerializeField] private float remainingLifeTime = 0;
    [SerializeField] private float hunger = 0;
    [SerializeField] private float thirst = 0;
    [SerializeField] private float urge = 0;

    [SerializeField] private bool isHungry;
    [SerializeField] private bool isThirsty;
    [SerializeField] private bool isEagerToMate;
    [SerializeField] private bool isReadyToGrowUp;
    [SerializeField] private bool isReadyToBear;

    [SerializeField] private float pregnancyCounter;
    [SerializeField] private float offspringCounter;
    //Female gense
    [SerializeField] private bool isPregnant = false;


    public static event EventHandler OnAnyUnitSpawn;
    public static event EventHandler OnAnyUnitDead;


    // Start is called before the first frame update
    void Start()
    {
        //Initialize starting parameters
        health = gens.MaxHealth;
        remainingLifeTime = gens.LifeTime;
        offspringCounter = gens.OffspringTime;
        isReadyToGrowUp = false;

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

    // Accesors
    public GenSample Gens
    {
        get { return gens; }
        set { gens = value; }
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

    public float RemainingLifeTime
    {
        get { return remainingLifeTime; }
        set { remainingLifeTime = value; }
    }

    public float PregnancyCounter
    {
        get { return pregnancyCounter; }
        set { pregnancyCounter = value; }
    }

    public float OffspringCounter
    {
        get { return offspringCounter; }
        set { offspringCounter = value; }
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
        //Debug.Log("Died with hunger: " + hunger + " and thirst: " + thirst + ", female:  " + gens.isFemale + ", pregnant status: " + isPregnant + ", lived: " + RemainingLifeTime + " secodns.");
        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
        Destroy(gameObject);
    }

    private void CheckStatuses()
    {
        if (!isAdult)
        {
            offspringCounter -= Time.deltaTime;
            if (offspringCounter <= 0)
            {
                isReadyToGrowUp = true;
            }
        }
        else
        {
            Urge += Time.deltaTime;
            RemainingLifeTime -= Time.deltaTime;
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

    private void UpdateParameters()
    {
        Hunger += Time.deltaTime * gens.HungerResistance;
        Thirst += Time.deltaTime * gens.ThirstResistance;


        if (hunger >= 100 || Thirst >= 100)
        {
            Health -= Time.deltaTime * 10;
        }
        if (health <= 0 || remainingLifeTime <= 0)
        {
            Die();
        }
    }
}
