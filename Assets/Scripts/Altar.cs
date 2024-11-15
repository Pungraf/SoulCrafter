using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Altar : MonoBehaviour
{
    [SerializeField] private GameObject genShardPrefab;

    public Unit HeldUnit;
    public GameObject subPanelPrefab;

    private RectTransform altarGensPanel;

    private void Start()
    {
        altarGensPanel = UIManager.Instance.AltarGensPanel;
    }

    private void OnTriggerEnter(Collider other)
    {
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
        if(altarGensPanel != null)
        {
            FieldInfo[] properties = typeof(GenSample).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

            foreach (var property in properties)
            {
                if (property.FieldType == typeof(SingleGen))
                {
                    SingleGen singleGen = (SingleGen)property.GetValue(genSample);
                    float genValue = singleGen.Value;
                    string value = singleGen.Type.ToString();
                    CreateSubPanel(value, genValue, singleGen);
                }
            }
        }
    }

    private void ClearAltarPanel()
    {
        if (altarGensPanel.childCount == 0) return;

        for (int i = altarGensPanel.childCount - 1; i >= 0; i--)
        {
            Destroy(altarGensPanel.GetChild(i).gameObject);
        }
    }

    private void CreateSubPanel(string genName, float genValue, SingleGen panelGen)
    {
        GenPanelUI subPanel = Instantiate(subPanelPrefab, altarGensPanel).GetComponent<GenPanelUI>();

        subPanel.SetGenPanel(genName, genValue, panelGen);
    }

    public void Sacrifice()
    {
        GenPanelUI selectedGenPanel = null;
        foreach(Toggle toggle in altarGensPanel.GetComponentsInChildren<Toggle>())
        {
            if (toggle.isOn)
            {
                selectedGenPanel = toggle.GetComponent<GenPanelUI>();
                break;
            }
        }
        if(selectedGenPanel != null)
        {
            Debug.Log("Sacrificed " + HeldUnit + " for " + selectedGenPanel.GetGenName() + " with value: " + selectedGenPanel.GetGenValue());
            GenShard genShard = Instantiate(genShardPrefab, HeldUnit.transform.position, Quaternion.identity).GetComponent<GenShard>();
            genShard.Initialize(selectedGenPanel.PanelGen);

            ClearAltarPanel();
            HeldUnit.Die();
        }
        else
        {
            Debug.Log("No gen to sacrifice.");
        }
    }
}
