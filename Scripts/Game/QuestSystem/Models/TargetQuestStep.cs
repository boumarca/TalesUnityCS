using Game.QuestSystem.Data;

namespace Game.QuestSystem.Models
{
    public class TargetQuestStep : QuestStep
    {
        #region Public Methods
        public override void ProgressStep(object payload)
        {
            if (payload is not QuestTarget questTarget)
                return;

            if (Data is TargetQuestStepData targetStepData && questTarget.TargetId == targetStepData.TargetId)
                MarkAsCompleted();
        }
        #endregion
    }
}
