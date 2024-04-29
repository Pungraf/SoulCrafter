using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispEgg : UnitEgg
{
    public override void Initialize(GenSample gen, GameObject evolveUnit, float health = 100, float hunger = 0, float thirst = 0)
    {
        base.Initialize(gen, evolveUnit, health, hunger, thirst);
        transform.SetParent(SoulsManager.Instance.WispsHolder);
    }
}
