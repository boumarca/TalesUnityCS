using Framework.Assertions;
using Game.QuestSystem.Data;
using UnityEngine;
using UnityEngine.Localization;

namespace Game.QuestSystem.Models
{
    /// <summary>
    /// This class represent a dynamic instance of a quest.
    /// </summary>
    public class Quest
    {
        #region Private Fields
        private readonly QuestData _data;
        #endregion

        #region Public Properties
        public QuestId Id => _data.Id;
        public QuestType Type => _data.Type;
        public LocalizedString Name => _data.Name;
        public LocalizedString Description => _data.Description;
        public LocalizedString QuestGiver => _data.QuestGiver;
        public LocalizedString Location => _data.Location;
        public int Chapter => _data.Chapter;
        public int StepCount => _data.StepCount;
        public QuestPreconditionsList Preconditions => _data.Preconditions;
        public bool IsCurrentStepCompleted => CurrentStep.IsCompleted;
        public bool IsInProgress => State == QuestState.InProgress;
        public bool IsCompleted => State == QuestState.Completed;
        public QuestState State { get; private set; }
        public int CurrentStepIndex { get; private set; }
        public QuestStep CurrentStep { get; private set; }
        #endregion

        #region Constructors
        public Quest(QuestData data)
        {
            _data = data;
        }
        #endregion

        #region Public Methods
        public void ChangeState(QuestState newState)
        {
            if (newState == State)
                return;

            State = newState;

            if (State == QuestState.InProgress)
                InstantiateCurrentQuestStep();
        }

        public void MoveToNextStep()
        {
            CurrentStep = null;
            CurrentStepIndex++;
            if (HasMoreStep())
            {
                InstantiateCurrentQuestStep();
                Debug.Log($"Quest {Id} is at step {CurrentStepIndex}: {CurrentStep.Name}");
            }
            else
            {
                Debug.Log($"Quest {Id} has no more steps at index {CurrentStepIndex}");
            }
        }

        public bool HasMoreStep()
        {
            return CurrentStepIndex < StepCount;
        }

        public void InstantiateCurrentQuestStep()
        {
            QuestStepData questStep = _data.GetQuestStepData(CurrentStepIndex);
            AssertWrapper.IsNotNull(questStep, $"{nameof(QuestStepData)} doesn't exist for index {CurrentStepIndex}");
            CurrentStep = QuestStepFactory.MakeQuestStep(questStep);
            CurrentStep.InitializeQuestStep(Id, questStep);
        }

        public void ProgressQuestStep(object payload)
        {
            CurrentStep.ProgressStep(payload);
        }        

        public int GetStepIndex(QuestStepData step)
        {
            return _data.GetStepIndex(step);
        }

        public QuestStepData GetStepData(int stepIndex)
        {
            return _data.GetQuestStepData(stepIndex);
        }

        public QuestStepData GetNextObjective()
        {
            QuestStepData currentStep = CurrentStep.Data;
            if (currentStep.IsObjective)
                return currentStep;

            for (int i = CurrentStepIndex + 1; i < StepCount; i++)
            {
                QuestStepData newStep = GetStepData(i);
                if (!newStep.IsObjective)
                    continue;

                return newStep;
            }

            return null;
        }

        public QuestSaveData SaveData()
        {
            return new QuestSaveData()
            {
                QuestState = State,
                QuestId = Id.Id,
                CurrentStepIndex = CurrentStepIndex
            };
        }

        public void LoadData(QuestSaveData saveData)
        {
            AssertWrapper.IsNotNull(saveData, $"{nameof(saveData)} should not be null.");

            if (saveData.QuestId != Id.Id)
            {
                Debug.LogWarning($"Attempting to load the wrong quest save data. Expected {_data.Id} but received {saveData.QuestId}");
                return;
            }

            CurrentStepIndex = saveData.CurrentStepIndex;
            ChangeState(saveData.QuestState);
        }
        #endregion

        #region Debug
        public void CheatForceQuestStep(int stepIndex)
        {
            if (stepIndex >= StepCount)
                stepIndex = StepCount;

            CurrentStepIndex = stepIndex;
            if (State == QuestState.InProgress && HasMoreStep())
                InstantiateCurrentQuestStep();
        }
        #endregion
    }
}
