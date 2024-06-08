using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one UIManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    [SerializeField] RectTransform UIPanel;
    [SerializeField] TextMeshProUGUI actionButtonText;
    [SerializeField] TextMeshProUGUI actionNameText;

    public void EnableActionButtonPanel(Vector2 position, string buttonName, string actionName)
    {
        UIPanel.position = position;
        actionButtonText.text = buttonName;
        actionNameText.text = actionName;
        UIPanel.gameObject.SetActive(true);
    }

    public void DisableActionButtonPanel()
    {
        UIPanel.gameObject.SetActive(false);
    }

    public void SetActionButtonPanelPosition(Vector2 position)
    {
        UIPanel.position = position;
    }
}
