using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public InventoryItem itemInSlot;


    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            GameObject dropped = eventData.pointerDrag;
            InventoryItem inventoryItem = dropped.GetComponent<InventoryItem>();
            if (inventoryItem != null)
            {
                inventoryItem.parentAfterDrag = transform;
                SetItemToSlot(inventoryItem);
            }
        }
        else if (transform.childCount == 1 && (itemInSlot.Count < itemInSlot.GetItem().MaxStack))
        {
            GameObject dropped = eventData.pointerDrag;
            InventoryItem inventoryItem = dropped.GetComponent<InventoryItem>();
            if (inventoryItem.GetType() == itemInSlot.GetType() && inventoryItem.GetItem().IsStackable)
            {
                if (inventoryItem.Count + itemInSlot.Count <= itemInSlot.GetItem().MaxStack)
                {
                    itemInSlot.Count++;
                    Destroy(dropped);
                }
                else
                {
                    int amountOverMaxStack = itemInSlot.Count + inventoryItem.Count - itemInSlot.GetItem().MaxStack;
                    itemInSlot.Count = itemInSlot.GetItem().MaxStack;
                    inventoryItem.Count = amountOverMaxStack;
                }
            }
        }
    }

    public virtual void SetItemToSlot(InventoryItem inventoryItem)
    {
        itemInSlot = inventoryItem;
    }

    public InventoryItem GetItemInSlot()
    {
        return itemInSlot;
    }
}
