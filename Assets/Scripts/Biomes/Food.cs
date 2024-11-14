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

    protected static float sekPerDay = 1440f;
    protected static float counterUpdateSampling = 1f;

    [SerializeField] public FoodType foodType;
    [SerializeField] protected float maxLifeTime;
    [SerializeField] protected float nutritiousness;

    [SerializeField] protected float currentLifeTime;
    [SerializeField] protected float currentNutritiousness;

    public void Eat(Unit eatingUnit)
    {
        float amountToConsume = eatingUnit.Gens.Ingestion.Value;

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
        InvokeRepeating("InvokeUpdate", 1f, counterUpdateSampling);
    }

    protected virtual void InvokeUpdate()
    {
        currentLifeTime -= counterUpdateSampling / sekPerDay;
        if(currentLifeTime < 0f)
        {
            Destroy(gameObject);
        }
    }
}
