using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    [SerializeField] protected float nutritiousness;
    public void Eat(Unit eatingUnit)
    {
        float amountToConsume = eatingUnit.Gens.consumingSpeed * Time.deltaTime;

        if(nutritiousness > amountToConsume)
        {
            eatingUnit.Feed(amountToConsume);
            nutritiousness -= amountToConsume;
        }
        else
        {
            eatingUnit.Feed(nutritiousness);
            Destroy(gameObject);
        }
        
    }
}
