using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    [SerializeField] protected float maxLifeTime;
    [SerializeField] protected float nutritiousness;

    private float currentLifeTime;
    private float currentNutritiousness;

    private void Start()
    {
        currentLifeTime = maxLifeTime;
        currentNutritiousness = nutritiousness;
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
}
