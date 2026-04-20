using System.Collections.Generic;
using UnityEngine;

namespace Framework.Identifiers
{
    [CreateAssetMenu(fileName = "IdentifierDatabase", menuName = "Databases/IdentifierDatabase")]
    public class IdentifierDatabase : ScriptableObject
    {
        #region Serialized Fields
        [SerializeField] private IdentifierData[] _allIdentifiers;
        #endregion

        #region Public Properties
        public IReadOnlyCollection<IdentifierData> Identifiers => _allIdentifiers;
        #endregion
    }
}
