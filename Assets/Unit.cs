using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private bool isFemale = false;
    [SerializeField] private bool isAdult = true;
    [SerializeField] private float health = 100;
    [SerializeField] private float urge = 0;
    [SerializeField] private float hungry = 0;
    [SerializeField] private float hungryTreshold = 80;
    [SerializeField] private float thirstTreshold = 50;
    [SerializeField] private float thirst = 0;
    [SerializeField] private float consumingSpeed = 1;
    [SerializeField] private float lifeTime = 600;
    [SerializeField] private float pregnancyTime = 100;
    [SerializeField] private float offspringTime = 100;
    [SerializeField] private float senseRadius = 10f;
    [SerializeField] private float interactionRadius = 2f;

    [SerializeField] public GameObject offspringPrefab;
    [SerializeField] public GameObject malePrefab;
    [SerializeField] public GameObject femalePrefab;

    //Female gense
    [SerializeField] private bool isPregnant = false;

    //male gense
    [SerializeField] private float attractiveness = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Hungry >= 100)
        {
            Health -= Time.deltaTime * 10;
        }
        if (health <= 0)
        {
            Debug.Log("Died with hunger: " + hungry + " and thirst: " + thirst + ", female:  " + isFemale + ", pregnant status: " + isPregnant);
            Die();
        }
    }

    public void Feed(float amount)
    {
        hungry -= amount;
        if(hungry < 0)
        {
            hungry = 0;
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

    public float Health
    {
        get { return health; }
        set { health = value; }

    }

    public float Hungry
    {
        get { return hungry; }
        set { hungry = value; }

    }

    public float HungryTreshold
    {
        get { return hungryTreshold; }
        set { hungryTreshold = value; }

    }
    public float ConsumingSpeed
    {
        get { return consumingSpeed; }
        set { consumingSpeed = value; }

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

    public float ThirstTreshold
    {
        get { return thirstTreshold; }
        set { thirstTreshold = value; }

    }

    public float SenseRadius
    {
        get { return senseRadius; }
        set { senseRadius = value; }

    }

    public float InteractionRadius
    {
        get { return interactionRadius; }
        set { interactionRadius = value; }

    }

    public bool IsFemale
    {
        get { return isFemale; }
        set { isFemale = value; }
    }

    public bool IsAdult
    {
        get { return isAdult; }
        set { isAdult = value; }
    }

    public float PregnancyTime
    {
        get { return pregnancyTime; }
        set
        {
            pregnancyTime = value;
        }
    }

    public float OffspringTime
    {
        get { return offspringTime; }
        set
        {
            offspringTime = value;
        }
    }

    public bool IsPregnant
    {
        get { return isPregnant;}
        set { isPregnant = value;}
    }

    public float LifeTime
    {
        get { return lifeTime; }
        set
        {
            lifeTime = value;
        }
    }

    public float Attractiveness
    {
        get { return attractiveness; }
        set
        {
            attractiveness = value;
        }
    }

    public void Die()
    {
        Debug.Log("Died with hunger: " + hungry + " and thirst: " + thirst + ", female:  " + isFemale + ", pregnant status: " + isPregnant + ", lived: " + LifeTime + " secodns.");

        Destroy(gameObject);
    }
}
