using System;

namespace Game.QuestSystem.Models
{
    public class QuestEventArgs : EventArgs
    {
        #region Public Properties
        public Quest Quest { get; private set; }
        #endregion

        #region Constructor
        public QuestEventArgs(Quest quest)
        {
            Quest = quest;
        }
        #endregion
    }
}
