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
        _uiManager = GetComponent<UIManager>();
    }

    private void Start()
    {
        _uiManager.PlayerSkillManager.OnSkillPointsChnaged += PopulateLabelText;
        GatherLabelReferences();
        PopulateLabelText();
    }

    private void PopulateLabelText()
    {
        _stregthLabel.text = "STR - " + _uiManager.PlayerSkillManager.Strength.ToString();
        _dexterityLabel.text = "DEX - " + _uiManager.PlayerSkillManager.Dexterity.ToString();
        _intelligenceLabel.text = "INT - " + _uiManager.PlayerSkillManager.Intelligence.ToString();
        _wisdomLabel.text = "WIS - " + _uiManager.PlayerSkillManager.Wisdom.ToString();
        _charismaLabel.text = "CHA - " + _uiManager.PlayerSkillManager.Charisma.ToString();
        _constitutionLabel.text = "CON - " + _uiManager.PlayerSkillManager.Constitution.ToString();

        _swingLabel.text = "Swing: " + (_uiManager.PlayerSkillManager.Swing ? "Unlocked" : "Locked");
        _whirlLabel.text = "Whirl: " + (_uiManager.PlayerSkillManager.Whirl ? "Unlocked" : "Locked");
        _throwlLabel.text = "Throw: " + (_uiManager.PlayerSkillManager.Throw ? "Unlocked" : "Locked");

        _skillPointsLabel.text = "Skill Points: " + _uiManager.PlayerSkillManager.SkillPoints.ToString();
    }

    private void GatherLabelReferences()
    {
        _stregthLabel = _uiManager.UIDocument.rootVisualElement.Q<Label>("StatLabel_Strength");
        _dexterityLabel = _uiManager.UIDocument.rootVisualElement.Q<Label>("StatLabel_Dexterity");
        _intelligenceLabel = _uiManager.UIDocument.rootVisualElement.Q<Label>("StatLabel_Intelligence");
        _wisdomLabel = _uiManager.UIDocument.rootVisualElement.Q<Label>("StatLabel_Wisdom");
        _charismaLabel = _uiManager.UIDocument.rootVisualElement.Q<Label>("StatLabel_Charisma");
        _constitutionLabel = _uiManager.UIDocument.rootVisualElement.Q<Label>("StatLabel_Constitution");

        _swingLabel = _uiManager.UIDocument.rootVisualElement.Q<Label>("AbilityLabel_Swing");
        _whirlLabel = _uiManager.UIDocument.rootVisualElement.Q<Label>("AbilityLabel_Whirl");
        _throwlLabel = _uiManager.UIDocument.rootVisualElement.Q<Label>("AbilityLabel_Throw");

        _skillPointsLabel = _uiManager.UIDocument.rootVisualElement.Q<Label>("AbilityPontsLabel");
    }
}
