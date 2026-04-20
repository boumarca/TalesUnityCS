using System;
using Framework.Identifiers;

namespace Game.QuestSystem.Data
{
    [Serializable]
    public class QuestId : Identifier
    {
        public QuestId(string id) : base(id, string.Empty) { }
    }
}
