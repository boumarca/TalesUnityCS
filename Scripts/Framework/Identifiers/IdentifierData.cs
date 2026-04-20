using System;
using UnityEngine;

namespace Framework.Identifiers
{
    [Serializable]
    public class IdentifierData
    {
        #region Serialized Properties
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        #endregion

        #region Constructors
        public IdentifierData(string id, string name)
        {
            Id = id;
            Name = name;
        }
        #endregion
    }
}
