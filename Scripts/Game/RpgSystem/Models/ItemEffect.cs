using Game.RpgSystem.Data;
using UnityEngine;

namespace Game.RpgSystem.Models
{
    public class ItemEffect
    {
        #region Private Fields
        private ItemEffectData _data;
        #endregion

        #region Public Properties
        public EffectType EffectType => _data.EffectType;
        public int MaxLevel => _data.MaxLevel;
        public int EffectValue => _data.GetEffectValue(Level);
        public string DisplayName => _data.DisplayName.GetLocalizedString(Level);
        public string DisplayDescription => _data.Description.GetLocalizedString(EffectValue);
        public int Level { get; private set; }
        #endregion

        #region Constructors
        public ItemEffect(ItemEffectData data, int level)
        {
            _data = data;
            //TODO: Remove later when all saves are migrated
            if (level == 0)
                level = 1;
            Level = level;
        }
        #endregion

        #region Public Methods
        public ItemEffect TransferEffect()
        {
            return new ItemEffect(_data, Level);
        }

        public void LevelUp()
        {
            Level = Mathf.Min(Level + 1, _data.MaxLevel);
        }

        public void LevelDown()
        {
            Level = Mathf.Max(Level - 1, 1);
        }

        public bool IsLevelMax()
        {
            return Level == _data.MaxLevel;
        }

        public bool IsCompatible(ItemType itemType)
        {
            return _data.HasCompatibleType(itemType);
        }

        public bool IsSameEffect(ItemEffect other)
        {
            if(other == null)
                return false;
            return other.EffectType == EffectType && other.Level == Level;
        }

        public ItemEffectSaveData SaveData()
        {
            return new ItemEffectSaveData()
            {
                EffectType = EffectType,
                Level = Level
            };
        }
        #endregion
    }
}
