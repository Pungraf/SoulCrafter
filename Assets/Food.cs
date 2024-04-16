using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    protected int nutritiousness;
    public void Eat(Unit eatingUnit)
    {
        eatingUnit.Feed(nutritiousness);
        Destroy(gameObject);
    }
}
