using Scripts.Player.AbilitySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIAbilitySystem : MonoBehaviour
{
    public PlayerSkillManager PlayerSkillManager => _playerSkillManager;
    public UIDocument UIDocument => _uiDocument;

    private VisualElement _abilityTopRow, _abilityMiddleTow, _abilityBottomRow;
    [SerializeField] private List<UIAbilityButton> _abliltyButtons;

    private void Awake()
    {
        _playerSkillManager = WorldManager.Instance.Player.GetComponent<PlayerSkillManager>();
        _uiDocument = GetComponent<UIDocument>();
    }

    [SerializeField] private ScriptableSkillLibrary skillLibrary;
    public ScriptableSkillLibrary SkillLibrary => skillLibrary;
    [SerializeField] private VisualTreeAsset uiAbilityButton;

    private PlayerSkillManager _playerSkillManager;
    private UIDocument _uiDocument;

    private void Start()
    {
        CreateAbilityButton();
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
        foreach (var skill in skills)
        {
            Button cloneButton = uiAbilityButton.CloneTree().Q<Button>();
            _abliltyButtons.Add(new UIAbilityButton(cloneButton, skill));
            parent.Add(cloneButton);
        }
    }
}
