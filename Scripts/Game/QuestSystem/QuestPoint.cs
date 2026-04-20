using Game.QuestSystem.Data;
using Game.QuestSystem.Models;
using UnityEngine;

namespace Game.QuestSystem
{
    /// <summary>
    /// This class represents an object where the player can start or finish a quest.
    /// </summary>
    public class QuestPoint : MonoBehaviour
    {
        #region Serialized Fields
        [Header("GameData")]
        [SerializeField] private QuestData _questData;
        #endregion

        #region Protected Properties
        protected QuestId Id => _questData.Id;
        protected QuestState CurrentQuestState { get; private set; }
        #endregion

        #region MonoBehaviour Methods
        private void OnEnable()
        {
            QuestManager.Instance.OnQuestStateChangedEvent += HandleQuestStateChanged;
        }

        private void OnDisable()
        {
            if(QuestManager.Instance != null)
                QuestManager.Instance.OnQuestStateChangedEvent -= HandleQuestStateChanged;
        }
        #endregion

        #region Public Methods
        public void RefreshQuestState()
        {
            StartQuest();
            CompleteQuest();
        }

        public void StartQuest()
        {
            if (CurrentQuestState == QuestState.CanStart)
                QuestManager.Instance.StartQuest(Id);
        }

        public void CompleteQuest()
        {
            if(CurrentQuestState == QuestState.CanComplete)
                QuestManager.Instance.CompleteQuest(Id);
        }
        #endregion

        #region Protected Methods
        protected virtual void HandleQuestStateChanged(object sender, QuestEventArgs eventArgs)
        {
            Quest quest = eventArgs.Quest;
            if (quest.Id != Id)
                return;

            CurrentQuestState = quest.State;
            Debug.Log($"Quest with id: {Id} updated to state {CurrentQuestState}.");
        }
        #endregion
    }
}
