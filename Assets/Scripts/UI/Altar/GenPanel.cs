using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GenPanel
{
    public static event Action<GenPanel> OnGenPanelSelected;

    private Button genPanel;
    private float genValue;
    private string genName;
    private Label valueLabel, nameLabel;
    private SingleGen panelGen;
    private bool isSelected;
    public bool IsSelected {
        get { return isSelected; }
        set
        {
            isSelected = value;
            ChangedSelected();
            OnGenPanelSelected?.Invoke(this);
        }
    }   

    public SingleGen PanelGen
    {
        get { return panelGen; }
    }

    public GenPanel(Button genPanel, SingleGen gen)
    {
        this.genPanel = genPanel;
        panelGen = gen;
        SetGenProperties();
        GatherLebelReferences(genPanel);
        UpdatePanel();

        genPanel.RegisterCallback<ClickEvent>(evt => OnButtonClicked());
    }

    private void OnButtonClicked()
    {

        IsSelected = !IsSelected;
    }

    private void ChangedSelected()
    {
        if(isSelected)
        {
            genPanel.AddToClassList("genPanelSelected");
        }
        else
        {
            genPanel.RemoveFromClassList("genPanelSelected");
        }
    }

    public void UpdatePanel()
    {
        valueLabel.text = genValue.ToString();
        nameLabel.text = genName;
    }

    private void GatherLebelReferences(Button root)
    {
        valueLabel = root.Q<Label>("GenValueLabel");
        nameLabel = root.Q<Label>("GenNameLabel");
    }

    private void SetGenProperties()
    {
        genValue = Mathf.Round(PanelGen.Value * 100f) / 100f;
        genName = PanelGen.Type.ToString();
    }

    public string GetGenName()
    {
        return panelGen.Type.ToString();
    }

    public float GetGenValue()
    {
        return panelGen.Value;
    }
}
