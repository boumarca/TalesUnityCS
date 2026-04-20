using System;
using Game.QuestSystem.Data;

namespace Game.QuestSystem.Models
{
    [Serializable]
    public class QuestSaveData
    {
        #region Public Properties
        public string QuestId { get; set; }
        public QuestState QuestState { get; set; }
        public int CurrentStepIndex { get; set; }
        public int CurrentStepState { get; set; }
        #endregion
    }
}
