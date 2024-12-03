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
    private Label _countLabel;
    private int _count;


    public Image Icon => _icon;
    public Item Item => _item;
    public int Count
    {
        get { return _count; }
        set 
        {
            _count = value;
            UpdateCountLabel();
        }
    }

    public UI_InventorySlot()
    {
        _icon = new Image();
        Add(_icon);
        _countLabel = new Label();
        Add(_countLabel);

        _icon.AddToClassList("slotIcon");
        _countLabel.AddToClassList("slotCountLabel");
        AddToClassList("slotContainer");

        RegisterCallback<PointerDownEvent>(OnPointerDown);
    }

    private void OnPointerDown(PointerDownEvent evt)
    {
        //Not the left mouse button
        if (evt.button == 0 & _item != null)
        {
            //Clear the image
            Icon.image = null;

            //Start the drag
            InventoryManager.Instance.StartDrag(evt.position, this);
        }
        else if(evt.button == 1 & _item != null)
        {
            UseItem();
        }
        
    }

    public void AddItemToSlot(Item item, int count = 1)
    {
        _item = item;
        _icon.sprite = item.GetSprite();
        Count = count;
    }

    public void RemoveItemFromSlot()
    {
        _item = null;
        _icon.sprite = null;
        Count = 0;
    }

    private void UpdateCountLabel()
    {
        _countLabel.text = _count > 1 ? _count.ToString() : string.Empty;
    }

    private void UseItem()
    {
        if (Item is IUsable)
        {
            IUsable usableitem = (IUsable)Item;
            usableitem.Use();
            Count--;
            if (Count < 1)
            {
                RemoveItemFromSlot();
            }
        }
        else
        {
            Debug.Log("Item is unusable");
        }
    }

    #region UXML
    [Preserve]
    public new class UxmlFactory : UxmlFactory<UI_InventorySlot, UxmlTraits> { }

    [Preserve]
    public new class UxmlTraits : VisualElement.UxmlTraits { }
    #endregion
}
