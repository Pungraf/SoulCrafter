using Scripts.Player.AbilitySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UISkillDescriptionPanel : MonoBehaviour
{
    private UIManager _uiManager;
    private ScriptableSkill _assignedSkill;
    private VisualElement _skillImage;
    private Label _skillNameLabel, _skillDescriptionLabel, _skillCostLabel, _skillPreReqLabel;
    private Button _purchaseSkillButton;

    private void Awake()
    {
        _uiManager= GetComponentInParent<UIManager>();
    }

    private void OnEnable()
    {
        UIAbilityButton.OnSkillButtonClicked += PopulateLabelText;
    }

    private void OnDisable()
    {
        UIAbilityButton.OnSkillButtonClicked -= PopulateLabelText;
        if(_purchaseSkillButton != null) _purchaseSkillButton.clicked -= PurchaseSkill;
    }

    private void Start()
    {
        GatherLebelReferences();
        var skill = _uiManager.UIAbilitySystem.SkillLibrary.GetSkillsOfTier(1)[0];
        PopulateLabelText(skill);
    }

    private void GatherLebelReferences()
    {
        _skillImage = _uiManager.UIAbilitySystem.UIDocument.rootVisualElement.Q<VisualElement>("Icon");
        _skillNameLabel = _uiManager.UIAbilitySystem.UIDocument.rootVisualElement.Q<Label>("SkillNameLabel");
        _skillDescriptionLabel = _uiManager.UIAbilitySystem.UIDocument.rootVisualElement.Q<Label>("SkillDescriptionLabel");
        _skillCostLabel = _uiManager.UIAbilitySystem.UIDocument.rootVisualElement.Q<Label>("SkillCostLabel");
        _skillPreReqLabel = _uiManager.UIAbilitySystem.UIDocument.rootVisualElement.Q<Label>("PreReqLabel");
        _purchaseSkillButton = _uiManager.UIAbilitySystem.UIDocument.rootVisualElement.Q<Button>("BuySkillButton");
        _purchaseSkillButton.clicked += PurchaseSkill;
    }

    private void PurchaseSkill()
    {
        if (_uiManager.UIAbilitySystem.PlayerSkillManager.CanAffordSkill(_assignedSkill))
        {
            _uiManager.UIAbilitySystem.PlayerSkillManager.UnlockSkill(_assignedSkill);
            PopulateLabelText(_assignedSkill);
        }
    }

    private void PopulateLabelText(ScriptableSkill skill)
    {
        if (skill is null) return;
        _assignedSkill = skill;

        if(_assignedSkill.SkillIcon) _skillImage.style.backgroundImage = new StyleBackground(_assignedSkill.SkillIcon);
        _skillNameLabel.text = _assignedSkill.SkillName;
        _skillDescriptionLabel.text = _assignedSkill.SkillDescription;
        _skillCostLabel.text = $"Cost: {skill.Cost}";

        if(_assignedSkill.SkillPrerequisites.Count > 0)
        {
            _skillPreReqLabel.text = "Prerequisites:";
            foreach (var preReq in _assignedSkill.SkillPrerequisites)
            {
                var lasIndex = _assignedSkill.SkillPrerequisites.Count - 1;
                string punctuation = preReq == _assignedSkill.SkillPrerequisites[lasIndex] ? "" : ",";
                _skillPreReqLabel.text += $" {preReq.SkillName}{punctuation}";
            }
        }
        else
        {
            _skillPreReqLabel.text = "";
        }
        
        if(_uiManager.UIAbilitySystem.PlayerSkillManager.IsSkillUnlocked(_assignedSkill))
        {
            _purchaseSkillButton.text = "Purchased";
            _purchaseSkillButton.SetEnabled(false);
        }
        else if (!_uiManager.UIAbilitySystem.PlayerSkillManager.PreReqaMet(_assignedSkill))
        {
            _purchaseSkillButton.text = "Prerequisites Not Met";
            _purchaseSkillButton.SetEnabled(false);
        }
        else if(!_uiManager.UIAbilitySystem.PlayerSkillManager.CanAffordSkill(_assignedSkill))
        {
            _purchaseSkillButton.text = "Can't Afford";
            _purchaseSkillButton.SetEnabled(false);
        }
        else
        {
            _purchaseSkillButton.text = "Purchase";
            _purchaseSkillButton.SetEnabled(true);
        }
    }
}
