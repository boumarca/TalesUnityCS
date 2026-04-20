using Game.QuestSystem.Models;

namespace Game.QuestSystem
{
    /// <summary>
    /// This class represents the Main Quest, which will auto start and complete its quests
    /// </summary>
    public class MainQuestPoint : QuestPoint
    {
        #region Protected Methods
        protected override void HandleQuestStateChanged(object sender, QuestEventArgs eventArgs)
        {
            Quest quest = eventArgs.Quest;
            if (quest.Id != Id)
                return;

            base.HandleQuestStateChanged(sender, eventArgs);
            RefreshQuestState();
        }
        #endregion
    }
}
