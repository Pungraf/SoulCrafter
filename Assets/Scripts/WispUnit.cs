using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispUnit : Unit
{
    public GameObject eggPrefab;

    public override void Initialize()
    {
        base.Initialize();
        transform.SetParent(SoulsManager.Instance.WispsHolder);
    }
}
