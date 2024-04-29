using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispUnit : Unit
{
    public GameObject eggPrefab;

    public override void Initialize(GenSample gen = null, float health = 100, float hunger = 0, float thirst = 0)
    {
        base.Initialize(gen, health, hunger, thirst);
        transform.SetParent(SoulsManager.Instance.WispsHolder);
    }
}
