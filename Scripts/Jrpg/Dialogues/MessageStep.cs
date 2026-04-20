using System;
using Game.DialogueSystem.Data;
using Game.UI;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Scripting.APIUpdating;

namespace Jrpg.Dialogues
{
    [Serializable]
    [MovedFrom(false, null, "Game.DialogueSystem")]
    public class MessageStep : IDialogueStep
    {
        #region Serialized Fields
        [SerializeField] private string _speaker;
        [SerializeField] private string _message;
        [SerializeField] private LocalizedString _localizedString;
        #endregion

        # region IDialogueStep Implementation
        public event EventHandler<DialogueEventArgs> OnStepCompleted = delegate { };

        public void Execute()
        {
            UIManager.Instance.OnTextboxClosedEvent += HandleOnTextboxClosed;

            string text = _localizedString.IsEmpty ? _message : _localizedString.GetLocalizedString();
            UIManager.Instance.DisplayTextbox(text, _speaker);
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
