using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GenPanel
{
    private float genValue;
    private string genName;
    private Label valueLabel, nameLabel;
    private SingleGen panelGen;

    public SingleGen PanelGen
    {
        get { return panelGen; }
    }

    public GenPanel(VisualElement genPanel, SingleGen gen)
    {
        panelGen = gen;
        SetGenProperties();
        GatherLebelReferences(genPanel);
        UpdatePanel();
    }

    public void UpdatePanel()
    {
        valueLabel.text = genValue.ToString();
        nameLabel.text = genName;
    }

    private void GatherLebelReferences(VisualElement root)
    {
        valueLabel = root.Q<Label>("GenValueLabel");
        nameLabel = root.Q<Label>("GenNameLabel");
    }

    private void SetGenProperties()
    {
        genValue = Mathf.Round(PanelGen.Value * 100f) / 100f;
        genName = PanelGen.Type.ToString();
    }
}
