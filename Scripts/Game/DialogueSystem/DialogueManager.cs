using System;
using Framework.Singleton;
using Game.DialogueSystem.Data;

namespace Game.DialogueSystem
{
    public class DialogueManager : SceneSingleton<DialogueManager>
    {
        #region Private Fields
        private IDialogueSequence _currentDialogueSequence;
        private int _nextDialogStep;
        #endregion

        #region Events
        public event EventHandler<DialogueEventArgs> OnDialogueStartedEvent = delegate { };
        public event EventHandler<DialogueEventArgs> OnDialogueEndedEvent = delegate { };
        #endregion

        #region Public Methods
        public void StartDialogue(IDialogueSequence dialogueSequence)
        {
            if (dialogueSequence == null || dialogueSequence.StepCount == 0)
            {
                OnDialogueEndedEvent(this, DialogueEventArgs.Empty);
                return;
            }

            OnDialogueStartedEvent(this, DialogueEventArgs.Empty);
            _currentDialogueSequence = dialogueSequence;
            _nextDialogStep = 0;
            AdvanceSequence();
        }
        #endregion

        #region Private Methods
        private void AdvanceSequence()
        {
            if (_nextDialogStep >= _currentDialogueSequence.StepCount)
            {
                CompleteDialogue();
                return;
            }

            IDialogueStep dialogueStep = _currentDialogueSequence.GetStep(_nextDialogStep);
            _nextDialogStep++;

            dialogueStep.OnStepCompleted += HandleOnStepCompleted;
            dialogueStep.Execute();
        }

        private void HandleOnStepCompleted(object sender, DialogueEventArgs eventArgs)
        {
            if (sender is IDialogueStep step)
                step.OnStepCompleted -= HandleOnStepCompleted;

            AdvanceSequence();
        }

        private void CompleteDialogue()
        {
            OnDialogueEndedEvent(this, DialogueEventArgs.Empty);
        }
        #endregion
    }
}
