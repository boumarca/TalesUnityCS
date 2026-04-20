using System.Collections.Generic;
using System.Globalization;
using UnityEditor;

namespace Framework.Identifiers.Editor
{
    [CustomPropertyDrawer(typeof(Tag))]
    public class TagPropertyDrawer : IdentifierPropertyDrawer
    {
        public override string IdentifierDatabasePath => string.Empty;
        public override IReadOnlyCollection<IdentifierData> IdentifierList
        {
            get
            {
                List<IdentifierData> identifiers = new();
                string[] tags = UnityEditorInternal.InternalEditorUtility.tags;
                for (int i = 0; i < tags.Length; i++)
                {
                    string id = tags[i];
                    identifiers.Add(new IdentifierData(id, id));
                }
                return identifiers;
            }
        }
    }
}
