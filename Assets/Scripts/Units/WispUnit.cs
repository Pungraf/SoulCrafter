using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispUnit : Unit
{
    public override void Initialize()
    {
        base.Initialize();
        transform.SetParent(SoulsManager.Instance.WispsHolder);
    }
}
