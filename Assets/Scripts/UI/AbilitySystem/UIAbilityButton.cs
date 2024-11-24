using Scripts.Player.AbilitySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

[System.Serializable]
public class UIAbilityButton
{
    private Button _button;
    private ScriptableSkill _skill;
    private bool _isUnlocked = false;

    public static UnityAction<ScriptableSkill> OnSkillButtonClicked;

    public UIAbilityButton(Button assignedButton, ScriptableSkill assignedSkill)
    {
        _button = assignedButton;
        _button.clicked += OnClick;
        _skill = assignedSkill;
        if(assignedSkill.SkillIcon) _button.style.backgroundImage = new StyleBackground(assignedSkill.SkillIcon);
    }

    private void OnClick()
    {
        OnSkillButtonClicked?.Invoke(_skill);
    }
}
