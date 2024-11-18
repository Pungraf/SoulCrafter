using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemDescriptionUI : MonoBehaviour
{
    [SerializeField] private RectTransform mainPanelRectTrasnform;
    [SerializeField] private RectTransform namePanellRectTrasnform;
    [SerializeField] private RectTransform descriptionPanelRectTrasnform;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescription;

    [SerializeField] float textPaddings = 10f;

    public void SetupItemDescriptioPanel(Vector2 position, string itemName, string itemDescription)
    {
        mainPanelRectTrasnform.position = position;
        this.itemName.text = itemName;
        this.itemDescription.text = itemDescription;

        Vector2 descriptionPanleSize = descriptionPanelRectTrasnform.sizeDelta;
        descriptionPanleSize.y = this.itemDescription.preferredHeight + textPaddings;
        Vector2 mainPanleSize = mainPanelRectTrasnform.sizeDelta;
        mainPanleSize.y = descriptionPanleSize.y + namePanellRectTrasnform.sizeDelta.y + 2 * textPaddings;

        descriptionPanelRectTrasnform.sizeDelta = descriptionPanleSize;
        mainPanelRectTrasnform.sizeDelta = mainPanleSize;
    }
}
