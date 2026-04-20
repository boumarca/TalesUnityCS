namespace Game.DialogueSystem.Data
{
    public interface IDialogueSequence
    {
        public int StepCount { get; }
        public IDialogueStep GetStep(int index);
    }
}
