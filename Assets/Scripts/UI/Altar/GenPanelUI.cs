using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GenPanelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI genName;
    [SerializeField] private TextMeshProUGUI genValue;

    private SingleGen panelGen;

    public SingleGen PanelGen
    {
        get { return panelGen; }
    }

    public void SetGenPanel(string genName, float genValue, SingleGen panelGen)
    {
        this.genName.text = genName;
        this.genValue.text = genValue.ToString();
        this.panelGen = panelGen;
        GetComponent<Toggle>().group = gameObject.GetComponentInParent<ToggleGroup>();
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
