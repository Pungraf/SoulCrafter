using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Scripts.Player.AbilitySystem
{ 
    [CreateAssetMenu(fileName = "New Skill", menuName = "Skill System/New Skill", order = 0)]
    public class ScriptableSkill : ScriptableObject
    {
        public List<UpgradeData> UpgradeDataList = new List<UpgradeData>();
        public bool IsAbility;
        public string SkillName;
        public bool OveriteDescription;
        [TextArea(1,4)] public string SkillDescription;
        public Sprite SkillIcon;
        public List<ScriptableSkill> SkillPrerequisites = new List<ScriptableSkill>();
        public int SkillTier;
        public int Cost;

        private void OnValidate()
        {
            SkillName = name;
            if (UpgradeDataList.Count ==0) return;
            if (OveriteDescription) return;

            GenerateDescription();
        }

        private void GenerateDescription()
        {
            if (IsAbility)
            {
                switch (UpgradeDataList[0].StatType)
                {
                    case StatTypes.Swing:
                        SkillDescription = $"{SkillName} grants the Swing ability.";
                        break;
                    case StatTypes.Whirl:
                        SkillDescription = $"{SkillName} grants the Whirl ability.";
                        break;
                    case StatTypes.Throw:
                        SkillDescription = $"{SkillName} grants the Throw ability.";
                        break;
                }
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"{SkillName} increases ");
                for(int i = 0; i < UpgradeDataList.Count; i++)
                {
                    sb.Append(UpgradeDataList[i].StatType.ToString());
                    sb.Append (" by ");
                    sb.Append(UpgradeDataList[i].SkillIncreaseAmount.ToString());
                    sb.Append(UpgradeDataList[i].IsPercentage ? "%" : " points");
                    if (i == UpgradeDataList.Count - 2) sb.Append(" and ");
                    sb.Append(i < UpgradeDataList.Count - 1 ? ", " : ".");
                }

                SkillDescription = sb.ToString();
            }
        }
    }

    [System.Serializable]
    public class UpgradeData
    {
        public StatTypes StatType;
        public int SkillIncreaseAmount;
        public bool IsPercentage;
    }

    public enum StatTypes 
    {
        Strength,
        Dexterity,
        Intelligence,
        Wisdom,
        Charisma,
        Constitution,
        Swing,
        Whirl,
        Throw
    }

}
