using Framework.Identifiers.Editor;
using Game.QuestSystem.Data;
using UnityEditor;

namespace Game.QuestSystem.Editor
{
    [CustomPropertyDrawer(typeof(QuestTargetId))]
    public class QuestTargetIdPropertyDrawer : IdentifierPropertyDrawer
    {
        public override string IdentifierDatabasePath => "Assets/Data/Quests/QuestTargetIdDatabase.asset";
    }
}
