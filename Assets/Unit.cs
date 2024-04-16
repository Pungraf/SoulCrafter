using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private int health = 100;
    [SerializeField] private int hungry = 0;
    [SerializeField] private int thirst = 0;
    [SerializeField] private float senseRadius = 10f;
    [SerializeField] private float interactionRadius = 2f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Hungry >= 100)
        {
            Health--;
        }
    }

    public void Feed(int amount)
    {
        hungry -= amount;
        if(hungry < 0)
        {
            hungry = 0;
        }
    }

    public int Health
    {
        get { return health; }
        set { health = value; }

    }

    public int Hungry
    {
        get { return hungry; }
        set { hungry = value; }

    }

    public int Thirst
    {
        get { return thirst; }
        set { thirst = value; }

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
}
