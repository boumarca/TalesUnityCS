using UnityEngine;
using UnityEngine.Localization;

namespace Game.QuestSystem.Data
{
    public abstract class QuestStepData : ScriptableObject
    {
        #region Serialized Properties
        [field: SerializeField] public LocalizedString Name { get; private set; }
        [field: SerializeField] public LocalizedString StorySummary { get; private set; }
        [field: SerializeField] public bool IsObjective { get; private set; }
        #endregion
    }
}
