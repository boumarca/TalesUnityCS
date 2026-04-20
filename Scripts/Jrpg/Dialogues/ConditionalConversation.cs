using System;
using System.Linq;
using Game.DialogueSystem.Data;
using Game.QuestSystem;
using Game.QuestSystem.Data;
using UnityEngine;

namespace Jrpg.Dialogues
{
    [Serializable]
    public class ConditionalConversation
    {
        #region Serialized Fields
        [SerializeField] private QuestPreconditionsList _preconditionsList;
        [SerializeField] private ConversationSequence _conversationSequence;
        #endregion

        #region Public Properties
        public ConversationSequence Sequence => _conversationSequence;
        #endregion

        #region Public Methods
        public bool ArePreconditionsMet()
        {
            return _preconditionsList.All(precondition => QuestManager.Instance.IsPreconditionMet(precondition));
        }
        #endregion
    }
}
