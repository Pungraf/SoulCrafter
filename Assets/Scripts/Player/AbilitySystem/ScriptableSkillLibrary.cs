using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Player.AbilitySystem
{
    [CreateAssetMenu(fileName = "New Skill", menuName = "Skill System/New Skill Library", order = 0)]
    public class ScriptableSkillLibrary : ScriptableObject
    {
        public List<ScriptableSkill> SkillLibrary;

        public List<ScriptableSkill> GetSkillsOfTier(int tier)
        {
            return SkillLibrary.Where(skill => skill.SkillTier == tier).ToList();
        }
    }
}
