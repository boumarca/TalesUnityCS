using System;
using Framework.Common;
using UnityEngine;

namespace Game.QuestSystem.Data
{
    [Serializable]
    public class QuestPrecondition
    {
        #region Serialized Properties
        [field: SerializeField] public QuestData RequiredQuest { get; private set; }
        [field: SerializeField] public QuestState RequiredQuestState { get; private set; }
        [field: SerializeField] public Comparison StepComparison { get; private set; }
        [field: SerializeField] public QuestStepData RequiredStep { get; private set; }
        #endregion

    }
}
