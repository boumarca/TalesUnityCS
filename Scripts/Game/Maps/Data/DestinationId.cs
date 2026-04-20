using System;
using Framework.Identifiers;

namespace Game.Maps.Data
{
    [Serializable]
    public class DestinationId : Identifier
    {
        #region Static Fields
        public static readonly DestinationId None = new DestinationId(string.Empty);
        #endregion

        #region Constructors
        public DestinationId(string id) : base(id, id) { }
        #endregion
    }
}
