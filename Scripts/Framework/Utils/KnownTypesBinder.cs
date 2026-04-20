using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Serialization;

namespace Framework.Utils
{
    public class KnownTypesBinder : ISerializationBinder
    {
        #region Public Properties
        public IList<Type> KnownTypes { get; private set; }
        #endregion

        #region Constructor
        public KnownTypesBinder(IList<Type> knownTypes)
        {
            KnownTypes = knownTypes;
        }
        #endregion

        #region ISerializationBinder Implementation
        public Type BindToType(string assemblyName, string typeName)
        {
            return KnownTypes.SingleOrDefault(t => t.Name == typeName);
        }

        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = serializedType?.Name;
        }
        #endregion
    }
}
