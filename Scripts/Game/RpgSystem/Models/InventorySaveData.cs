using System;
using System.Collections.Generic;
using Game.SaveSystem;

namespace Game.RpgSystem.Models
{
    [Serializable]
    public class InventorySaveData : SaveDataBase
    {
        public ICollection<InventoryItemSaveData> Inventory { get; } = new List<InventoryItemSaveData>(); 
    }
}
