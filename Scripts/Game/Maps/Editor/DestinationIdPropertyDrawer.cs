using Framework.Identifiers.Editor;
using Game.Maps.Data;
using UnityEditor;

namespace Game.Maps.Editor
{
    [CustomPropertyDrawer(typeof(DestinationId))]
    public class DestinationIdPropertyDrawer : IdentifierPropertyDrawer
    {
        public override string IdentifierDatabasePath => "Assets/Data/DestinationIdDatabase.asset";
    }
}
