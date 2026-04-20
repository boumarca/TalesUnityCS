using Framework.Identifiers.Editor;
using Game.RpgSystem.Data;
using UnityEditor;

namespace Game.QuestSystem.Editor
{
    [CustomPropertyDrawer(typeof(ActorId))]
    public class ActorIdPropertyDrawer : IdentifierPropertyDrawer
    {
        public override string IdentifierDatabasePath => "Assets/Data/Actors/ActorIdDatabase.asset";
    }
}
