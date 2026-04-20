using System;
using Game.RpgSystem.Data;

namespace Game.RpgSystem.Models
{
    [Serializable]
    public class ItemEffectSaveData
    {
        #region Public Properties
        public EffectType EffectType { get; set; }
        public int Level { get; set; }
        #endregion
    }
}
