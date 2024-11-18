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

    [SerializeField] private RectTransform ActionButtonPanel;
    [SerializeField] private TextMeshProUGUI actionButtonText;
    [SerializeField] private TextMeshProUGUI actionNameText;

    [SerializeField] private RectTransform itemDescriptionPanel;

    [SerializeField] private RectTransform altarGensPanel;
    [SerializeField] private RectTransform altarPanel;

    [SerializeField] private RectTransform spliceCorePanel;

    private ItemDescriptionUI itemDescriptionUI;

    private void Start()
    {
        itemDescriptionUI = itemDescriptionPanel.GetComponent<ItemDescriptionUI>();
    }

    public RectTransform AltarGensPanel
    {
        get { return altarGensPanel; } set { altarGensPanel = value; }
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

    public void EnableItemDescriptionPanel(Vector2 position, string itemName, string itemDescription)
    {
        itemDescriptionPanel.gameObject.SetActive(true);
        itemDescriptionUI.SetupItemDescriptioPanel(position, itemName, itemDescription);
    }

    public void DisableItemDescriptionPanel()
    {
        itemDescriptionPanel.gameObject.SetActive(false);
    }

    public void  AltarPanelChangeState()
    {
        if(!altarPanel.gameObject.activeSelf)
        {
            DisableAllWindows();
        }
        altarPanel.gameObject.SetActive(!altarPanel.gameObject.activeSelf);
    }

    public void DisableAltarPanel()
    {
        altarPanel.gameObject.SetActive(false) ;
    }

    public void SpliceCorePanelChangeState()
    {
        if(!spliceCorePanel.gameObject.activeSelf)
        {
            DisableAllWindows();
        }
        spliceCorePanel.gameObject.SetActive(!spliceCorePanel.gameObject.activeSelf);
    }

    public void DisableSpliceCorePanel()
    {
        spliceCorePanel.gameObject.SetActive(false);
    }

    public void DisableAllWindows()
    {
        DisableSpliceCorePanel();
        DisableAltarPanel();
    }
}
