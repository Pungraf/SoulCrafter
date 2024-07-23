using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPackManager : PackManager
{
    public override void DisbandPack()
    {
        foreach (UnitPackManager packMember in Pack)
        {
            packMember.DisbandPlayer();
        }
        base.DisbandPack();
    }
}
