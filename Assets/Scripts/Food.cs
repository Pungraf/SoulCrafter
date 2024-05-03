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
    }

    private void Update()
    {
        currentLifeTime -= Time.deltaTime;
        if(currentLifeTime <= 0 )
        {
            Destroy(gameObject);
        }
    }
    public void Eat(Unit eatingUnit)
    {
        float amountToConsume = eatingUnit.Gens.ConsumingSpeed * Time.deltaTime;

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
    }
}
