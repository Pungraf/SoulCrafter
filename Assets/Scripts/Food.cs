using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
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

    private void Start()
    {
        Initialize();
        Ticker.Tick_05 += Update_Tick05;
    }

    private void Update()
    {
        currentLifeTime -= Time.deltaTime;
    }

    protected void Update_Tick05(object sender, EventArgs e)
    {
        if (currentLifeTime <= 0)
        {
            Ticker.Tick_05 -= Update_Tick05;
            Destroy(gameObject);
        }
    }

    public void Eat(Unit eatingUnit)
    {
        float amountToConsume = eatingUnit.Gens.Ingestion * Time.deltaTime;

        if(currentNutritiousness > amountToConsume)
        {
            eatingUnit.Feed(amountToConsume);
            currentNutritiousness -= amountToConsume;
        }
        else
        {
            eatingUnit.Feed(currentNutritiousness);
            Ticker.Tick_05 -= Update_Tick05;
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
    }
}
