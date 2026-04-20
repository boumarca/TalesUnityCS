using Framework.Identifiers.Editor;
using Game.QuestSystem.Data;
using UnityEditor;

namespace Game.QuestSystem.Editor
{
    [CustomPropertyDrawer(typeof(QuestId))]
    public class QuestIdPropertyDrawer : IdentifierPropertyDrawer
    {
        public override string IdentifierDatabasePath => "Assets/Data/Quests/QuestIdDatabase.asset";
    }
}
