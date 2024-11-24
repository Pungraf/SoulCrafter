using Scripts.Player.AbilitySystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public PlayerSkillManager PlayerSkillManager => _playerSkillManager;
    public UIDocument UIDocument => _uiDocument;

    private VisualElement _abilityTopRow, _abilityMiddleTow, _abilityBottomRow;
    [SerializeField] private List<UIAbilityButton> _abliltyButtons;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one UIManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;

        itemDescriptionUI = itemDescriptionPanel.GetComponent<ItemDescriptionUI>();
        _playerSkillManager = WorldManager.Instance.Player.GetComponent<PlayerSkillManager>();
        _uiDocument = GetComponent<UIDocument>();
    }

    [SerializeField] private ScriptableSkillLibrary skillLibrary;
    public ScriptableSkillLibrary SkillLibrary => skillLibrary;
    [SerializeField] private VisualTreeAsset uiAbilityButton;

    [SerializeField] private GameObject InventoryGO;

    [SerializeField] private RectTransform ActionButtonPanel;
    [SerializeField] private TextMeshProUGUI actionButtonText;
    [SerializeField] private TextMeshProUGUI actionNameText;

    [SerializeField] private RectTransform itemDescriptionPanel;

    [SerializeField] private RectTransform altarGensPanel;
    [SerializeField] private RectTransform altarPanel;

    [SerializeField] private RectTransform spliceCorePanel;

    private PlayerSkillManager _playerSkillManager;
    private UIDocument _uiDocument;
    private ItemDescriptionUI itemDescriptionUI;

    public RectTransform AltarGensPanel
    {
        get { return altarGensPanel; } set { altarGensPanel = value; }
    }

    private void Start()
    {
        CreateAbilityButton();
    }

    public void ToggleInventoryPanel()
    {
        InventoryGO.SetActive(!InventoryGO.activeSelf);
        if (InventoryGO.activeSelf == false)
        {
            DisableItemDescriptionPanel();
        }
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
        if(!spliceCorePanel.gameObject.activeSelf)
        {
            DisableItemDescriptionPanel();
        }
    }

    public void DisableSpliceCorePanel()
    {
        spliceCorePanel.gameObject.SetActive(false);
        DisableItemDescriptionPanel();
    }

    public void DisableAllWindows()
    {
        DisableSpliceCorePanel();
        DisableAltarPanel();
    }

    private void CreateAbilityButton()
    {
        var root = _uiDocument.rootVisualElement;
        _abilityBottomRow = root.Q<VisualElement>("Ability_RowOne");
        _abilityMiddleTow = root.Q<VisualElement>("Ability_RowTwo");
        _abilityTopRow = root.Q<VisualElement>("Ability_RowThree");

        SpawnButtons(_abilityBottomRow, skillLibrary.GetSkillsOfTier(1));
        SpawnButtons(_abilityMiddleTow, skillLibrary.GetSkillsOfTier(2));
        SpawnButtons(_abilityTopRow, skillLibrary.GetSkillsOfTier(3));

    }

    private void SpawnButtons(VisualElement parent, List<ScriptableSkill> skills)
    {
        foreach( var skill in skills)
        {
            Button cloneButton = uiAbilityButton.CloneTree().Q<Button>();
            _abliltyButtons.Add(new UIAbilityButton(cloneButton, skill));
            parent.Add(cloneButton);
        }
    }
}
