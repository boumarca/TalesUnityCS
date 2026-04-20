using System;
using UnityEngine;

namespace Game.RpgSystem.Models
{
    public class LevelInfo
    {
        #region Private Fields
        private int _maxLevel;
        private Func<int, int> _experienceFormula;
        #endregion

        #region Public Properties
        public int Level { get; private set; } = 1;
        public int TotalExperience { get; private set; }
        #endregion

        #region Events
        public event Action OnLevelUpEvent = delegate { };
        #endregion

        #region Constructor
        public LevelInfo(int maxLevel, Func<int, int> formula)
        {
            _maxLevel = maxLevel;
            _experienceFormula = formula;
        }
        #endregion

        #region Public Methods
        public void GainExperience(int amount)
        {
            TotalExperience += amount;
            while (CanLevelUp())
                LevelUp();
        }

        public int ExperienceToNextLevel()
        {
            return TotalExperienceToNextLevel() - ComputeExperienceForLevel(Level);
        }

        public int TotalExperienceToNextLevel()
        {
            return ComputeExperienceForLevel(NextLevel());
        }

        public int RemainingExperienceToNextLevel()
        {
            return TotalExperienceToNextLevel() - TotalExperience;
        }

        public float GetProgressPercentToNextLevel()
        {
            return 1.0f - (float)RemainingExperienceToNextLevel() / ExperienceToNextLevel();
        }

        public void LoadData(int level, int experience)
        {
            Level = level;
            TotalExperience = experience;
        }
        #endregion

        #region Private Methods
        private int ComputeExperienceForLevel(int level)
        {
            if (level == 1)
                return 0;

            return _experienceFormula(level);
        }

        private int NextLevel()
        {
            return Mathf.Min(Level + 1, _maxLevel);
        }

        private bool CanLevelUp()
        {
            return TotalExperience >= TotalExperienceToNextLevel();
        }

        private void LevelUp()
        {
            Level = NextLevel();
            OnLevelUpEvent();
        }
        #endregion
    }
}
