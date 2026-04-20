using System;
using System.Collections.Generic;
using Game.SaveSystem;

namespace Game.QuestSystem.Models
{
    [Serializable]
    public class QuestListSaveData : SaveDataBase
    {
        #region Public Properties
        public ICollection<QuestSaveData> QuestList { get; set; } = new List<QuestSaveData>();
        #endregion
    }
}
