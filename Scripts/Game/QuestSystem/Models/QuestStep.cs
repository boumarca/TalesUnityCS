using Game.QuestSystem.Data;
using UnityEngine.Localization;

namespace Game.QuestSystem.Models
{
    public abstract class QuestStep
    {
        #region Public Properties
        public QuestId QuestId { get; private set; }
        public bool IsCompleted { get; private set; }
        public QuestStepData Data { get; private set; }
        public LocalizedString Name => Data.Name;
        #endregion

        #region Public Methods
        public void InitializeQuestStep(QuestId questId, QuestStepData data)
        {
            QuestId = questId;
            Data = data;
        }

        public abstract void ProgressStep(object payload);
        #endregion

        #region Protected Methods
        protected void MarkAsCompleted()
        {
            if (IsCompleted)
                return;

            IsCompleted = true;
        }
        #endregion
    }
}
