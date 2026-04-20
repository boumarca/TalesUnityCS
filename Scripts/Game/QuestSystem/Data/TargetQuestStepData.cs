using UnityEngine;

namespace Game.QuestSystem.Data
{
    [CreateAssetMenu(fileName = "NewTargetQuestStepData", menuName = "Game Data/Quests/TargetQuestStepData")]
    public class TargetQuestStepData : QuestStepData
    {
        #region Serialized Properties
        [field: SerializeField] public QuestTargetId TargetId { get; private set; }
        #endregion
    }
}
