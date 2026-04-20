using System;
using Framework.Identifiers;

namespace Game.RpgSystem.Data
{
    [Serializable]
    public class ActorId : Identifier
    {
        public ActorId(string id) : base(id, string.Empty) { }
    }
}
