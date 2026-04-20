using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Inputs.Data
{
    [CreateAssetMenu(fileName = "NewBindingsIconMapping", menuName = "Databases/BindingsIconMapping")]
    public class BindingsIconMapping : ScriptableObject
    {
        #region Serialized Fields
        [SerializeField] private List<ControlSchemeBindingsData> _schemesBindingList;
        #endregion

        #region Private Fields
        private Dictionary<string, IReadOnlyDictionary<string, Sprite>> _mapping;
        #endregion

        #region Public Properties
        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, Sprite>> BindingMapping
        {
            get
            {
                if (_mapping == null)
                    PopulateMapping();
                return _mapping;
            }
        }
        #endregion

        #region Private Methods
        private void PopulateMapping()
        {
            _mapping = new Dictionary<string, IReadOnlyDictionary<string, Sprite>>();
            foreach(ControlSchemeBindingsData controlScheme in _schemesBindingList)
                _mapping.Add(controlScheme.ControlScheme, controlScheme.Bindings);
        }
        #endregion

        #region Nested Classes
        [Serializable]
        private class ControlSchemeBindingsData
        {
            #region Serialized Fields
            [SerializeField] private string _controlScheme;
            [SerializeField] private List<BindingIconMap> _bindings;
            #endregion

            #region Private Fields
            private Dictionary<string, Sprite> _bindingMap;
            #endregion

            #region Public Properties
            public string ControlScheme => _controlScheme;
            public IReadOnlyDictionary<string, Sprite> Bindings
            {
                get
                {
                    if (_bindingMap == null)
                        PopulateBindingMap();
                    return _bindingMap;
                }
            }
            #endregion

            #region Private Methods
            private void PopulateBindingMap()
            {
                _bindingMap = new Dictionary<string, Sprite>();
                foreach (BindingIconMap binding in _bindings)
                    _bindingMap.Add(binding.BindingPath, binding.Icon);
            }
            #endregion
        }

        [Serializable]
        private class BindingIconMap
        {
            [field: SerializeField] public string BindingPath { get; private set; }
            [field: SerializeField] public Sprite Icon { get; private set; }
        }
        #endregion
    }
}
