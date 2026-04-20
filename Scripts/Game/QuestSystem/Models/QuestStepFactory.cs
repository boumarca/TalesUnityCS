using Game.QuestSystem.Data;

namespace Game.QuestSystem.Models
{
    public static class QuestStepFactory
    {
        public static QuestStep MakeQuestStep(QuestStepData questStepData)
        {
            if (questStepData is TargetQuestStepData)
                return new TargetQuestStep();
            return null;
        }
    }
}
