using Game.CraftingSystem.Data;

namespace Game.CraftingSystem.Models
{
    public class CraftingSkillResult
    {
        #region Private Fields
        private int _resultValue;
        #endregion

        #region
        public string ResultMessage => Skill.ProcMessage.GetLocalizedString(_resultValue);
        public CraftingSkillData Skill { get; private set; }
        #endregion

        #region Constructor
        public CraftingSkillResult(CraftingSkillData skill)
        {
            Skill = skill;
        }
        #endregion

        #region Public Methods
        public void AddResultValue(int value)
        {
            _resultValue += value;
        }
        #endregion
    }
}
