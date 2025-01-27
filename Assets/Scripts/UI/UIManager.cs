using Scripts.Player.AbilitySystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    //Inventory Main Panel
    private VisualElement m_InventoryRoot;
    public VisualElement M_InventoryRoot => m_InventoryRoot;

    private VisualElement secondaryInventoryPanel;

    public bool IsInventoryPanelActive => _isInventoryPanelActive;
    private bool _isInventoryPanelActive;

    //Altar Panel
    private VisualElement m_AltarPanel;
    public ScrollView M_GensListScrollPanel
    {
        get => m_GensListScrollPanel;
        set => m_GensListScrollPanel = value;
    }
    private ScrollView m_GensListScrollPanel;
    public Button M_sacrificeButton
    {
        get => m_sacrificeButton;
        set => m_sacrificeButton = value;
    }
    private Button m_sacrificeButton;

    //Splice Core Panel
    private VisualElement m_SpliceCorePanel;
    public UI_InventorySlot M_EggSlot
    {
        get => m_EggSlot;
        set => m_EggSlot = value;
    }
    private UI_InventorySlot m_EggSlot;
    public UI_InventorySlot M_GenSlot
    {
        get => m_GenSlot;
        set => m_GenSlot = value;
    }
    private UI_InventorySlot m_GenSlot;
    public Button M_SpliceButton
    {
        get => m_SpliceButton;
        set => m_SpliceButton = value;
    }
    private Button m_SpliceButton;

    //Ability Main Panel
    private VisualElement m_AbilityRoot;
    public VisualElement M_AbilityRoot => m_AbilityRoot;
    private VisualElement m_AbilityCointainer;
    private bool isAbilityVisible = false;

    private VisualElement m_InventoryCointainer;
    private bool isInventoryVisible = false;

    [SerializeField]
    private InputActionAsset InputActions;
    private InputActionMap UIActionMap;
    private InputAction ToggleInventory;
    private InputAction ToggleAbility;

    public UIAbilitySystem UIAbilitySystem => _uIAbilitySystem;

    private UIAbilitySystem _uIAbilitySystem;

    public InventoryManager UIInventorySystem => _UIInventorySystem;

    private InventoryManager _UIInventorySystem;

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
        _UIInventorySystem = GetComponentInChildren<InventoryManager>();

        m_InventoryRoot = _UIInventorySystem.GetComponent<UIDocument>().rootVisualElement;
        m_InventoryCointainer = M_InventoryRoot.Q<VisualElement>("Container");

        m_AltarPanel = M_InventoryRoot.Query<VisualElement>("AltarPanel");
        m_GensListScrollPanel = m_AltarPanel.Query<ScrollView>("GensContainer");
        m_sacrificeButton = m_AltarPanel.Query<Button>("SacrificeButton");

        m_SpliceCorePanel = M_InventoryRoot.Query<VisualElement>("SpliceCorePanel");
        m_EggSlot = m_SpliceCorePanel.Query<UI_InventorySlot>("EggSlot");
        m_GenSlot = m_SpliceCorePanel.Query<UI_InventorySlot>("GenSlot");
        m_SpliceButton = m_SpliceCorePanel.Query<Button>("SpliceButton");

        m_AbilityRoot = _uIAbilitySystem.gameObject.GetComponent<UIDocument>().rootVisualElement;
        m_AbilityCointainer = m_AbilityRoot.Q<VisualElement>("MainPanel");

        UIActionMap = InputActions.FindActionMap("UI");

        ToggleInventory = InputActions.FindAction("ToggleInventory");
        ToggleInventory.performed += HandleToggleInventoryPerformed;
        ToggleInventory.Enable();

        ToggleAbility = InputActions.FindAction("ToggleAbility");
        ToggleAbility.performed += HandleToggleAbilityPerformed;
        ToggleAbility.Enable();

        UIActionMap.Enable();
        InputActions.Enable();
    }

    private void Start()
    {
        ToggleAbilityUI(false);
        ToggleInventoryUI(false);
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

    // Function to toggle the visibility of the ability panel
    public void ToggleAbilityUI(bool isVisible)
    {
        if (M_AbilityRoot != null)
        {
            isAbilityVisible = isVisible;
            m_AbilityCointainer.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    private void HandleToggleAbilityPerformed(InputAction.CallbackContext Context)
    {
        ToggleAbilityUI(!isAbilityVisible);
    }

    // Function to toggle the visibility of the inventory panel
    public void ToggleInventoryUI(bool isVisible)
    {
        if (M_InventoryRoot != null)
        {
            isInventoryVisible = isVisible;
            if(isVisible)
            {
                m_InventoryCointainer.style.display = DisplayStyle.Flex;
                _isInventoryPanelActive = true;
            }
            else
            {
                m_InventoryCointainer.style.display = DisplayStyle.None;
                _isInventoryPanelActive = false;
            }
        }
    }

    private void HandleToggleInventoryPerformed(InputAction.CallbackContext Context)
    {
        ToggleInventoryUI(!isInventoryVisible);
    }

    public void ToggleAltarPanelUI(bool isVisible)
    {
        //Open main panel if is closed
        if (!IsInventoryPanelActive) ToggleInventoryUI(isVisible);
        //Disable previous secondary panel if is opened
        if (secondaryInventoryPanel != null && secondaryInventoryPanel.style.display == DisplayStyle.Flex)
        {
            secondaryInventoryPanel.style.display = DisplayStyle.None;
        }
        if (m_AltarPanel != null)
        {
            m_AltarPanel.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
            secondaryInventoryPanel = m_AltarPanel;
        }
    }

    public void ToggleSpliceCorePanelUI(bool isVisible)
    {
        //Open main panel if is closed
        if (!IsInventoryPanelActive) ToggleInventoryUI(isVisible);
        //Disable previous secondary panel if is opened
        if(secondaryInventoryPanel!= null && secondaryInventoryPanel.style.display == DisplayStyle.Flex)
        {
            secondaryInventoryPanel.style.display = DisplayStyle.None;
        }
        if (m_SpliceCorePanel != null)
        {
            m_SpliceCorePanel.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
            SpliceCoreInvetoryItemsSubscription(isVisible);
            secondaryInventoryPanel = m_SpliceCorePanel;
        }
    }

    private void SpliceCoreInvetoryItemsSubscription(bool subscribe)
    {
        if(subscribe)
        {
            InventoryManager.Instance.InventoryItems.Add(M_EggSlot);
            InventoryManager.Instance.InventoryItems.Add(M_GenSlot);
        }
        else
        {
            InventoryManager.Instance.InventoryItems.Remove(M_EggSlot);
            InventoryManager.Instance.InventoryItems.Remove(M_GenSlot);
        }
    }
}
