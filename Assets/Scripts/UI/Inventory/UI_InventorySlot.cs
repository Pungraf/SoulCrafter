using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;


public class UI_InventorySlot : VisualElement
{
    public Image Icon;
    public string ItemGuid = "";

    public UI_InventorySlot()
    {
        Icon = new Image();
        Add(Icon);

        Icon.AddToClassList("slotIcon");
        AddToClassList("slotContainer");
    }

    #region UXML
    [Preserve]
    public new class UxmlFactory : UxmlFactory<UI_InventorySlot, UxmlTraits> { }

    [Preserve]
    public new class UxmlTraits : VisualElement.UxmlTraits { }
    #endregion
}
