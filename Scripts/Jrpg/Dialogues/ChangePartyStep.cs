using System;
using Game.DialogueSystem.Data;
using Game.RpgSystem;
using Game.RpgSystem.Data;
using Game.UI;
using UnityEngine;

namespace Jrpg.Dialogues
{
    [Serializable]
    public class ChangePartyStep : IDialogueStep
    {
        [Serializable]
        private enum ChangePartyOperation
        {
            Add,
            Remove
        }

        #region Serialized Fields
        [SerializeField] private RpgActorData _actor;
        [SerializeField] private ChangePartyOperation _operation;
        #endregion

        # region IDialogueStep Implementation
        public event EventHandler<DialogueEventArgs> OnStepCompleted;
        public void Execute()
        {
            string joinMessage = string.Empty;
            if (_operation == ChangePartyOperation.Add)
            {
                PartyManager.Instance.AddPartyMember(_actor.Id);
                joinMessage = $"{_actor.Name.GetLocalizedString()} joint le party!";
            }
            else if (_operation == ChangePartyOperation.Remove)
            {
                PartyManager.Instance.RemovePartyMember(_actor.Id);
                joinMessage = $"{_actor.Name.GetLocalizedString()} quitte le party!";
            }

            //TODO: Redo the player message
            UIManager.Instance.OnTextboxClosedEvent += HandleOnTextboxClosed;
            UIManager.Instance.DisplayTextbox(joinMessage, null);
        }
        #endregion

        #region Private Methods
        private void HandleOnTextboxClosed(object sender, EventArgs eventArgs)
        {
            UIManager.Instance.OnTextboxClosedEvent -= HandleOnTextboxClosed;
            CompleteStep();
        }

        private void CompleteStep()
        {
            OnStepCompleted(this, DialogueEventArgs.Empty);
        }
        #endregion
    }
}
