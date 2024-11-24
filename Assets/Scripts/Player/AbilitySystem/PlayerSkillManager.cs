using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts.Player.AbilitySystem
{
    public class PlayerSkillManager : MonoBehaviour
    {
        private int _strength, _dexterity, _intelligence, _wisdom, _charisma, _constitution;
        private int _swing, _whirl, _throw;
        private int _skillPoints;

        public int Strength => _strength;
        public int Dexterity => _dexterity;
        public int Intelligence => _intelligence;
        public int Wisdom => _wisdom;
        public int Charisma => _charisma;
        public int Constitution => _constitution;

        public bool Swing => _swing > 0;
        public bool Whirl => _whirl > 0;
        public bool Throw => _throw > 0;

        public int SkillPoints => _skillPoints;

        public UnityAction OnSkillPointsChnaged;

        private List<ScriptableSkill> _unlockedSkills = new List<ScriptableSkill>();

        private void Awake()
        {
            _strength = 10;
            _dexterity = 10;
            _intelligence = 10;
            _wisdom = 10;
            _charisma = 10;
            _constitution = 10;
            _skillPoints = 5;
        }

        public void GainSkillPoint()
        {
            _skillPoints++;
            OnSkillPointsChnaged?.Invoke();
        }

        public bool CanAffordSkill(ScriptableSkill skil)
        {
            return _skillPoints >= skil.Cost;
        }

        public void UnlockSkill(ScriptableSkill skill)
        {
            if (!CanAffordSkill(skill)) return;
            ModifyStats(skill);
            _unlockedSkills.Add(skill);
            _skillPoints -= skill.Cost;
            OnSkillPointsChnaged?.Invoke();
        }

        private void ModifyStats(ScriptableSkill skill)
        {
            foreach (UpgradeData data in skill.UpgradeDataList)
            {
                switch (data.StatType)
                {
                    case StatTypes.Strength:
                        ModifyStat(ref _strength, data);
                        break;
                    case StatTypes.Dexterity:
                        ModifyStat(ref _dexterity, data);
                        break;
                    case StatTypes.Intelligence:
                        ModifyStat(ref _intelligence, data);
                        break;
                    case StatTypes.Wisdom:
                        ModifyStat(ref _wisdom, data);
                        break;
                    case StatTypes.Charisma:
                        ModifyStat(ref _charisma, data);
                        break;
                    case StatTypes.Constitution:
                        ModifyStat(ref _constitution, data);
                        break;
                    case StatTypes.Swing:
                        ModifyStat(ref _swing, data);
                        break;
                    case StatTypes.Whirl:
                        ModifyStat(ref _whirl, data);
                        break;
                    case StatTypes.Throw:
                        ModifyStat(ref _throw, data);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public bool IsSkillUnlocked(ScriptableSkill skill)
        {
            return _unlockedSkills.Contains(skill);
        }

        public bool PreReqaMet(ScriptableSkill skill)
        {
            return skill.SkillPrerequisites.Count == 0 || skill.SkillPrerequisites.All(_unlockedSkills.Contains);
        }

        private void ModifyStat(ref int stat, UpgradeData data)
        {
            if (data.IsPercentage) stat += (int)(stat * (data.SkillIncreaseAmount / 100f));
            else stat += data.SkillIncreaseAmount;
        }
    }
}
