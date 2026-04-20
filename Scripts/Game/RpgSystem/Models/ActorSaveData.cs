using System;
using System.Collections.Generic;

namespace Game.RpgSystem.Models
{
    [Serializable]
    public class ActorSaveData
    {
        public string ActorId { get; set; }
        public int Level { get; set; }
        public int TotalExperience { get; set; }
        public Dictionary<string, string> Stats { get; set; }
        public List<EquipmentSaveData> EquippedItems { get; set; }
    }
}
