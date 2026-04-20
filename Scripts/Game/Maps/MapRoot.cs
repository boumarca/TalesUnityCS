using System;
using System.Collections.Generic;
using Game.Maps.Data;
using UnityEngine;
using UnityEngine.Localization;

namespace Game.Maps
{
    public abstract class MapRoot : MonoBehaviour
    {
        #region Serialized Fields
        [Header("Game Data")]
        [SerializeField] private SubMap _defaultSubMap;
        [SerializeField] private LocalizedString _localizedMapName;
        #endregion

        #region Public Properties
        public SubMap CurrentSubMap { get; private set; }
        public LocalizedString MapName => _localizedMapName;
        #endregion

        #region Abstract Methods
        protected abstract IReadOnlyCollection<SubMap> SubMaps { get; }
        #endregion

        #region Virtual Methods
        protected virtual void InitializeMap() { }
        public virtual void ChangeSubMap(SubMap subMap)
        {
            CurrentSubMap = subMap;
        }
        #endregion

        #region Public Methods
        public void Initialize()
        {
            CurrentSubMap = _defaultSubMap;
            InitializeMap();
        }

        public void RegisterEvent(EventHandler<TeleporterEventArgs> eventHandler)
        {
            foreach (SubMap subMap in SubMaps)
                subMap.RegisterTeleporterEvent(eventHandler);
        }

        public Tuple<SubMap, SpawnPoint> GetSubMapWithSpawnPoint(DestinationId id)
        {
            SpawnPoint spawnPoint = null;
            SubMap subMap = null;

            foreach (SubMap map in SubMaps)
            {
                if (!map.TryGetSpawnPoint(id, out SpawnPoint sp))
                    continue;

                spawnPoint = sp;
                subMap = map;
                break;
            }

            if (spawnPoint != null)
                return new Tuple<SubMap, SpawnPoint>(subMap, spawnPoint);

            Debug.LogWarning($"The SpawnPoint with id {id} doesn't exist in the map {gameObject.name}. Make sure that {id} is valid or that the spawn point is correctly set in the Inspector.");
            return new Tuple<SubMap, SpawnPoint>(_defaultSubMap, _defaultSubMap.GetDefaultSpawnPoint());
        }

        public SubMap GetSubMapByName(string submapName)
        {
            foreach (SubMap subMap in SubMaps)
            {
                if (subMap.name == submapName)
                    return subMap;
            }

            Debug.LogError($"Submap {submapName} doesn't exist in map {gameObject.name}.");
            return _defaultSubMap;
        }
        #endregion
    }
}
