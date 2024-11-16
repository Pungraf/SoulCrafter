using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleGenSlot : InventorySlot
{
    protected override bool AvalaibleItemType(InventoryItem dropedItem)
    {
        if (dropedItem.Item.GetType() != typeof(GenItem))
        {
            return false;
        }

        return base.AvalaibleItemType(dropedItem);
    }
}
