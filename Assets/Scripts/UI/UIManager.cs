using Scripts.Player.AbilitySystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private VisualElement m_Root;
    public VisualElement M_Root => m_Root;

    public UIAbilitySystem UIAbilitySystem => _uIAbilitySystem;

    private UIAbilitySystem _uIAbilitySystem;

    [SerializeField] private RectTransform ActionButtonPanel;
    [SerializeField] private TextMeshProUGUI actionButtonText;
    [SerializeField] private TextMeshProUGUI actionNameText;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one UIManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _uIAbilitySystem = GetComponentInChildren<UIAbilitySystem>();


        m_Root = GetComponent<UIDocument>().rootVisualElement;
    }


    public void EnableActionButtonPanel(Vector2 position, string buttonName, string actionName)
    {
        ActionButtonPanel.position = position;
        actionButtonText.text = buttonName;
        actionNameText.text = actionName;
        ActionButtonPanel.gameObject.SetActive(true);
    }

    public void DisableActionButtonPanel()
    {
        ActionButtonPanel.gameObject.SetActive(false);
    }

    public void SetActionButtonPanelPosition(Vector2 position)
    {
        ActionButtonPanel.position = position;
    }

}
