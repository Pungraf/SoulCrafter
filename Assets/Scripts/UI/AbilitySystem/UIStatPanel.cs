using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIStatPanel : MonoBehaviour
{
    private Label _stregthLabel, _dexterityLabel, _intelligenceLabel, _wisdomLabel, _charismaLabel, _constitutionLabel;
    private Label _swingLabel, _whirlLabel, _throwlLabel;
    private Label _skillPointsLabel;

    private UIManager _uiManager;

    private void Awake()
    {
        _uiManager = GetComponentInParent<UIManager>();
    }

    private void Start()
    {
        _uiManager.UIAbilitySystem.PlayerSkillManager.OnSkillPointsChnaged += PopulateLabelText;
        GatherLabelReferences();
        PopulateLabelText();
    }

    private void PopulateLabelText()
    {
        _stregthLabel.text = "STR - " + _uiManager.UIAbilitySystem.PlayerSkillManager.Strength.ToString();
        _dexterityLabel.text = "DEX - " + _uiManager.UIAbilitySystem.PlayerSkillManager.Dexterity.ToString();
        _intelligenceLabel.text = "INT - " + _uiManager.UIAbilitySystem.PlayerSkillManager.Intelligence.ToString();
        _wisdomLabel.text = "WIS - " + _uiManager.UIAbilitySystem.PlayerSkillManager.Wisdom.ToString();
        _charismaLabel.text = "CHA - " + _uiManager.UIAbilitySystem.PlayerSkillManager.Charisma.ToString();
        _constitutionLabel.text = "CON - " + _uiManager.UIAbilitySystem.PlayerSkillManager.Constitution.ToString();

        _swingLabel.text = "Swing: " + (_uiManager.UIAbilitySystem.PlayerSkillManager.Swing ? "Unlocked" : "Locked");
        _whirlLabel.text = "Whirl: " + (_uiManager.UIAbilitySystem.PlayerSkillManager.Whirl ? "Unlocked" : "Locked");
        _throwlLabel.text = "Throw: " + (_uiManager.UIAbilitySystem.PlayerSkillManager.Throw ? "Unlocked" : "Locked");

        _skillPointsLabel.text = "Skill Points: " + _uiManager.UIAbilitySystem.PlayerSkillManager.SkillPoints.ToString();
    }

    private void GatherLabelReferences()
    {
        _stregthLabel = _uiManager.UIAbilitySystem.UIDocument.rootVisualElement.Q<Label>("StatLabel_Strength");
        _dexterityLabel = _uiManager.UIAbilitySystem.UIDocument.rootVisualElement.Q<Label>("StatLabel_Dexterity");
        _intelligenceLabel = _uiManager.UIAbilitySystem.UIDocument.rootVisualElement.Q<Label>("StatLabel_Intelligence");
        _wisdomLabel = _uiManager.UIAbilitySystem.UIDocument.rootVisualElement.Q<Label>("StatLabel_Wisdom");
        _charismaLabel = _uiManager.UIAbilitySystem.UIDocument.rootVisualElement.Q<Label>("StatLabel_Charisma");
        _constitutionLabel = _uiManager.UIAbilitySystem.UIDocument.rootVisualElement.Q<Label>("StatLabel_Constitution");

        _swingLabel = _uiManager.UIAbilitySystem.UIDocument.rootVisualElement.Q<Label>("AbilityLabel_Swing");
        _whirlLabel = _uiManager.UIAbilitySystem.UIDocument.rootVisualElement.Q<Label>("AbilityLabel_Whirl");
        _throwlLabel = _uiManager.UIAbilitySystem.UIDocument.rootVisualElement.Q<Label>("AbilityLabel_Throw");

        _skillPointsLabel = _uiManager.UIAbilitySystem.UIDocument.rootVisualElement.Q<Label>("AbilityPontsLabel");
    }
}
