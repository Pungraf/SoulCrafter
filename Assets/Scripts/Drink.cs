using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drink : MonoBehaviour
{
    [SerializeField] protected float hydration;
    public void Drinking(Unit drinkingUnit)
    {
        float amountToConsume = drinkingUnit.Gens.consumingSpeed * Time.deltaTime;

        if (hydration > amountToConsume)
        {
            drinkingUnit.Hydrate(amountToConsume);
            hydration -= amountToConsume;
        }
        else
        {
            drinkingUnit.Hydrate(hydration);
            Destroy(gameObject);
        }

    }
}
