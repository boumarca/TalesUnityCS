using System;

namespace Game.DialogueSystem.Data
{
    public interface IDialogueStep
    {
        public event EventHandler<DialogueEventArgs> OnStepCompleted;
        public void Execute();
    }
}
