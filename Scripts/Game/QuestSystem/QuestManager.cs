using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Assertions;
using Framework.Common;
using Framework.Singleton;
using Game.QuestSystem.Data;
using Game.QuestSystem.Models;
using Game.SaveSystem;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.QuestSystem
{
    /// <summary>
    /// This manager handles Quests and change their states.
    /// </summary>
    public class QuestManager : GlobalSingleton<QuestManager>, ISaveable
    {
        #region Serialized Fields
        [SerializeField] private QuestDatabase _questDatabase;
        #endregion

        #region Private Fields
        private readonly Dictionary<QuestId, Quest> _allQuestsDict = new();
        private readonly List<Quest> _activeQuests = new();
        private readonly List<Quest> _inactiveQuests = new();
        private readonly List<Quest> _completedQuests = new();

        private Quest _currentlyFollowedQuest;
        #endregion

        #region Public Properties
        public int CurrentChapter => _activeQuests.FirstOrDefault(quest => quest.Type == QuestType.Main)?.Chapter ?? 0;
        public IReadOnlyList<Quest> ActiveQuests => _activeQuests;
        public IReadOnlyList<Quest> InactiveQuests => _inactiveQuests;
        public IReadOnlyList<Quest> CompletedQuests => _completedQuests;
        #endregion

        #region Events
        public event EventHandler<QuestEventArgs> OnQuestStateChangedEvent = delegate { };
        public event EventHandler<QuestEventArgs> OnQuestStepUpdatedEvent = delegate { };
        #endregion

        #region ISaveable Implementation
        public IEnumerable<Type> SaveDataTypes => new[] { typeof(QuestListSaveData), typeof(QuestSaveData) };

        public bool TryLoadData(SaveDataBase saveData)
        {
            if(saveData is not QuestListSaveData questListSaveData)
                return false;
                        
            InitializeAllQuestsAsDefault();
            foreach (QuestSaveData serializedQuest in questListSaveData.QuestList)
            {
                QuestId id = new(serializedQuest.QuestId);
                Quest quest = GetQuest(id);
                quest.LoadData(serializedQuest);
                MarkQuest(quest);
            }
            return true;
        }

        public SaveDataBase SaveData()
        {
            QuestListSaveData questListSaveData = new();
            foreach(Quest quest in _allQuestsDict.Values)
            {
                if (quest.State == QuestState.RequirementsNotMet)
                    continue;
                questListSaveData.QuestList.Add(quest.SaveData());
            }
            return questListSaveData;
        }
        #endregion

        #region Public Methods
        public void InitializeAsNew()
        {
            InitializeAllQuestsAsDefault();
            RefreshInactiveQuestsRequirement();
        }

        public void InitializeAllQuestsAsDefault()
        {
            ResetQuestStatus();
            foreach (QuestData questData in _questDatabase)
            {
                AssertWrapper.IsFalse(_allQuestsDict.ContainsKey(questData.Id), $"Duplicate ID found: {questData.Id}");
                Quest quest = new(questData);
                _allQuestsDict.Add(questData.Id, quest);
                _inactiveQuests.Add(quest);
            }
        }

        public Quest GetQuest(QuestId id)
        {
            Quest quest = _allQuestsDict[id];
            AssertWrapper.IsNotNull(quest, $"{id} not found in {nameof(_allQuestsDict)}");
            return quest;
        }

        public void BroadcastQuestsStatus()
        {
            foreach (Quest quest in _allQuestsDict.Values)
                OnQuestStateChangedEvent(this, new QuestEventArgs(quest));
        }

        public void RefreshInactiveQuestsRequirement()
        {
            for (int i = _inactiveQuests.Count - 1; i >= 0; i--)
            {
                Quest quest = _inactiveQuests[i];
                if (quest.State == QuestState.RequirementsNotMet && AreRequirementsMet(quest))
                    ChangeQuestState(quest, QuestState.CanStart);
            }
        }

        public void StartQuest(QuestId id)
        {
            Quest quest = GetQuest(id);
            MarkQuestAsActive(quest);
            ChangeQuestState(quest, QuestState.InProgress);
        }

        public void AdvanceQuest(Quest quest)
        {
            quest.MoveToNextStep();
            OnQuestStepUpdatedEvent(this, new QuestEventArgs(quest));
            if (!quest.HasMoreStep())
                ChangeQuestState(quest, QuestState.CanComplete);
        }

        public void CompleteQuest(QuestId id)
        {
            Quest quest = GetQuest(id);
            MarkQuestAsCompleted(quest);
            ChangeQuestState(quest, QuestState.Completed);
            RefreshInactiveQuestsRequirement();
        }

        public void NotifyQuestSteps(object payload)
        {
            List<Quest> finishedQuests = ListPool<Quest>.Get();
            foreach (Quest quest in _activeQuests)
            {
                quest.ProgressQuestStep(payload);
                if (quest.IsCurrentStepCompleted)
                    finishedQuests.Add(quest);
            }

            foreach (Quest quest in finishedQuests)
                AdvanceQuest(quest);

            ListPool<Quest>.Release(finishedQuests);
        }

        public bool IsPreconditionMet(QuestPrecondition precondition)
        {
            AssertWrapper.IsNotNull(precondition, "A precondition should never be null. Check in quest data if the list is correctly setup.");

            if (precondition.RequiredQuest == null)
                return false;

            Quest requiredQuest = GetQuest(precondition.RequiredQuest.Id);
            if (requiredQuest.State != precondition.RequiredQuestState)
                return false;

            if (precondition.RequiredQuestState != QuestState.InProgress || precondition.RequiredStep == null)
                return true;

            int requiredQuestStepIndex = requiredQuest.GetStepIndex(precondition.RequiredStep);

            return ComparisonExtensions.CompareValues(requiredQuest.CurrentStepIndex, requiredQuestStepIndex, precondition.StepComparison);
        }

        public void FollowQuest(Quest quest)
        {
            _currentlyFollowedQuest = quest;
        }

        public void UnfollowQuest()
        {
            _currentlyFollowedQuest = null;
        }

        public bool IsFollowedQuest(Quest quest)
        {
            return quest == _currentlyFollowedQuest;
        }
        #endregion

        #region Private Methods
        private void ResetQuestStatus()
        {
            _allQuestsDict.Clear();
            _activeQuests.Clear();
            _completedQuests.Clear();
            _inactiveQuests.Clear();            
        }

        private bool AreRequirementsMet(Quest quest)
        {
            return quest.Preconditions.All(IsPreconditionMet);
        }

        private void ChangeQuestState(Quest quest, QuestState state)
        {
            if (quest.State == state)
                return;

            quest.ChangeState(state);
            OnQuestStateChangedEvent(this, new QuestEventArgs(quest));
        }

        private void MarkQuest(Quest quest)
        {
            switch (quest.State)
            {
                case QuestState.RequirementsNotMet:
                case QuestState.CanStart:
                    if(!_inactiveQuests.Contains(quest))
                        _inactiveQuests.Add(quest);
                    break;
                case QuestState.InProgress:
                case QuestState.CanComplete:
                    MarkQuestAsActive(quest);
                    break;
                case QuestState.Completed:
                    MarkLoadedQuestAsCompleted(quest);
                    break;
            }
        }

        private void MarkQuestAsActive(Quest quest)
        {
            _inactiveQuests.Remove(quest);
            _activeQuests.Add(quest);
        }

        private void MarkQuestAsCompleted(Quest quest)
        {
            _activeQuests.Remove(quest);
            _completedQuests.Add(quest);
            if (_currentlyFollowedQuest == quest)
                _currentlyFollowedQuest = null;
        }

        private void MarkLoadedQuestAsCompleted(Quest quest)
        {
            _inactiveQuests.Remove(quest);
            _completedQuests.Add(quest);
        }
        #endregion

        #region Debug
        public void CheatForceQuestToState(QuestId questId, QuestState state)
        {
            Quest quest = GetQuest(questId);
            quest.ChangeState(state);
            _inactiveQuests.Remove(quest);
            _activeQuests.Remove(quest);
            _completedQuests.Remove(quest);
            MarkQuest(quest);
            if (quest.State != QuestState.RequirementsNotMet)
                CheatCompleteDependencies();

            RefreshInactiveQuestsRequirement();
            BroadcastQuestsStatus();
            return;

            void CheatCompleteDependencies()
            {
                foreach(QuestPrecondition condition in quest.Preconditions)
                {
                    QuestData requiredQuest = condition.RequiredQuest;
                    if (requiredQuest == null)
                        continue;

                    CheatForceQuestStep(requiredQuest.Id, requiredQuest.StepCount);
                    CheatForceQuestToState(requiredQuest.Id, QuestState.Completed);
                }
            }
        }

        public void CheatForceQuestStep(QuestId questId, int stepIndex)
        {
            if (stepIndex < 0)
                return;

            Quest quest = GetQuest(questId);
            quest.CheatForceQuestStep(stepIndex);
            BroadcastQuestsStatus();
        }
        #endregion
    }
}
