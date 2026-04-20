using System;
using System.Collections.Generic;
using Game.SaveSystem;

namespace Game.RpgSystem.Models
{
    [Serializable]
    public class PartySaveData : SaveDataBase
    {
        public ICollection<string> MemberIds { get; } = new List<string>();
    }
}
