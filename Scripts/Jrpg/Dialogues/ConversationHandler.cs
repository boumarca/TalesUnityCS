using System;
using Game.DialogueSystem;
using Game.DialogueSystem.Data;
using UnityEngine;

namespace Jrpg.Dialogues
{
    public class ConversationHandler : DialogueHandler
    {
        #region Serialized Fields
        [Header("Game Data")]
        [Tooltip("Dialogues are in priority order. ")]
        [SerializeField] private ConditionalConversation[] _conditionalConversations;
        [SerializeField] private ConversationSequence _defaultConversation;
        #endregion

        #region Events
        public event EventHandler<DialogueEventArgs> OnDialogueEndedEvent = delegate { };
        #endregion

        #region Override Methods

        protected override IDialogueSequence GetDialogueSequence()
        {
            foreach (ConditionalConversation convo in _conditionalConversations)
            {
                if (!convo.ArePreconditionsMet())
                    continue;

                return convo.Sequence;
            }
            return _defaultConversation;
        }
        #endregion
    }
}
