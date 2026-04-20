using System.Collections.Generic;
using Framework.Identifiers;
using Game.QuestSystem;
using Game.QuestSystem.Data;
using Game.QuestSystem.Models;
using UnityEngine;

namespace Jrpg.Debug
{
    public class QuestDebugMenu : MonoBehaviour
    {
        [SerializeField] private QuestId _questId;
        [SerializeField] private QuestState _state;
        [SerializeField] private int _stepIndex;

        public IReadOnlyList<Quest> ActiveQuests => QuestManager.Instance.ActiveQuests;
        public IReadOnlyList<Quest> InactiveQuests => QuestManager.Instance.InactiveQuests;
        public IReadOnlyList<Quest> CompletedQuests => QuestManager.Instance.CompletedQuests;

        public bool IsSetQuestCheatValid() => Identifier.IsValid(_questId) && _stepIndex >= 0;

        public void CheatSetQuestToState()
        {
            if (Identifier.IsNullOrEmpty(_questId))
            {
                UnityEngine.Debug.LogError($"{nameof(_questId)} is empty");
                return;
            }

            QuestManager questManager = QuestManager.Instance;

            questManager.CheatForceQuestToState(_questId, _state);
            if(_state == QuestState.InProgress)
                questManager.CheatForceQuestStep(_questId, _stepIndex);
        }
    }
}
