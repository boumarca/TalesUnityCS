using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Maps.LocalMap
{
    public class LocalSubMap : SubMap
    {
        #region Serialized Fields
        [FormerlySerializedAs("_conditionalObjects")][SerializeField] private MapObject[] _mapObjects;
        #endregion

        #region Public Methods
        public void RefreshSubmapState()
        {
            Debug.Log($"Refreshed Submap {name}");
            RefreshMapObjects();
        }
        #endregion

        #region Private Methods
        private void RefreshMapObjects()
        {
            foreach (MapObject conditionalObject in _mapObjects)
            {
                conditionalObject.Refresh();
            }
        }
        #endregion

        #region Editor
        protected override void OnValidate()
        {
            base.OnValidate();
            _mapObjects = GetComponentsInChildren<MapObject>(true);
        }
        #endregion
    }
}
