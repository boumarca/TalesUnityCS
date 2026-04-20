using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.SaveSystem
{
    public class SaveMetadataProvider : MonoBehaviour
    {
        #region Public Properties
        public virtual IEnumerable<Type> MetadataTypes => Enumerable.Empty<Type>();
        #endregion

        #region Virtual Methods
        public virtual SaveMetadataBase CreateMetadata(int slotId)
        {
            return new SaveMetadataBase(slotId);
        }
        #endregion
    }
}
