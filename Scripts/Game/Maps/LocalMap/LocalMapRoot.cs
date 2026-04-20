using System.Collections.Generic;
using UnityEngine;

namespace Game.Maps.LocalMap
{
    public class LocalMapRoot : MapRoot
    {
        #region Serialized Fields
        [Header("Game Data")]
        [SerializeField] private LocalSubMap[] _localSubMaps;
        #endregion

        #region MonoBehaviour Methods
        private void Start()
        {
            DeactivateInactiveSubMaps();
        }
        #endregion

        #region MapRoot Implementation
        protected override IReadOnlyCollection<SubMap> SubMaps => _localSubMaps;

        public override void ChangeSubMap(SubMap subMap)
        {
            CurrentSubMap.DeactivateMap();
            base.ChangeSubMap(subMap);
            CurrentSubMap.ActivateMap();
        }
        #endregion

        #region Public Methods
        public void RefreshMapState()
        {
            foreach (LocalSubMap localSubMap in _localSubMaps)
            {
                localSubMap.RefreshSubmapState();
            }
        }
        #endregion

        #region Private Methods
        private void DeactivateInactiveSubMaps()
        {
            foreach (LocalSubMap map in _localSubMaps)
            {
                if (map == CurrentSubMap)
                    continue;

                map.DeactivateMap();
            }
        }
        #endregion

        #region Editor
        private void OnValidate()
        {
            _localSubMaps = GetComponentsInChildren<LocalSubMap>(true);
            transform.position = Vector3.zero;
        }
        #endregion
    }
}
