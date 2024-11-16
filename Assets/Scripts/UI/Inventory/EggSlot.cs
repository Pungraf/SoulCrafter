using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggSlot : InventorySlot
{
    protected override bool AvalaibleItemType(InventoryItem dropedItem)
    {
        if(dropedItem.Item.GetType() != typeof(EggItem))
        {
            return false;
        }

        return base.AvalaibleItemType(dropedItem);
    }
}
