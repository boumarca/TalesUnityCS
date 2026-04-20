using System;

namespace Game.RpgSystem.Models
{
    [Serializable]
    public class InventoryItemSaveData
    {
        #region Public Properties
        public RpgItemSaveData ItemSaveData { get; set; }
        public int Quantity { get; set; }
        #endregion
    }
}
