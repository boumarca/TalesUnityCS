using System.Collections.Generic;
using SerializedTuples;
using SerializedTuples.Runtime;
using UnityEngine;

namespace Game.RpgSystem.Data
{
    [CreateAssetMenu(fileName = "ElementIconMap", menuName = "Scriptable Objects/ElementIconMap")]
    public class ElementIconMap : ScriptableObject
    {
        #region Serialized Fields
        [SerializedTupleLabels("Element", "Icon")]
        [SerializeField] private SerializedTuple<RpgElements, Sprite>[] _elementsIcons;
        #endregion

        #region Private Fields
        private Dictionary<RpgElements, Sprite> _mapping;
        #endregion

        #region Public Methods
        public Sprite GetSprite(RpgElements element)
        {
            if (_mapping == null)
                _mapping = _elementsIcons.ToDictionary();
            return _mapping[element];
        }
        #endregion
    }
}
