using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Altar : MonoBehaviour
{
    [SerializeField] private GameObject genShardPrefab;

    public Unit HeldUnit;

    private List<GenPanel> genPanels = new List<GenPanel>();

    private VisualElement m_AltarPanel;
    private ScrollView m_GensListScrollPanel;
    private Button m_sacrificeButton;
    [SerializeField] private VisualTreeAsset uiGenElement;

    private GenPanel selectedGenPanel;

    private void Start()
    {
        m_AltarPanel = InventoryManager.Instance.M_Root.Query<VisualElement>("AltarPanel");
        m_GensListScrollPanel = m_AltarPanel.Query<ScrollView>("GensContainer");

        m_sacrificeButton = m_AltarPanel.Query<Button>("SacrificeButton");
        if (m_sacrificeButton != null)
        {
            m_sacrificeButton.clicked += Sacrifice;
        }

        GenPanel.OnGenPanelSelected += HandleGenPanelSelected;
    }

    private void OnDestroy()
    {
        GenPanel.OnGenPanelSelected -= HandleGenPanelSelected;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Food>() != null)
        {
            other.GetComponent<Food>().CurrentLifeTime = 0.01f;
        }
        Unit unit = other.GetComponent<Unit>();
        if(unit != null && HeldUnit == null)
        {
            CaptureUnit(unit);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Unit unit = other.GetComponent<Unit>();
        if(HeldUnit == unit)
        {
            HeldUnit = null;
            ClearAltarPanel();
        }
    }

    private void HandleGenPanelSelected(GenPanel selectedPanel)
    {
        if (selectedPanel.IsSelected) selectedGenPanel = selectedPanel;
        else if (!selectedGenPanel.IsSelected && selectedGenPanel == selectedPanel) selectedGenPanel = null;
        
        foreach (var panel in genPanels)
        {
            if (panel != selectedGenPanel && panel.IsSelected)
            {
                panel.IsSelected = false;
            }
        }
    }

    private void CaptureUnit(Unit capturedUnit)
    {
        GameObject unitGO = capturedUnit.gameObject;

        capturedUnit.controller.Brain.ClearAllPeristentBehaviours();
        Vector3 destination = transform.position;
        capturedUnit.controller.ForceBehaviour(BaseBehaviour.Behaviour.Incapacitated);

        HeldUnit = capturedUnit;

        unitGO.transform.position = transform.position;

        FillAltarPanelWithGens(capturedUnit.Gens);
    }

    private void FillAltarPanelWithGens(GenSample genSample)
    {
        if(genSample != null)
        {
            FieldInfo[] properties = typeof(GenSample).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

            foreach (var property in properties)
            {
                if (property.FieldType == typeof(SingleGen))
                {
                    SingleGen singleGen = (SingleGen)property.GetValue(genSample);

                    Button genPanel = uiGenElement.CloneTree().Q<Button>();
                    GenPanel panel = new GenPanel(genPanel, singleGen);
                    genPanels.Add(panel);
                    m_GensListScrollPanel.Add(genPanel);
                }
            }
        }
    }

    private void ClearAltarPanel()
    {
        if (m_GensListScrollPanel.childCount == 0) return;

        // Destroy all Buttons in m_GensListScrollPanel
        for (int i = m_GensListScrollPanel.childCount - 1; i >= 0; i--)
        {
            VisualElement child = m_GensListScrollPanel[i];
            if (child is Button)
            {
                m_GensListScrollPanel.Remove(child);
            }
        }

        // Clear the genPanels list
        genPanels.Clear();
        selectedGenPanel = null;
    }

    public void Sacrifice()
    {
        if(selectedGenPanel is null) return;

        Debug.Log("Sacrificed " + HeldUnit + " for " + selectedGenPanel.GetGenName() + " with value: " + selectedGenPanel.GetGenValue());
        GenShard genShard = Instantiate(genShardPrefab, HeldUnit.transform.position, Quaternion.identity).GetComponent<GenShard>();
        genShard.Initialize(selectedGenPanel.PanelGen, genShard.GenItem);

        ClearAltarPanel();
        HeldUnit.Die();
    }
}
