using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;

public class Altar : MonoBehaviour
{
    public Unit HeldUnit;
    public GameObject subPanelPrefab;

    private RectTransform altarPanel;

    private void Start()
    {
        altarPanel = UIManager.Instance.AltarPanel;
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
        if(altarPanel != null)
        {
            FieldInfo[] properties = typeof(GenSample).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

            Debug.Log(properties.Length);

            foreach (var property in properties)
            {
                if (property.FieldType == typeof(float))
                {
                    float genValue = (float)property.GetValue(genSample);
                    string value = property.Name + ": " + genValue;
                    CreateSubPanel(value);
                }
            }
        }
    }

    void CreateSubPanel(string text)
    {
        GameObject subPanel = Instantiate(subPanelPrefab, altarPanel);
        TextMeshProUGUI panelText = subPanel.GetComponentInChildren<TextMeshProUGUI>();
        if (panelText != null)
        {
            panelText.text = text.ToString();
        }
    }
}
