using Framework.Identifiers.Editor;
using Game.Maps.Data;
using UnityEditor;

namespace Game.Maps.Editor
{
    [CustomPropertyDrawer(typeof(SceneId))]
    public class SceneIdPropertyDrawer : IdentifierPropertyDrawer
    {
        public override string IdentifierDatabasePath => "Assets/Data/SceneIdDatabase.asset";
    }
}
