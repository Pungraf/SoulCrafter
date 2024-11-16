using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    [SerializeField] protected InventoryItem itemInSlot;

    public virtual InventoryItem ItemInSlot
    {
        set { itemInSlot = value; }
        get { return itemInSlot; }
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        InventoryItem inventoryItem = dropped.GetComponent<InventoryItem>();
        if (!AvalaibleItemType(inventoryItem))
        {
            return;
        }
        if (transform.childCount == 0)
        {
            if (inventoryItem != null)
            {
                inventoryItem.parentAfterDrag = transform;
                SetItemToSlot(inventoryItem);
            }
        }
        else if (transform.childCount == 1 && (ItemInSlot.Count < ItemInSlot.GetItem().MaxStack))
        {
            if (inventoryItem.GetType() == ItemInSlot.GetType() && inventoryItem.GetItem().IsStackable)
            {
                if (inventoryItem.Count + ItemInSlot.Count <= ItemInSlot.GetItem().MaxStack)
                {
                    ItemInSlot.Count++;
                    Destroy(dropped);
                }
                else
                {
                    int amountOverMaxStack = ItemInSlot.Count + inventoryItem.Count - ItemInSlot.GetItem().MaxStack;
                    ItemInSlot.Count = ItemInSlot.GetItem().MaxStack;
                    inventoryItem.Count = amountOverMaxStack;
                }
            }
        }
    }

    public virtual void SetItemToSlot(InventoryItem inventoryItem)
    {
        ItemInSlot = inventoryItem;
    }

    public InventoryItem GetItemInSlot()
    {
        return ItemInSlot;
    }

    protected virtual bool AvalaibleItemType(InventoryItem dropedItem)
    {
        return true;
    }
}
