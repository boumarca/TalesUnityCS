using System;
using Framework.Assertions;
using Game.Maps.Data;
using SuperTiled2Unity;
using UnityEngine;

namespace Game.Maps
{
    public class SubMap : MonoBehaviour
    {
        #region Serialized Fields
        [Header("Components References")]
        [SerializeField] private SuperMap _tiledSuperMap;

        [Header("Map Data")]
        [SerializeField] private SpawnPoint[] _spawnPoints;
        [SerializeField] private Teleporter[] _teleporters;
        #endregion

        #region Public Properties
        public Vector2 Position => transform.position;
        public int Width => _tiledSuperMap.m_Width;
        public int Height => _tiledSuperMap.m_Height;
        #endregion

        #region Virtual Methods
        public void ActivateMap()
        {
            gameObject.SetActive(true);
        }

        public void DeactivateMap()
        {
            gameObject.SetActive(false);
        }
        #endregion

        #region Public Methods
        public void RegisterTeleporterEvent(EventHandler<TeleporterEventArgs> teleportEvent)
        {
            foreach (Teleporter teleporter in _teleporters)
            {
                teleporter.OnTeleportEvent += teleportEvent;
            }
        }

        public SpawnPoint GetDefaultSpawnPoint()
        {
            AssertWrapper.IsTrue(_spawnPoints.Length > 0, "A submap should have at least one spawn point.");
            return _spawnPoints[0];
        }

        public bool TryGetSpawnPoint(DestinationId id, out SpawnPoint spawnPoint)
        {
            spawnPoint = null;
            foreach (SpawnPoint point in _spawnPoints)
            {
                if (point.Id != id)
                    continue;

                spawnPoint = point;
                return true;
            }
            return false;
        }
        #endregion

        #region Editor
        protected virtual void OnValidate()
        {
            _tiledSuperMap = GetComponentInChildren<SuperMap>(true);
            _spawnPoints = GetComponentsInChildren<SpawnPoint>(true);
            _teleporters = GetComponentsInChildren<Teleporter>(true);
        }
        #endregion
    }
}
