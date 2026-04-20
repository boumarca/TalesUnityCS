using Framework.Identifiers.Editor;
using Game.RpgSystem.Data;
using UnityEditor;

namespace Game.QuestSystem.Editor
{
    [CustomPropertyDrawer(typeof(ItemId))]
    public class ItemIdPropertyDrawer : IdentifierPropertyDrawer
    {
        public override string IdentifierDatabasePath => "Assets/Data/Items/ItemIdDatabase.asset";
    }
}
