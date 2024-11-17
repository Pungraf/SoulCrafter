using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleGenSlot : InventorySlot
{
    public event EventHandler OnSlotItemChanged;

    public override InventoryItem ItemInSlot
    {
        set
        {
            itemInSlot = value;
            if (value != null)
            {
                ItemInSlot.ButtonIsActive = false;
            }
            OnSlotItemChanged?.Invoke(this, EventArgs.Empty);
        }
        get { return itemInSlot; }
    }

    protected override bool AvalaibleItemType(InventoryItem dropedItem)
    {
        if(dropedItem.Item.GetType() != typeof(GenItem))
        {
            return false;
        }

        return base.AvalaibleItemType(dropedItem);
    }
}