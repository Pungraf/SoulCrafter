using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispEgg : UnitEgg
{
    public override void Initialize(GenSample gen, GameObject evolveUnit, float durablity)
    {
        base.Initialize(gen, evolveUnit, durablity);
        transform.SetParent(SoulsManager.Instance.WispsHolder);
    }

    protected override GenSample DefaultGenSample()
    {
        return GenManager.Instance.DefaultWispGen;
    }
}
