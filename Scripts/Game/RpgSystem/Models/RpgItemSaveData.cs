using System;
using System.Collections.Generic;

namespace Game.RpgSystem.Models
{
    [Serializable]
    public class RpgItemSaveData
    {
        #region Public Properties
        public string ItemId { get; set; }
        public long Timestamp { get; set; }
        public ICollection<ItemEffectSaveData> Effects { get; set; } = new List<ItemEffectSaveData>();
        #endregion
    }
}
