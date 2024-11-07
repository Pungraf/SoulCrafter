using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Food : MonoBehaviour
{
    public enum FoodType
    {
        Vegetable,
        Meat
    }

    [SerializeField] public FoodType foodType;
    [SerializeField] protected float maxLifeTime;
    [SerializeField] protected float nutritiousness;

    private float currentLifeTime;
    [SerializeField] private float currentNutritiousness;
    [SerializeField] private float updateFrequency;

    public void Eat(Unit eatingUnit)
    {
        float amountToConsume = eatingUnit.Gens.Ingestion;

        if(currentNutritiousness > amountToConsume)
        {
            eatingUnit.Feed(amountToConsume);
            currentNutritiousness -= amountToConsume;
        }
        else
        {
            eatingUnit.Feed(currentNutritiousness);
            Destroy(gameObject);
        }
        
    }
    public float Nutritiousness
    {
        get { return nutritiousness; }
        set { nutritiousness = value; }
    }

    public void Initialize()
    {
        currentLifeTime = maxLifeTime;
        currentNutritiousness = nutritiousness;
        InvokeRepeating("InvokeUpdate", 1f, updateFrequency);
    }

    protected virtual void InvokeUpdate()
    {
        currentLifeTime -= updateFrequency;
        currentNutritiousness += updateFrequency;
        if(currentLifeTime < 0f)
        {
            Destroy(gameObject);
        }
    }
}
