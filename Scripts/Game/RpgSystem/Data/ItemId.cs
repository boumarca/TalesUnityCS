using System;
using Framework.Identifiers;

namespace Game.RpgSystem.Data
{
    [Serializable]
    public class ItemId : Identifier
    {
        public ItemId(string id) : base(id, string.Empty) { }

        public static explicit operator ItemId(string id) => FromString(id);

        public static ItemId FromString(string id) => new(id);
    }
}
