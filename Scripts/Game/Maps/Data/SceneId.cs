using System;
using Framework.Identifiers;

namespace Game.Maps.Data
{
    [Serializable]
    public class SceneId : Identifier
    {
        #region Constructors
        public SceneId(string id) : base(id, id) { }
        #endregion
    }
}
