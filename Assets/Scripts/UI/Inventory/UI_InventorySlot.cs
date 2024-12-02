using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;


public class UI_InventorySlot : VisualElement
{
    private Image _icon;
    private Item _item;

    public Image Icon => _icon;
    public Item Item => _item;

    public UI_InventorySlot()
    {
        _icon = new Image();
        Add(_icon);

        _icon.AddToClassList("slotIcon");
        AddToClassList("slotContainer");

        RegisterCallback<PointerDownEvent>(OnPointerDown);
    }

    private void OnPointerDown(PointerDownEvent evt)
    {
        //Not the left mouse button
        if (evt.button != 0 || _item == null)
        {
            return;
        }

        //Clear the image
        Icon.image = null;

        //Start the drag
        InventoryManager.Instance.StartDrag(evt.position, this);
    }

    public void AddItemToSlot(Item item, int count = 1)
    {
        _item = item;
        _icon.sprite = item.GetSprite();
    }

    public void RemoveItemFromSlot()
    {
        _item = null;
        _icon.sprite = null;
    }
    #region UXML
    [Preserve]
    public new class UxmlFactory : UxmlFactory<UI_InventorySlot, UxmlTraits> { }

    [Preserve]
    public new class UxmlTraits : VisualElement.UxmlTraits { }
    #endregion
}
