using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Framework.Identifiers
{
    [Serializable]
    public class Identifier : IComparable<Identifier>
    {
        #region Serialized Field
        [JsonProperty]
        [field: SerializeField] private string _id;
        #endregion

        #region Public Properties
        public string Id => _id;
        [field: SerializeField] public string Name { get; set; } = string.Empty;
        #endregion

        #region Constructors
        public Identifier() : this(string.Empty, string.Empty) { }

        public Identifier(string name) : this(string.Empty, name) { }

        public Identifier(string id, string name)
        {
            _id = id;
            Name = name;
        }
        #endregion

        #region IComparable Implementation
        public int CompareTo(Identifier other)
        {
            if(other == null)
                return 1;

            return string.Compare(Name, other.Name, StringComparison.Ordinal);
        }
        #endregion

        #region Override Methods
        public override bool Equals(object obj) => obj is Identifier asIdentifier && Equals(asIdentifier);

        public override int GetHashCode() => _id.GetHashCode(StringComparison.Ordinal);

        public override string ToString() => !string.IsNullOrWhiteSpace(Name) ? Name : "<Empty>";
        #endregion

        #region Public Methods
        public bool Equals(Identifier identifier) => identifier is not null && _id.Equals(identifier._id, StringComparison.Ordinal);
        #endregion

        #region Static Methods
        public static bool operator ==(Identifier left, Identifier right) => left is not null && left.Equals(right);

        public static bool operator !=(Identifier left, Identifier right) => !(left == right);

        public static bool IsNullOrEmpty(Identifier identifier) => identifier is null || string.IsNullOrEmpty(identifier._id);
        public static bool IsValid(Identifier identifier) => !IsNullOrEmpty(identifier);        
        #endregion
    }
}
