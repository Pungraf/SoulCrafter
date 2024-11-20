using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.Ability_System
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

        private void Awake()
        {
            _strength = 10;
            _dexterity = 10;
            _intelligence = 10;
            _wisdom = 10;
            _charisma = 10;
            _constitution = 10;
            _skillPoints = 10;
        }

        public void GainSkillPoint()
        {
            _skillPoints++;
            OnSkillPointsChnaged?.Invoke();
        }
    }
}
