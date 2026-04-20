using System;
using UnityEngine;
using UnityEngine.Events;

namespace Game.DialogueSystem.Data
{
    [Serializable]
    public class UnityEventStep : IDialogueStep
    {
        #region Serialized Fields
        [SerializeField] private UnityEvent _onStepEvents;
        #endregion

        # region IDialogueStep Implementation
        public event EventHandler<DialogueEventArgs> OnStepCompleted = delegate { };

        public void Execute()
        {
            _onStepEvents.Invoke();
            CompleteStep();
        }
        #endregion

        #region Private Methods
        private void CompleteStep()
        {
            OnStepCompleted(this, DialogueEventArgs.Empty);
        }
        #endregion
    }
}
