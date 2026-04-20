using System;

namespace Game.RpgSystem.Models
{
    [Serializable]
    public class EquipmentSaveData
    {
        #region Public Properties
        public EquipSlot Slot { get; set; }
        public RpgItemSaveData Item { get; set; }
        #endregion
    }
}
