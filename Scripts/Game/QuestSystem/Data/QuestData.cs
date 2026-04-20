using System.Collections.Generic;
using Framework.Assertions;
using UnityEngine;
using UnityEngine.Localization;

namespace Game.QuestSystem.Data
{
    /// <summary>
    /// this class represents the static data for quests.
    /// </summary>
    [CreateAssetMenu(fileName = "NewQuestData", menuName = "Game Data/QuestData")]
    public class QuestData : ScriptableObject
    {
        #region Serialized Fields
        [Header("Game Data")]
        [SerializeField] private QuestId _id;
        [SerializeField] private QuestType _type;
        [SerializeField] private LocalizedString _name;
        [SerializeField] private LocalizedString _description;
        [SerializeField] private LocalizedString _questGiver;
        [SerializeField] private LocalizedString _location;
        [SerializeField] private int _chapter;
        [SerializeField] private QuestPreconditionsList _preconditionList;
        [SerializeField] private List<QuestStepData> _questSteps;
        #endregion

        #region Public Properties
        public QuestId Id => _id;
        public QuestType Type => _type;
        public LocalizedString Name => _name;
        public LocalizedString Description => _description;
        public LocalizedString QuestGiver => _questGiver;
        public LocalizedString Location => _location;
        public int Chapter => _chapter;
        public int StepCount => _questSteps.Count;
        public QuestPreconditionsList Preconditions => _preconditionList;
        #endregion

        #region Public Methods
        public QuestStepData GetQuestStepData(int stepIndex)
        {
            AssertWrapper.IsIndexInRange(stepIndex, _questSteps, $"There is no QuestStepData for index {stepIndex}. {stepIndex} is out of range.");
            return _questSteps[stepIndex];
        }

        public int GetStepIndex(QuestStepData questStep)
        {
            int stepIndex = _questSteps.IndexOf(questStep);
            AssertWrapper.IsTrue(stepIndex >= 0, $"Could not find {nameof(questStep)} in {nameof(_questSteps)}. Verify that the step exists.");
            return stepIndex;
        }
        #endregion
    }
}
